using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WorldBuilder : MonoBehaviour, IFileReceiver
{
    public static WorldBuilder instance = null;

    public GameObject floorPrefab;
    public GameObject wallPrefab;

    public GameObject world;
    public string floorTexPath;
    private Texture2D floorTex;

	private IO io;

    // Hacky workaround to fix fact that mazes build backwards
    public float floorMazeOffset = 0;

    public bool isStartSpecified = false;
    public float robotStartX = 0, robotStartY = 0;
    public string robotStartPhi = "90";

    // Store the locations of 
    public List<float[]> golfBalls;
    public List<float[]> boxes;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    public GameObject ReceiveFile(string filepath)
    {
        SimManager.instance.DestroyWorld();
        floorTex = null;
        EyesimLogger.instance.Log("Loading world: " + filepath);
        world = new GameObject();
		world.name = "World";
        io = new IO("#;");

        if (!io.Load (filepath))
			return null;
		switch (io.extension (filepath))
        {
		    case ".wld":
			    processwld ();
			    break;
		    case ".maz":
                isStartSpecified = false;
                robotStartX = 0;
                robotStartY = 0;
                golfBalls = new List<float[]>();
                boxes = new List<float[]>();
                float size;
                string last = File.ReadAllLines(filepath).Last();
                if (float.TryParse(last, out size))
                    size = size / Eyesim.Scale;
                else
                    size = 0.36f;
			    processmaz (size);
                world.transform.position = new Vector3(0, 0, floorMazeOffset);
                AddObjectsToMaze();
                if(isStartSpecified)
                    AddRobotToMaze();
                break;
		}
        if (floorTex != null)
        {
            Transform floor = world.transform.Find("floor");
            if (floor != null)
            {
                floor.GetComponent<Renderer>().material.mainTexture = floorTex;
                floor.GetComponent<Renderer>().material.mainTextureScale = Vector2.one;
            }
        }
        SimManager.instance.world = world;
        SimManager.instance.worldChanged = true;
        SimManager.instance.worldFilepath = filepath;
        io = null;
        return world;
	}

    private void LoadPNG(string filename)
    {
        Texture2D tex = null;
        byte[] imageData;

        if(File.Exists(filename))
        {
            imageData = File.ReadAllBytes(filename);
            tex = new Texture2D(2, 2);
            tex.LoadImage(imageData);
            floorTexPath = filename;
        }
        floorTex = tex;
    }

    public GameObject CreateBox(int width, int height)
    {
        world = new GameObject("World");
        addFloor(0f, 0f, height / Eyesim.Scale, width / Eyesim.Scale);
        AddWall(new Vector2(0, 0), new Vector2(0, width / Eyesim.Scale));
        AddWall(new Vector2(0, width / Eyesim.Scale), new Vector2(height / Eyesim.Scale, width / Eyesim.Scale));
        AddWall(new Vector2(height / Eyesim.Scale, 0), new Vector2(height / Eyesim.Scale, width / Eyesim.Scale));
        AddWall(new Vector2(0, 0), new Vector2(height / Eyesim.Scale, 0));
        SimManager.instance.world = world;
        SimManager.instance.worldChanged = true;
        return world;
    }

	public void processwld ()
    {
        //string line;
        string[] args;
		float width = -1.0f;
		float height = -1.0f;
		Stack<Vector3> relativepos = new Stack<Vector3> ();
		relativepos.Push (new Vector3(0.0f, 0.0f, 0.0f));
		while ( (args = io.ReadNextArguments())[0] != "ENDOFFILE")
        {
			if (args.Length > 0) {
                if (!(args[0][0] == '#' || args[0][0] == ';'))
                {
					//convert argument string to float list
					List<float> parameters = new List<float> ();
					foreach (string s in args) {
						try{
							parameters.Add(float.Parse(s));
						} catch {
							//nothing, just too many spaces
						}
					}
					switch (args[0])
                    {
					    case "floor":
						    if (parameters.Count < 2)
							    break;
						    addFloor(0,0,parameters[0]/Eyesim.Scale, parameters[1]/Eyesim.Scale);
						    break;
					    case "width":
						    if (parameters.Count < 1)
							    break;
						    width = parameters [0]/Eyesim.Scale;
						    if(width >= 0 && height >= 0){
							    addFloor(0,0,width, height);
						    }
						    break;
					    case "height":
						    if (parameters.Count < 1)
							    break;
						    height = parameters [0]/Eyesim.Scale;
						    if(width >= 0 && height >= 0){
							    addFloor(0,0,width, height);
						    }
						    break;
					    case "position":
						    break;
					    case "push":
						    if (parameters.Count < 3)
							    break;
						    relativepos.Push (new Vector3 (parameters [0]/Eyesim.Scale, parameters [1]/Eyesim.Scale, -parameters[2]));
						    break;
					    case "pop":
						    if (relativepos.Count <= 1)
							    break;
						    relativepos.Pop ();
						    break;
                        case "floor_texture":
                                string texPath = io.SearchForFile(args[1], SettingsManager.instance.GetSetting("worlddir", ""));
                                if (texPath == "")
                                {
                                    EyesimLogger.instance.Log("Unable to find floor texture");
                                    Debug.Log("Couldnt find tex");
                                }
                                else
                                {
                                    LoadPNG(texPath);
                                }
                                break;
                        default:
						    if (parameters.Count < 4)
							    break;
						    Vector2 p1 = mapDomain (new Vector2 (parameters [0]/Eyesim.Scale, parameters [1]/Eyesim.Scale), relativepos);
						    Vector2 p2 = mapDomain (new Vector2 (parameters [2]/Eyesim.Scale, parameters [3]/Eyesim.Scale), relativepos);
                            // Thickness + Height specified
                            if(parameters.Count == 6)
                                AddWall(p1, p2, parameters[4], parameters[5]);
                            // RGB specificed
                            else if(parameters.Count == 9)
                                AddWall(p1, p2, parameters[4], parameters[5], (int) parameters[6], (int) parameters[7], (int) parameters[8]);
                            else
                                AddWall(p1, p2);
						    break;
					}
				}
			}
		}
	}

	Vector2 mapDomain(Vector2 point, Stack<Vector3> relativepos)
    {
		foreach(Vector3 transform in relativepos)
        {
			//rotate point
			point = new Vector2(point.x * Mathf.Cos(Mathf.Deg2Rad * transform.z) - point.y * Mathf.Sin(Mathf.Deg2Rad * transform.z),
				point.x * Mathf.Sin(Mathf.Deg2Rad * transform.z) + point.y * Mathf.Cos(Mathf.Deg2Rad * transform.z));
			//translate point
			point += new Vector2(transform.x, transform.y);
		}
		return point;
	}

	public void processmaz (float size)
    {
		string line;
		float ypos = 0;
		float xmax = 0;
		float ymax = 0;
		while ((line = io.readLine()) != "ENDOFFILE")
        {
			if (line.Length > 0 && (line[0] == '|' || line[0] == ' ' || line[0] == ' '))
            {
				for(int i = 0; i<line.Length; i++)
                {
					float xpos = ((i+1) / 2) * size;
                    if(line[i] == '|')
                    {
                        AddWall(new Vector2(xpos, ypos), new Vector2(xpos, ypos + size));
                        ymax = Mathf.Max(ymax, ypos + size);
                    }
                    else if(line[i] == '_')
                    {
                        AddWall(new Vector2(xpos - size, ypos), new Vector2(xpos, ypos));
                    }
                    else if(char.ToLower(line[i]) == 'o')
                    {
                        golfBalls.Add(new float[2] { (xpos - size / 2f), (ypos + size / 2f) });
                        if(line[i] == 'O')
                            AddWall(new Vector2(xpos - size, ypos), new Vector2(xpos, ypos));
                    }
                    else if(char.ToLower(line[i]) == 'x')
                    {
                        boxes.Add(new float[2] { (xpos - size / 2f), (ypos + size / 2f) });
                        if(line[i] == 'X')
                            AddWall(new Vector2(xpos - size, ypos), new Vector2(xpos, ypos));
                    }
                    // Start position of robot
                    else if("uldrs".Contains(char.ToLower(line[i])))
                    {
                        isStartSpecified = true;
                        robotStartX = (xpos - size/2f);
                        robotStartY = (ypos + size/2f);
                        switch(char.ToLower(line[i]))
                        {
                            case 's':
                            case 'u':
                                robotStartPhi = "90";
                                break;
                            case 'l':
                                robotStartPhi = "180";
                                break;
                            case 'd':
                                robotStartPhi = "270";
                                break;
                            case 'r':
                                robotStartPhi = "0";
                                break;
                            default:
                                Debug.Log("Error reading maze: robot orientation");
                                break;             
                        }
                        if(char.IsUpper(line[i]))
                            AddWall(new Vector2(xpos - size, ypos), new Vector2(xpos, ypos));
                    }
                    xmax = Mathf.Max(xmax, xpos);
				}
				ypos -= size;
			}
		}
		addFloor (0,ypos + size - ymax,xmax, ymax - ypos - size);
	}

	GameObject AddWall (Vector2 start, Vector2 end)
    {
		GameObject wall = Instantiate(wallPrefab);
		wall.name = "wall";
        //wall.layer = 0;
		wall.transform.localScale = new Vector3 (Vector2.Distance(start, end),0.3f,0.01f);
		wall.transform.position = new Vector3 ((end.x+start.x)/2,0.05f,(end.y+start.y)/2);
		wall.transform.rotation = Quaternion.Euler (0,-Mathf.Atan2(end.y-start.y,end.x-start.x)/Mathf.PI*180,0);
		wall.transform.SetParent(world.transform);
        return wall;
	}

    GameObject AddWall(Vector2 start, Vector2 end, float thickness, float height)
    {
        GameObject wall = AddWall(start, end);
        Vector3 lScale = wall.transform.localScale;
        lScale.y = height / Eyesim.Scale;
        lScale.z = thickness / Eyesim.Scale;
        wall.transform.localScale = lScale;
        return wall;
    }

    void AddWall(Vector2 start, Vector2 end, float thickness, float height, int r, int g, int b)
    {
        GameObject wall = AddWall(start, end, thickness, height);
        Renderer rend = wall.GetComponent<Renderer>();
        rend.material.color = new Color(Mathf.Clamp01(r / 255f),  Mathf.Clamp01(g / 255f), Mathf.Clamp01(b / 255f));
    }

	void addFloor (float xpos, float ypos, float width, float height)
    {
		GameObject floor = Instantiate(floorPrefab);
		floor.name = "floor";
        floor.layer = Layers.GroundLayer;
		floor.transform.localScale = new Vector3 (width,0.1f,height);
		floor.transform.position = new Vector3 (xpos + width/2,-0.05f,ypos + height/2);
		floor.transform.SetParent (world.transform);
        floorMazeOffset = -2f * floor.transform.position.z;
    }

    // Add specified objects to the scene
    void AddObjectsToMaze()
    {
        foreach(float[] pos in golfBalls)
        {
            ObjectManager.instance.AddGolfBallToScene((pos[0] * Eyesim.Scale).ToString() + ":" + ((pos[1] + floorMazeOffset) * Eyesim.Scale).ToString() + ":0");
        }

        foreach(float[] pos in boxes)
        {
            ObjectManager.instance.AddBoxToScene((pos[0] * Eyesim.Scale).ToString() + ":" + ((pos[1] + floorMazeOffset) * Eyesim.Scale).ToString() + ":0");
        }
    }

    // Add the robot to the maze if specified starting position given if not loaded from sim file
    void AddRobotToMaze()
    {
        if(!SimReader.instance.readingSimFile)
            ObjectManager.instance.AddS4ToScene((robotStartX*Eyesim.Scale).ToString() + ":" + ((robotStartY + floorMazeOffset)*Eyesim.Scale).ToString() + ":" + robotStartPhi);
    }
}

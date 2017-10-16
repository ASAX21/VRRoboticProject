using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WorldBuilder : MonoBehaviour, IFileReceiver {

    public static WorldBuilder instance = null;

    public GameObject floorPrefab;
    public GameObject wallPrefab;

    public GameObject world;
	public string filepath;
	IO io;

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
		this.filepath = filepath;
        EyesimLogger.instance.Log("Loading world: " + filepath);
        world = new GameObject();
		world.name = "World";
		io = new IO();

        if (!io.Load (filepath))
			return null;
		switch (io.extension (filepath)) {
		    case ".wld":
			    processwld ();
			    break;
		    case ".maz":
                float size;
                string last = File.ReadAllLines(filepath).Last();
                if (float.TryParse(last, out size))
                    size = size / Eyesim.Scale;
                else
                    size = 0.36f;
			    processmaz (size);	
			    break;
		}
        SimManager.instance.world = world;
        SimManager.instance.worldChanged = true;
        return world;
	}

    public GameObject CreateBox(int width, int height)
    {
        world = new GameObject("World");
        addFloor(0f, 0f, height / Eyesim.Scale, width / Eyesim.Scale);
		addWall(new Vector2(0, 0), new Vector2(0, width / Eyesim.Scale));
		addWall(new Vector2(0, width / Eyesim.Scale), new Vector2(height / Eyesim.Scale, width / Eyesim.Scale));
		addWall(new Vector2(height / Eyesim.Scale, 0), new Vector2(height / Eyesim.Scale, width / Eyesim.Scale));
		addWall(new Vector2(0, 0), new Vector2(height / Eyesim.Scale, 0));
        SimManager.instance.world = world;
        SimManager.instance.worldChanged = true;
        return world;
    }

	public void processwld (){
		string line;
		float width = -1.0f;
		float height = -1.0f;
		Stack<Vector3> relativepos = new Stack<Vector3> ();
		relativepos.Push (new Vector3(0.0f, 0.0f, 0.0f));
		while ((line = io.readLine()) != "ENDOFFILE") {
			if (line.Length > 0) {
				if (line [0] != '#' && line [0] != ';') {
					string[] args = line.Split (new char [] {' ','\t'});
					//convert argument string to float list
					List<float> parameters = new List<float> ();
					foreach (string s in args) {
						try{
							parameters.Add(float.Parse(s));
						} catch {
							//nothing, just too many spaces
						}
					}
					switch (args[0]) {
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
					default:
						if (parameters.Count < 4)
							break;
						Vector2 p1 = mapDomain (new Vector2 (parameters [0]/Eyesim.Scale, parameters [1]/Eyesim.Scale), relativepos);
						Vector2 p2 = mapDomain (new Vector2 (parameters [2]/Eyesim.Scale, parameters [3]/Eyesim.Scale), relativepos);
						addWall (p1, p2);
						break;
					}
				}
			}
		}
	}

	Vector2 mapDomain(Vector2 point, Stack<Vector3> relativepos){
		foreach(Vector3 transform in relativepos){
			//rotate point
			point = new Vector2(point.x * Mathf.Cos(Mathf.Deg2Rad * transform.z) - point.y * Mathf.Sin(Mathf.Deg2Rad * transform.z),
				point.x * Mathf.Sin(Mathf.Deg2Rad * transform.z) + point.y * Mathf.Cos(Mathf.Deg2Rad * transform.z));
			//translate point
			point += new Vector2(transform.x, transform.y);
		}
		return point;
	}

	public void processmaz (float size){
		string line;
		float ypos = 0;
		float xmax = 0;
		float ymax = 0;
		while ((line = io.readLine()) != "ENDOFFILE") {
			if (line.Length > 0) {
				try{
					size = float.Parse(line)/Eyesim.Scale;
				} catch {
					for(int i = 0; i<line.Length; i++){
						float xpos = ((i+1) / 2) * size;
						if (i % 2 == 0) {
							if (line [i] == '|') {
								addWall (new Vector2(xpos, ypos), new Vector2(xpos, ypos + size));
								ymax = Mathf.Max (ymax, ypos + size);
							}
						} else {
							if("_SUDLR".Contains(line[i].ToString())){
								addWall (new Vector2(xpos - size, ypos), new Vector2(xpos, ypos)); 
							}
						}
						xmax = Mathf.Max (xmax, xpos);
					}
					ypos -= size;
				}
			}
		}
		print (ymax);
		addFloor (0,ypos + size - ymax,xmax, ymax - ypos - size);
	}

	void addWall (Vector2 start, Vector2 end) {
		GameObject wall = Instantiate(wallPrefab);
		wall.name = "wall";
        wall.layer = 0;
		wall.transform.localScale = new Vector3 (Vector2.Distance(start, end),0.3f,0.01f);
		wall.transform.position = new Vector3 ((end.x+start.x)/2,0.05f,(end.y+start.y)/2);
		wall.transform.rotation = Quaternion.Euler (0,-Mathf.Atan2(end.y-start.y,end.x-start.x)/Mathf.PI*180,0);
		wall.transform.SetParent (world.transform);
	}

	void addFloor (float xpos, float ypos, float width, float height) {
		GameObject floor = Instantiate(floorPrefab);
		floor.name = "floor";
        floor.layer = Layers.GroundLayer;
		floor.transform.localScale = new Vector3 (width,0.1f,height);
		floor.transform.position = new Vector3 (xpos + width/2,-0.05f,ypos + height/2);
		floor.transform.SetParent (world.transform);
	}
}

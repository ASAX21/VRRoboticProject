using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour, IFileReceiver {

    public static WorldBuilder instance = null;

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
			processmaz ();	
			break;
		}
        SimManager.instance.world = world;
		return world;
	}

    public GameObject CreateBox()
    {
        world = new GameObject("World");
        addFloor(0f, 0f, 2f, 2f);
        addWall(0, 0, 0, 2f);
        addWall(0, 2f, 2f, 2f);
        addWall(2f, 0, 2f, 2f);
        addWall(0, 0, 2f, 0);
        return world;
    }

	public void processwld (){
		string line;
		float width = -1.0f;
		float height = -1.0f;
		while ((line = io.readLine()) != "ENDOFFILE") {
			if (line.Length > 0) {
				if (line [0] != '#' && line [0] != ';') {
					string[] args = line.Split (new char [] {' ','\t'});
					//convert argument string to float list
					List<float> parameters = new List<float> ();
					foreach (string s in args) {
						try{
							parameters.Add(float.Parse(s)/1000);
						} catch {
							//nothing, just too many spaces
						}
					}
					switch (args[0]) {
					case "floor":
						print (parameters.Count);
						if (parameters.Count < 2)
							break;
						addFloor(0,0,parameters[0], parameters[1]);
						break;
					case "width":
						if (parameters.Count < 1)
							break;
						width = parameters [0];
						if(width >= 0 && height >= 0){
							addFloor(0,0,width, height);
						}
						break;
					case "height":
						if (parameters.Count < 1)
							break;
						height = parameters [0];
						if(width >= 0 && height >= 0){
							addFloor(0,0,width, height);
						}
						break;
					case "position":
						break;
					default:
						if (parameters.Count < 4)
							break;
						addWall (parameters[0], parameters[1], parameters[2], parameters[3]); 
						break;
					}
				}
			}
		}
	}

	public void processmaz (){
		string line;
		float size = 0.36f; //default wall length
		float ypos = 0;
		float xmax = 0;
		float ymax = 0;
		while ((line = io.readLine()) != "ENDOFFILE") {
			if (line.Length > 0) {
				try{
					size = float.Parse(line)/1000;
				} catch {
					for(int i = 0; i<line.Length; i++){
						float xpos = (i / 2) * size;
						if (i % 2 == 0) {
							if (line [i] == '|') {
								addWall (xpos, ypos, xpos, ypos + size);
								ymax = Mathf.Max (ymax, ypos + size);
							}
						} else {
							if("_SUDLR".Contains(line[i].ToString())){
								addWall (xpos, ypos, xpos + size, ypos); 
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

	void addWall (float x1, float y1, float x2, float y2) {
		GameObject wall = Instantiate(Resources.Load("Wall")) as GameObject;
		wall.name = "wall";
        wall.layer = 0;
		Vector2 start = new Vector2(x1, y1);
		Vector2 end = new Vector2(x2, y2);
		wall.transform.localScale = new Vector3 (Vector2.Distance(start, end),0.3f,0.01f);
		wall.transform.position = new Vector3 ((end.x+start.x)/2,0.05f,(end.y+start.y)/2);
		wall.transform.rotation = Quaternion.Euler (0,Mathf.Atan2(end.y-start.y,end.x-start.x)/Mathf.PI*180,0);
		wall.transform.SetParent (world.transform);
	}

	void addFloor (float xpos, float ypos, float width, float height) {
		GameObject floor = Instantiate(Resources.Load("Floor")) as GameObject;
		floor.name = "floor";
        floor.layer = Layers.GroundLayer;
		floor.transform.localScale = new Vector3 (width,0.1f,height);
		floor.transform.position = new Vector3 (xpos + width/2,-0.05f,ypos + height/2);
		floor.transform.SetParent (world.transform);
	}
}

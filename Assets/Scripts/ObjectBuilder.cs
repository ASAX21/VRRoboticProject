using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ObjectBuilder constructs a gameObject from the .esObj file

public class ObjectBuilder : MonoBehaviour
{
    public GameObject BuildObjectFromFile(string filepath)
    {
        IO io = new IO();
        GameObject customObj = null;
        string line;

        if (!io.Load(filepath))
        {
            Debug.Log("Couldn't load file");
            return null;
        }

        if (io.extension(filepath) != ".esObj")
        {
            Debug.Log("Invalid input to Object Loader");
            return null;
        }

        // Read each line
        while((line = io.readLine()) != "ENDOFFILE")
        {
            // Check for either empty line or comment
            if(line.Length > 0 && line[0] != '#' && line[0] != ';')
            {
                string[] args = line.Split(new char[] {' ', '\t'});
                switch (args[0])
                {
                    // Read the .obj file and construct a new gameObject (MUST BE FIRST NONCOMMENT)
                    case "obj":
                        string objPath = null;
                        if (args[1][0] == '"')
                        {
                            objPath = Regex.Matches(line, "\"[^\"]*\"")[0].ToString();
                            objPath = objPath.Trim('"');
                        }
                        else
                            objPath = args[1];
                        customObj = OBJLoader.LoadOBJFile(objPath);
                        customObj.transform.position = new Vector3(0f, -20f, 0f);
                        customObj.AddComponent<Rigidbody>().isKinematic = true;
                        break;

                    case "scale":
                        if (customObj == null)
                        {
                            Debug.Log("obj must be first input in .esObj file");
                            return null;
                        }
                        try
                        {
                            customObj.transform.localScale = new Vector3(float.Parse(args[1]), float.Parse(args[1]), float.Parse(args[1]));
                        }
                        catch
                        {
                            Debug.Log("Error in scale: Invalid argument");
                            return null;
                        }
                        break;

                    // Configure mass properties
                    case "mass":
                        if(customObj == null)
                        {
                            Debug.Log("obj must be first input in .esObj file");
                            return null;
                        }
                        try
                        {
                            Rigidbody rb = customObj.GetComponent<Rigidbody>();
                            rb.mass = float.Parse(args[1]);
                            rb.centerOfMass = new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
                        }
                        catch
                        {
                            Debug.Log("Error in mass: Invalid argument");
                            Destroy(customObj);
                            return null;
                        }
                        break;

                    // Add colliders
                    // Capsule
                    case "capsule":
                        {
                            if (customObj == null)
                            {
                                Debug.Log("obj must be first input in .esObj file");
                                return null;
                            }
                            CapsuleCollider col = customObj.AddComponent<CapsuleCollider>();
                            try
                            {
                                col.center = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                                col.radius = float.Parse(args[4]);
                                col.height = float.Parse(args[5]);
                                if (args[6] == "x")
                                    col.direction = 0;
                                else if (args[6] == "y")
                                    col.direction = 1;
                                else if (args[6] == "z")
                                    col.direction = 2;
                                else
                                    throw new FormatException();
                            }
                            catch
                            {
                                Debug.Log("Error in capsule: Invalid arguments");
                                Destroy(customObj);
                                return null;
                            }
                            break;
                        }
                    // Sphere
                    case "sphere":
                        {
                            if (customObj == null)
                            {
                                Debug.Log("obj must be first input in .esObj file");
                                return null;
                            }
                            SphereCollider col = customObj.AddComponent<SphereCollider>();
                            try
                            {
                                col.center = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                                col.radius = float.Parse(args[4]);
                            }
                            catch
                            {
                                Debug.Log("Error in sphere: Invalid arguments");
                                Destroy(customObj);
                                return null;
                            }
                            break;
                        }
                    // Box
                    case "box":
                        {
                            if (customObj == null)
                            {
                                Debug.Log("obj must be first input in .esObj file");
                                return null;
                            }
                            BoxCollider col = customObj.AddComponent<BoxCollider>();
                            try
                            {
                                col.center = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                                col.size = new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]));
                            }
                            catch
                            {
                                Debug.Log("Error in capsule: Invalid arguments");
                                Destroy(customObj);
                                return null;
                            }
                            break;
                        }
                    default:
                        {
                            Debug.Log("Unknown input: " + args[0]);
                            break;
                        }
                        
                }
            }
        }
        // Add the WorldObject component, and calculate vertical offset
        customObj.AddComponent<WorldObject>().defaultVerticalOffset = -customObj.GetComponent<Collider>().bounds.min.y - 20f;
        customObj.SetActive(false);
        return customObj;
    }
}

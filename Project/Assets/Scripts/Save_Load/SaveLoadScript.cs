using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveLoadScript : MonoBehaviour {
    // Prefabs
    public GameObject wallPrefab;
    public GameObject windowPrefab;
    public GameObject doorPrefab;

    // Materials
    public Material Unlit_Wall_Material;
    public Material Wall_Material;

    static string folder = "C:/Users/netanelgip/Documents/Models/";
    static public string modelName;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReturnToStartScene()
    {
        // Switch scenes
        SceneManager.LoadScene("Start_scene");
    }

    static public void saveModleName()
    {
        // Get destination
        string destination = folder + "modelName";

        // Open file (or create if doesn't exist)
        FileStream file;
        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        // Create modelData
        ModelName modelname = new ModelName();

        // Save modelData in file
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, modelname);
        file.Close();
    }

    static public void loadModleName()
    {
        // Get destination
        string destination = folder + "modelName";

        // Check if destination exists
        if (!System.IO.File.Exists(destination))
        {
            return;
        }

        // Open file (or create if doesn't exist)
        FileStream file;
        if (File.Exists(destination))
            file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        // Load modelData from file
        BinaryFormatter bf = new BinaryFormatter();
        ModelName modelname = (ModelName)bf.Deserialize(file);
        file.Close();

        SaveLoadScript.modelName = modelname.modelNum.ToString();
    }

    public void Save()
    {
        // Verify valif modelName
        if (modelName == "")
            return;

        // Get destination
        string destination = folder + modelName;

        // Open file (or create if doesn't exist)
        FileStream file;
        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        // Create modelData
        ModelData modelData = new ModelData();

        // Find Model
        GameObject Model = GameObject.Find("Model");

        // Save Model in modelData
        foreach (Transform wall in Model.transform)
        {
            // Create wallStruct
            ModelData.wallStruct wallStruct = new ModelData.wallStruct();
            wallStruct.windows = new List<ModelData.Transform>();
            wallStruct.doors = new List<ModelData.Transform>();

            // Get Wall position
            wallStruct.wallTransform.position_x = wall.gameObject.transform.position.x;
            wallStruct.wallTransform.position_y = wall.gameObject.transform.position.y;
            wallStruct.wallTransform.position_z = wall.gameObject.transform.position.z;

            // Get Wall rotation
            wallStruct.wallTransform.rotation_x = wall.gameObject.transform.rotation.x;
            wallStruct.wallTransform.rotation_y = wall.gameObject.transform.rotation.y;
            wallStruct.wallTransform.rotation_z = wall.gameObject.transform.rotation.z;
            wallStruct.wallTransform.rotation_w = wall.gameObject.transform.rotation.w;

            // Get Start position
            wallStruct.startTransform.position_x = wall.gameObject.transform.Find("Start").transform.position.x;
            wallStruct.startTransform.position_y = wall.gameObject.transform.Find("Start").transform.position.y;
            wallStruct.startTransform.position_z = wall.gameObject.transform.Find("Start").transform.position.z;

            // Get Middle position
            wallStruct.middleTransform.position_x = wall.gameObject.transform.Find("Middle").transform.position.x;
            wallStruct.middleTransform.position_y = wall.gameObject.transform.Find("Middle").transform.position.y;
            wallStruct.middleTransform.position_z = wall.gameObject.transform.Find("Middle").transform.position.z;

            // Get Middle scale
            wallStruct.middleTransform.scale_x = wall.gameObject.transform.Find("Middle").localScale.x;
            wallStruct.middleTransform.scale_y = wall.gameObject.transform.Find("Middle").localScale.y;
            wallStruct.middleTransform.scale_z = wall.gameObject.transform.Find("Middle").localScale.z;

            // Get End position
            wallStruct.endTransform.position_x = wall.gameObject.transform.Find("End").transform.position.x;
            wallStruct.endTransform.position_y = wall.gameObject.transform.Find("End").transform.position.y;
            wallStruct.endTransform.position_z = wall.gameObject.transform.Find("End").transform.position.z;

            // Get Wall_Mesh position
            wallStruct.wallMeshTransform.position_x = wall.gameObject.transform.Find("Wall_Mesh").transform.position.x;
            wallStruct.wallMeshTransform.position_y = wall.gameObject.transform.Find("Wall_Mesh").transform.position.y;
            wallStruct.wallMeshTransform.position_z = wall.gameObject.transform.Find("Wall_Mesh").transform.position.z;

            // Get Wall_Mesh scale
            wallStruct.wallMeshTransform.scale_x = wall.gameObject.transform.Find("Wall_Mesh").localScale.x;
            wallStruct.wallMeshTransform.scale_y = wall.gameObject.transform.Find("Wall_Mesh").localScale.y;
            wallStruct.wallMeshTransform.scale_z = wall.gameObject.transform.Find("Wall_Mesh").localScale.z;

            // Get meshMaterial
            string strName = wall.Find("Wall_Mesh").gameObject.GetComponent<MeshRenderer>().material.name;
            if (wall.Find("Wall_Mesh").gameObject.GetComponent<MeshRenderer>().material.name == "Unlit_Wall (Instance)")
                wallStruct.meshMaterial = ModelData.MaterialType.Unlit_Wall;
            else
                wallStruct.meshMaterial = ModelData.MaterialType.Wall;

            // Windows
            foreach (Transform window in wall.Find("Wall_Mesh").Find("Windows"))
            {
                if (window.tag != "Window")
                    continue;

                // Create windowTransform
                ModelData.Transform windowTransform = new ModelData.Transform();

                // Get window position
                windowTransform.position_x = window.position.x;
                windowTransform.position_y = window.position.y;
                windowTransform.position_z = window.position.z;

                // Get window scale
                windowTransform.scale_x = window.localScale.x;
                windowTransform.scale_y = window.localScale.y;
                windowTransform.scale_z = window.localScale.z;

                // Get window rotation
                windowTransform.rotation_x = window.rotation.x;
                windowTransform.rotation_y = window.rotation.y;
                windowTransform.rotation_z = window.rotation.z;
                windowTransform.rotation_w = window.rotation.w;

                // Save current window
                wallStruct.windows.Add(windowTransform);
            }

            // Doors
            foreach (Transform door in wall)
            {
                if (door.tag != "Door")
                    continue;

                // Create windowTransform
                ModelData.Transform doorTransform = new ModelData.Transform();

                // Get window position
                doorTransform.position_x = door.position.x;
                doorTransform.position_y = door.position.y;
                doorTransform.position_z = door.position.z;

                // Get window scale
                doorTransform.scale_x = door.localScale.x;
                doorTransform.scale_y = door.localScale.y;
                doorTransform.scale_z = door.localScale.z;

                // Get window rotation
                doorTransform.rotation_x = door.rotation.x;
                doorTransform.rotation_y = door.rotation.y;
                doorTransform.rotation_z = door.rotation.z;
                doorTransform.rotation_w = door.rotation.w;

                // Save current window
                wallStruct.doors.Add(doorTransform);
            }

            // Save current wall in modelData
            modelData.walls.Add(wallStruct);
        }

        // Save modelData in file
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, modelData);
        file.Close();
    }

    public void Load(GameObject stand = null)
    {
        // Get destination
        string destination = folder + modelName;

        // Check if destination exists
        if (!System.IO.File.Exists(destination))
        {
            return;
        }

        // Open file (or create if doesn't exist)
        FileStream file;
        if (File.Exists(destination))
            file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        // Load modelData from file
        BinaryFormatter bf = new BinaryFormatter();
        ModelData modelData = (ModelData)bf.Deserialize(file);
        file.Close();

        // Create walls
        foreach (var wallStruct in modelData.walls)
        {
            // Get wall params from wallStruct
            Vector3 VecPosition = new Vector3(wallStruct.wallTransform.position_x, wallStruct.wallTransform.position_y, wallStruct.wallTransform.position_z);
            Quaternion VecRotation = new Quaternion(wallStruct.wallTransform.rotation_x, wallStruct.wallTransform.rotation_y, wallStruct.wallTransform.rotation_z, wallStruct.wallTransform.rotation_w);

            // Create wall
            GameObject Wall = (GameObject)Instantiate(wallPrefab, VecPosition, VecRotation);

            if (stand)
                Wall.transform.SetParent(stand.transform);
            else
                Wall.transform.SetParent(GameObject.Find("Model").transform);

            // Restore start position
            Wall.transform.Find("Start").transform.position = new Vector3(wallStruct.startTransform.position_x, wallStruct.startTransform.position_y, wallStruct.startTransform.position_z);

            // Restore middle position & scale
            Wall.transform.Find("Middle").transform.position = new Vector3(wallStruct.middleTransform.position_x, wallStruct.middleTransform.position_y, wallStruct.middleTransform.position_z);
            Wall.transform.Find("Middle").transform.localScale = new Vector3(wallStruct.middleTransform.scale_x, wallStruct.middleTransform.scale_y, wallStruct.middleTransform.scale_z);

            // Restore end position
            Wall.transform.Find("End").transform.position = new Vector3(wallStruct.endTransform.position_x, wallStruct.endTransform.position_y, wallStruct.endTransform.position_z);

            // Restore Wall_Mesh position & scale & material
            Wall.transform.Find("Wall_Mesh").transform.position = new Vector3(wallStruct.wallMeshTransform.position_x, wallStruct.wallMeshTransform.position_y, wallStruct.wallMeshTransform.position_z);
            Wall.transform.Find("Wall_Mesh").transform.localScale = new Vector3(wallStruct.wallMeshTransform.scale_x, wallStruct.wallMeshTransform.scale_y, wallStruct.wallMeshTransform.scale_z);
            if (wallStruct.meshMaterial == ModelData.MaterialType.Unlit_Wall)
                Wall.transform.Find("Wall_Mesh").gameObject.GetComponent<MeshRenderer>().material = Unlit_Wall_Material;
            else
                Wall.transform.Find("Wall_Mesh").gameObject.GetComponent<MeshRenderer>().material = Wall_Material;

            // Windows
            foreach (ModelData.Transform window in wallStruct.windows)
            {
                Vector3 VecWindowPosition = new Vector3(window.position_x, window.position_y, window.position_z);
                Quaternion VecWindowRotation = new Quaternion(window.rotation_x, window.rotation_y, window.rotation_z, window.rotation_w);

                // Create window
                GameObject Window = (GameObject)Instantiate(windowPrefab, VecWindowPosition, VecWindowRotation);

                // Set window parent
                Window.transform.SetParent(Wall.transform.Find("Wall_Mesh").Find("Windows"));

                // Restore window scale
                Window.transform.localScale = new Vector3(window.scale_x, window.scale_y, window.scale_z);

                // Restore raycast behavior
                Window.transform.Find("Window_Middle").gameObject.layer = LayerMask.NameToLayer("Default");
                Window.transform.Find("Frame_1").gameObject.layer = LayerMask.NameToLayer("Default");
                Window.transform.Find("Frame_2").gameObject.layer = LayerMask.NameToLayer("Default");
                Window.transform.Find("Frame_3").gameObject.layer = LayerMask.NameToLayer("Default");
                Window.transform.Find("Frame_4").gameObject.layer = LayerMask.NameToLayer("Default");
            }

            // Doors
            foreach (ModelData.Transform door in wallStruct.doors)
            {
                Vector3 VecDoorPosition = new Vector3(door.position_x, door.position_y, door.position_z);
                Quaternion VecDoorRotation = new Quaternion(door.rotation_x, door.rotation_y, door.rotation_z, door.rotation_w);

                // Create Door
                GameObject Door = (GameObject)Instantiate(doorPrefab, VecDoorPosition, VecDoorRotation);

                // Set Door parent
                Door.transform.SetParent(Wall.transform);

                // Restore Door scale
                Door.transform.localScale = new Vector3(door.scale_x, door.scale_y, door.scale_z);

                // Restore raycast behavior
                Door.transform.Find("Window_Middle").gameObject.layer = LayerMask.NameToLayer("Default");
                Door.transform.Find("Frame_1").gameObject.layer = LayerMask.NameToLayer("Default");
                Door.transform.Find("Frame_2").gameObject.layer = LayerMask.NameToLayer("Default");
                Door.transform.Find("Frame_3").gameObject.layer = LayerMask.NameToLayer("Default");
                Door.transform.Find("Frame_4").gameObject.layer = LayerMask.NameToLayer("Default");
            }

            // Restore raycast behavior
            Wall.transform.Find("Start").gameObject.layer = LayerMask.NameToLayer("Default");
            Wall.transform.Find("Middle").gameObject.layer = LayerMask.NameToLayer("Default");
            Wall.transform.Find("End").gameObject.layer = LayerMask.NameToLayer("Default");
        }

        //Debug.Log(data.name);
        //Debug.Log(data.score);
        //Debug.Log(data.timePlayed);

    }
}

[Serializable]
public class ModelData
{
    [Serializable]
    public struct Transform
    {
        public float position_x, position_y, position_z;
        public float rotation_x, rotation_y, rotation_z, rotation_w;
        public float scale_x, scale_y, scale_z;
    }

    [Serializable]
    public enum MaterialType
    {
        Unlit_Wall,
        Wall
    }

    [Serializable]
    public struct wallStruct
    {
        // Wall
        public Transform wallTransform;
        public Transform startTransform;
        public Transform middleTransform;
        public Transform endTransform;
        public Transform wallMeshTransform;
        public MaterialType meshMaterial;

        // Windows
        public List<Transform> windows;

        // Doors
        public List<Transform> doors;
    }

    public List<wallStruct> walls;

    public ModelData()
    {
        walls = new List<wallStruct>();
    }
}

[Serializable]
public class ModelName
{
    public int modelNum;

    public ModelName()
    {
        modelNum = int.Parse(SaveLoadScript.modelName);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadScript : MonoBehaviour {
    // Prefabs
    public GameObject wallPrefab;

    string folder = "C:/Users/netanelgip/Documents/Models/";
    static public string modelName = "model.sav";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Save()
    {
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
        foreach (Transform child in Model.transform)
        {
            // Create wallStruct
            ModelData.wallStruct wallStruct = new ModelData.wallStruct();

            // Get Wall position
            wallStruct.wallTransform.position_x = child.gameObject.transform.position.x;
            wallStruct.wallTransform.position_y = child.gameObject.transform.position.y;
            wallStruct.wallTransform.position_z = child.gameObject.transform.position.z;

            // Get Wall rotation
            wallStruct.wallTransform.rotation_x = child.gameObject.transform.rotation.x;
            wallStruct.wallTransform.rotation_y = child.gameObject.transform.rotation.y;
            wallStruct.wallTransform.rotation_z = child.gameObject.transform.rotation.z;
            wallStruct.wallTransform.rotation_w = child.gameObject.transform.rotation.w;

            // Get Start position
            wallStruct.startTransform.position_x = child.gameObject.transform.Find("Start").transform.position.x;
            wallStruct.startTransform.position_y = child.gameObject.transform.Find("Start").transform.position.y;
            wallStruct.startTransform.position_z = child.gameObject.transform.Find("Start").transform.position.z;

            // Get Middle position
            wallStruct.middleTransform.position_x = child.gameObject.transform.Find("Middle").transform.position.x;
            wallStruct.middleTransform.position_y = child.gameObject.transform.Find("Middle").transform.position.y;
            wallStruct.middleTransform.position_z = child.gameObject.transform.Find("Middle").transform.position.z;

            // Get Middle scale
            wallStruct.middleTransform.scale_x = child.gameObject.transform.Find("Middle").localScale.x;
            wallStruct.middleTransform.scale_y = child.gameObject.transform.Find("Middle").localScale.y;
            wallStruct.middleTransform.scale_z = child.gameObject.transform.Find("Middle").localScale.z;

            // Get End position
            wallStruct.endTransform.position_x = child.gameObject.transform.Find("End").transform.position.x;
            wallStruct.endTransform.position_y = child.gameObject.transform.Find("End").transform.position.y;
            wallStruct.endTransform.position_z = child.gameObject.transform.Find("End").transform.position.z;

            // Get Wall_Mesh position
            wallStruct.wallMeshTransform.position_x = child.gameObject.transform.Find("Wall_Mesh").transform.position.x;
            wallStruct.wallMeshTransform.position_y = child.gameObject.transform.Find("Wall_Mesh").transform.position.y;
            wallStruct.wallMeshTransform.position_z = child.gameObject.transform.Find("Wall_Mesh").transform.position.z;

            // Get Wall_Mesh scale
            wallStruct.wallMeshTransform.scale_x = child.gameObject.transform.Find("Wall_Mesh").localScale.x;
            wallStruct.wallMeshTransform.scale_y = child.gameObject.transform.Find("Wall_Mesh").localScale.y;
            wallStruct.wallMeshTransform.scale_z = child.gameObject.transform.Find("Wall_Mesh").localScale.z;

            // Save current wall in modelData
            modelData.walls.Add(wallStruct);
        }

        // Save modelData in file
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, modelData);
        file.Close();
    }

    public void Load()
    {
        // Get destination
        string destination = folder + modelName;

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
            Wall.transform.SetParent(GameObject.Find("Model").transform);

            // Restore start position
            Wall.transform.Find("Start").transform.position = new Vector3(wallStruct.startTransform.position_x, wallStruct.startTransform.position_y, wallStruct.startTransform.position_z);

            // Restore middle position & scale
            Wall.transform.Find("Middle").transform.position = new Vector3(wallStruct.middleTransform.position_x, wallStruct.middleTransform.position_y, wallStruct.middleTransform.position_z);
            Wall.transform.Find("Middle").transform.localScale = new Vector3(wallStruct.middleTransform.scale_x, wallStruct.middleTransform.scale_y, wallStruct.middleTransform.scale_z);

            // Restore end position
            Wall.transform.Find("End").transform.position = new Vector3(wallStruct.endTransform.position_x, wallStruct.endTransform.position_y, wallStruct.endTransform.position_z);

            // Restore Wall_Mesh position & scale
            Wall.transform.Find("Wall_Mesh").transform.position = new Vector3(wallStruct.wallMeshTransform.position_x, wallStruct.wallMeshTransform.position_y, wallStruct.wallMeshTransform.position_z);
            Wall.transform.Find("Wall_Mesh").transform.localScale = new Vector3(wallStruct.wallMeshTransform.scale_x, wallStruct.wallMeshTransform.scale_y, wallStruct.wallMeshTransform.scale_z);

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
    public struct wallStruct
    {
        public Transform wallTransform;
        public Transform startTransform;
        public Transform middleTransform;
        public Transform endTransform;
        public Transform wallMeshTransform;
    }

    public List<wallStruct> walls;

    public ModelData()
    {
        walls = new List<wallStruct>();
    }
}
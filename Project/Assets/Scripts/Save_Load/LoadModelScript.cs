using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModelScript : MonoBehaviour
{
    SaveLoadScript saveLoadScript;
    bool loaded = false;
    public string modelName;

    // Use this for initialization
    void Start()
    {
        SaveLoadScript.loadModleName();
    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded)
        {
            saveLoadScript = GetComponent<SaveLoadScript>();
            saveLoadScript.Load();

            loaded = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModelScript : MonoBehaviour
{
    SaveLoadScript saveLoadScript;
    bool loaded = false;

    // Use this for initialization
    void Start()
    {

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
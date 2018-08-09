using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using HTC.UnityPlugin.Vive;
using VRTK.Highlighters;
using UnityEngine.SceneManagement;

public class UsableStand : VRTK.VRTK_InteractableObject
{
    // Load model data
    SaveLoadScript saveLoadScript;
    public string modelname;
    GameObject Ground;

    // Modify color
    public Material Wall_Atlas_01_Green;
    public Material Wall_Atlas_01_Dif;

    bool usingActive = false;

    public void Start()
    {
        // Get script
        saveLoadScript = GetComponent<SaveLoadScript>();
        
        Ground = transform.parent.Find("Ground").gameObject;
        Ground.transform.localScale = new Vector3(2f, 10f, 2f);

        SaveLoadScript.modelName = modelname;
        saveLoadScript.Load(Ground);

        Ground.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (usingActive)
        {
            if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger) && modelname!="")
            {
                // Update model name
                SaveLoadScript.modelName = modelname;

                // Switch scenes
                SceneManager.LoadScene("Edit_scene");
            }
        }
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(currentUsingObject);
        HighlightStand();

        usingActive = true;
    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);
        UnHighlightStand();

        usingActive = false;
    }

    public void HighlightStand()
    {
        Material[] mats = GetComponent<MeshRenderer>().materials;
        mats[0] = Wall_Atlas_01_Green;
        GetComponent<MeshRenderer>().materials = mats;
    }

    public void UnHighlightStand()
    {
        Material[] mats = GetComponent<MeshRenderer>().materials;
        mats[0] = Wall_Atlas_01_Dif;
        GetComponent<MeshRenderer>().materials = mats;
    }
}

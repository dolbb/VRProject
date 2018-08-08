using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using HTC.UnityPlugin.Vive;

public class UsableWindowSectionScript : UsableScript
{
    // Cursor data
    GameObject controllerData;
    controllerDataScript controllerDataScript;

    // Edit menu data
    GameObject EditMenu;
    public EditScript EditMenuScript;

    // Window data
    public GameObject windowPrefab;
    GameObject window;

    // Window highlighter
    VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter windowHighlighter;

    public Material Unlit_Window_Highlight;
    public Material Unlit_Window;
    public Material Frame_Material_Highlight;
    public Material Frame_Material;

    public override void Start()
    {
        base.Start();

        // Get controllerDataScript
        controllerData = GameObject.Find("Controller_Data");
        controllerDataScript = controllerData.GetComponent<controllerDataScript>();

        // Get EditMenuScript
        EditMenu = GameObject.Find("Edit menu");
        EditMenuScript = EditMenu.GetComponent<EditScript>();

        // Get windowHighlighter
        windowHighlighter = transform.parent.Find("Window_Mesh").gameObject.GetComponent<VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(currentUsingObject);
    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);
    }

    // Highlight entire window
    public void HighlightWindow()
    {
        // Disable start,middle,end meshes
        //transform.parent.gameObject.transform.Find("Window_Middle").gameObject.GetComponent<Renderer>().enabled = false;

        // Highlight window
        if (!windowHighlighter)
        {
            // Get windowHighlighter
            windowHighlighter = transform.parent.Find("Window_Mesh").gameObject.GetComponent<VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter>();
        }
        windowHighlighter.Highlight(Color.green);
        transform.parent.transform.Find("Window_Mesh").gameObject.GetComponent<MeshRenderer>().material = Unlit_Window_Highlight;
    }

    // Unhighlight entire window
    public void UnHighlightWindow()
    {
        // Enable start,middle,end meshes
        //transform.parent.gameObject.transform.Find("Window_Middle").gameObject.GetComponent<Renderer>().enabled = true;

        // Unhighlight window
        windowHighlighter.Unhighlight();
        transform.parent.transform.Find("Window_Mesh").gameObject.GetComponent<MeshRenderer>().material = Unlit_Window;
    }
}

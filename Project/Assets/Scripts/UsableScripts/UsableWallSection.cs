using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using HTC.UnityPlugin.Vive;

public class UsableWallSectionScript : UsableScript
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

    // Door data
    public GameObject doorPrefab;
    GameObject door;

    // Wall highlighter
    VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter wallHighlighter;

    public Material Unlit_Wall_Highlight;
    public Material Unlit_Wall;

    public override void Start()
    {
        base.Start();

        // Get controllerDataScript
        controllerData = GameObject.Find("Controller_Data");
        if (controllerData)
            controllerDataScript = controllerData.GetComponent<controllerDataScript>();

        // Get EditMenuScript
        EditMenu = GameObject.Find("Edit menu");
        if (EditMenu)
            EditMenuScript = EditMenu.GetComponent<EditScript>();

        // Get wallHighlighter
        wallHighlighter = transform.parent.Find("Wall_Mesh").gameObject.GetComponent<VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (!EditMenuScript)
            return;

        if (EditMenuScript.curr_state == EditScript.state.Add_Window && window)
        {
            AdjustWindow();
        }

        else if (EditMenuScript.curr_state == EditScript.state.Add_Door && door)
        {
            AdjustDoor();
        }
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(currentUsingObject);

        if (!EditMenuScript)
            return;

        if (EditMenuScript.curr_state == EditScript.state.Add_Window)
        {
            // Create window
            if (!window)
            {
                window = (GameObject)Instantiate(windowPrefab);
            }
        }

        else if (EditMenuScript.curr_state == EditScript.state.Add_Door)
        {
            // Create window
            if (!door)
            {
                door = (GameObject)Instantiate(doorPrefab);
            }
        }
    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);

        if (EditMenuScript.curr_state == EditScript.state.Add_Window)
        {
            if (window)
            {
                window.SetActive(false);
                Destroy(window);
            }

            window = null;
        }

        else if (EditMenuScript.curr_state == EditScript.state.Add_Door)
        {
            if (door)
            {
                door.SetActive(false);
                Destroy(door);
            }

            door = null;
        }
    }

    // Highlight entire wall
    public void HighlightWall()
    {
        // Disable start,middle,end meshes
        //transform.parent.gameObject.transform.Find("Start").gameObject.GetComponent<Renderer>().enabled = false;
        //transform.parent.gameObject.transform.Find("Middle").gameObject.GetComponent<Renderer>().enabled = false;
        //transform.parent.gameObject.transform.Find("End").gameObject.GetComponent<Renderer>().enabled = false;

        // Highlight wall
        wallHighlighter.Highlight(Color.green);
        transform.parent.transform.Find("Wall_Mesh").gameObject.GetComponent<MeshRenderer>().material = Unlit_Wall_Highlight;
    }

    // Unhighlight entire wall
    public void UnHighlightWall()
    {
        // Enable start,middle,end meshes
        //transform.parent.gameObject.transform.Find("Start").gameObject.GetComponent<Renderer>().enabled = true;
        //transform.parent.gameObject.transform.Find("Middle").gameObject.GetComponent<Renderer>().enabled = true;
        //transform.parent.gameObject.transform.Find("End").gameObject.GetComponent<Renderer>().enabled = true;

        // Unhighlight wall
        wallHighlighter.Unhighlight();
        transform.parent.transform.Find("Wall_Mesh").gameObject.GetComponent<MeshRenderer>().material = Unlit_Wall;
    }

    public void AdjustWindow()
    {
        // Get middle
        GameObject currWallSection = controllerDataScript.curr_game_object;

        // Edge case: controllerDataScript is called after this script
        if (!currWallSection)
            return;

        // Edge case: entering a "Middle" but controllerDataScript.curr_game_object still points to the ground
        if (currWallSection.tag != "Middle" && currWallSection.tag != "Wall_Edge")
            return;

        GameObject wall = controllerDataScript.curr_game_object.transform.parent.gameObject;
        GameObject start = wall.transform.Find("Start").gameObject;

        // Get line
        Vector3 lineVec = controllerDataScript.direction;
        Vector3 linePoint = controllerDataScript.worldPoint;

        // Get plane
        Vector3 planeNormal = controllerDataScript.normal;
        Vector3 intersection = controllerDataScript.worldPoint;

        // Adjust window transform
        window.transform.position = intersection - planeNormal.normalized * wall.transform.localScale.x / 2;
        window.transform.rotation = currWallSection.transform.rotation;

        // Attach to wall
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            // Restore raycast behavior
            window.layer = LayerMask.NameToLayer("Default");

            // Save in "Windows" field
            window.transform.parent = currWallSection.transform.parent.transform.Find("Wall_Mesh").transform.Find("Windows").transform;

            // Restore raycast behavior
            window.transform.Find("Window_Middle").gameObject.layer = LayerMask.NameToLayer("Default");
            window.transform.Find("Frame_1").gameObject.layer = LayerMask.NameToLayer("Default");
            window.transform.Find("Frame_2").gameObject.layer = LayerMask.NameToLayer("Default");
            window.transform.Find("Frame_3").gameObject.layer = LayerMask.NameToLayer("Default");
            window.transform.Find("Frame_4").gameObject.layer = LayerMask.NameToLayer("Default");

            // The window
            window = null;
        }
    }


    public void AdjustDoor()
    {
        // Get middle
        GameObject currWallSection = controllerDataScript.curr_game_object;

        // Edge case: controllerDataScript is called after this script
        if (!currWallSection)
            return;

        // Edge case: entering a "Middle" but controllerDataScript.curr_game_object still points to the ground
        if (currWallSection.tag != "Middle" && currWallSection.tag != "Wall_Edge")
            return;

        GameObject wall = controllerDataScript.curr_game_object.transform.parent.gameObject;
        GameObject start = wall.transform.Find("Start").gameObject;

        // Get line
        Vector3 lineVec = controllerDataScript.direction;
        Vector3 linePoint = controllerDataScript.worldPoint;

        // Get plane
        Vector3 planeNormal = controllerDataScript.normal;
        Vector3 intersection = controllerDataScript.worldPoint;

        // Adjust window transform
        door.transform.position = intersection - planeNormal.normalized * wall.transform.localScale.x / 2;
        door.transform.position = new Vector3(door.transform.position.x, door.transform.localScale.y / 2, door.transform.position.z);
        door.transform.rotation = currWallSection.transform.rotation;

        // Attach to wall
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            // Restore raycast behavior
            door.layer = LayerMask.NameToLayer("Default");

            // Save in "Windows" field
            door.transform.parent = currWallSection.transform.parent.transform;

            // Restore raycast behavior
            door.transform.Find("Window_Middle").gameObject.layer = LayerMask.NameToLayer("Default");
            door.transform.Find("Frame_1").gameObject.layer = LayerMask.NameToLayer("Default");
            door.transform.Find("Frame_2").gameObject.layer = LayerMask.NameToLayer("Default");
            door.transform.Find("Frame_3").gameObject.layer = LayerMask.NameToLayer("Default");
            door.transform.Find("Frame_4").gameObject.layer = LayerMask.NameToLayer("Default");

            // The door
            door = null;
        }
    }
}

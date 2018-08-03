using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using HTC.UnityPlugin.Vive;

public class UsableMiddleScript : UsableScript
{
    // Cursor data
    GameObject controllerData;
    controllerDataScript controllerDataScript;

    // Edit menu data
    GameObject EditMenu;
    EditScript EditMenuScript;

    // Window data
    public GameObject windowPrefab;
    GameObject window;

    public override void Start()
    {
        base.Start();

        // Get controllerDataScript
        controllerData = GameObject.Find("Controller_Data");
        controllerDataScript = controllerData.GetComponent<controllerDataScript>();

        // Get EditMenuScript
        EditMenu = GameObject.Find("Edit menu");
        EditMenuScript = EditMenu.GetComponent<EditScript>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (EditMenuScript.curr_state == EditScript.state.Add_Window && window)
        {
            // Get middle
            GameObject middle = controllerDataScript.curr_game_object;

            // Edge case: entering a "Middle" but controllerDataScript.curr_game_object still points to the ground
            if (middle.tag != "Middle")
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
            window.transform.rotation = middle.transform.rotation;

            // Attach to wall
            if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
            {
                // Restore raycast behavior
                window.layer = LayerMask.NameToLayer("Default");

                // Save in "Windows" field
                window.transform.parent = middle.transform.parent.transform.Find("Windows").transform;

                // The window
                window = null;
            }
        }
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(currentUsingObject);

        if (EditMenuScript.curr_state == EditScript.state.Add_Window)
        {
            // Create window
            if (!window)
            {
                window = (GameObject)Instantiate(windowPrefab);
            }
        }
    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);

        if (EditMenuScript.curr_state == EditScript.state.Add_Window)
        {
            if (window)
                Destroy(window);
            window = null;
        }
    }
}

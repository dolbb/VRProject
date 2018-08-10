using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UsableEdgeScript : UsableWallSectionScript
{
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        GetComponent<MeshRenderer>().enabled = true;
        base.StartUsing(currentUsingObject);

        if (EditMenuScript.curr_mode == EditScript.Mode.Move || EditMenuScript.curr_state == EditScript.state.Create_Wall)
        {
            // Highlight current edge
            //transform.parent.transform.Find("Wall_Mesh").gameObject.GetComponent<Renderer>().enabled = false;
            highlighter.Highlight(Color.green);
            GetComponent<MeshRenderer>().material = Unlit_Wall_Highlight;
        }

        else
        {
            HighlightWall();
        }
            
    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);

        if (EditMenuScript.curr_mode == EditScript.Mode.Move || EditMenuScript.curr_state == EditScript.state.Create_Wall)
        {
            // UnHighlight current edge
            //transform.parent.transform.Find("Wall_Mesh").gameObject.GetComponent<Renderer>().enabled = true;
            highlighter.Unhighlight();
            GetComponent<MeshRenderer>().material = Unlit_Wall;
        }

        else
        {
            UnHighlightWall();
        }

        GetComponent<MeshRenderer>().enabled = false;
    }
}
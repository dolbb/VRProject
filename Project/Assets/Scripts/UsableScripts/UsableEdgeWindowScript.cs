using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UsableEdgeWindowScript : UsableWindowSectionScript
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
        //GetComponent<MeshRenderer>().enabled = true;
        base.StartUsing(currentUsingObject);

        if (!EditMenuScript)
            return;

        if (EditMenuScript.curr_mode == EditScript.Mode.Move)
        {
            // Highlight current edge
            highlighter.Highlight(Color.green);
            GetComponent<MeshRenderer>().material = Frame_Material_Highlight;
        }

    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);

        if (EditMenuScript.curr_mode == EditScript.Mode.Move)
        {
            // UnHighlight current edge
            highlighter.Unhighlight();
            GetComponent<MeshRenderer>().material = Frame_Material;
        }

        //GetComponent<MeshRenderer>().enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using HTC.UnityPlugin.Vive;

public class UsableMiddleWindowScript : UsableWindowSectionScript
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
        base.StartUsing(currentUsingObject);

        if (!EditMenuScript)
            return;

        if (EditMenuScript.curr_mode == EditScript.Mode.Move)
        {
            // Highlight Window_Middle
            HighlightWindow();
        }
    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);

        if (EditMenuScript.curr_mode == EditScript.Mode.Move)
        {
            // Unhighlight Window_Middle
            UnHighlightWindow();
        }
    }
}

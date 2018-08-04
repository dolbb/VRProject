using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UsableScript : VRTK.VRTK_InteractableObject
{
    public VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter highlighter;

    public virtual void Start()
    {
        highlighter = GetComponent<VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter>();
        highlighter.Initialise();
    }

    // Update is called once per frame
    public virtual void Update()
    {
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        // Call base VRTK_InteractableObject
        base.StartUsing(currentUsingObject);

        // Highlight GameObject
        //highlighter.Highlight(Color.green);
    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        // Call base VRTK_InteractableObject
        base.StopUsing(previousUsingObject, resetUsingObjectState);

        // Unhighlight GameObject
        //highlighter.Unhighlight();
    }
}
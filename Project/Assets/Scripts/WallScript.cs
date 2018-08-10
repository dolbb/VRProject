using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class WallScript : MonoBehaviour {
    VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter highlighter;

    // Use this for initialization
    void Start () {
        highlighter = GetComponent<VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter>();
        highlighter.Initialise();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

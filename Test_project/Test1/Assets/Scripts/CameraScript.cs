using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public Vector3 worldPoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		updateWorldPoint ();
	}

	void updateWorldPoint()
	{
		Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			worldPoint = hit.point;
			return;
		}
		worldPoint = Vector3.zero; // Never reaching here
	}
}

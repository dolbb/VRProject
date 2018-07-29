using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	// Data of current camera point
	public Vector3 worldPoint;
	public GameObject curr_game_object;

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
			curr_game_object = hit.transform.gameObject;
			return;
		}
		worldPoint = Vector3.zero; // Never reaching here
	}
}

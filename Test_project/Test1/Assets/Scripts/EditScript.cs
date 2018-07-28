using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditScript : MonoBehaviour {

	public GameObject mainCamera;
	CameraScript camScript;

	bool creating;
	public GameObject start;
	public GameObject end;

	public GameObject wallPrefab;
	GameObject wall;

	// Use this for initialization
	void Start()
	{
		camScript = mainCamera.GetComponent<CameraScript>();
	}

	// Update is called once per frame
	void Update()
	{
		getInput();
	}

	void getInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			setStart();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			setEnd();
		}
		else
		{
			if (creating)
				adjust();
		}
	}

	void setStart()
	{
		creating = true;
		start.transform.position = camScript.worldPoint;
		wall = (GameObject)Instantiate(wallPrefab, start.transform.position, Quaternion.identity);
	}

	void setEnd()
	{
		creating = false;
		end.transform.position = camScript.worldPoint;
	}

	void adjust()
	{
		end.transform.position = camScript.worldPoint;
		adjustWall();
	}

	void adjustWall()
	{
		start.transform.LookAt(end.transform.position);
		end.transform.LookAt(start.transform.position);
		float distance = Vector3.Distance(start.transform.position, end.transform.position);
		wall.transform.position = start.transform.position + distance / 2 * start.transform.forward;
		wall.transform.rotation = start.transform.rotation;
		wall.transform.localScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, distance);
	}

}

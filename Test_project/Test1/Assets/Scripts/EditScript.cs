using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditScript : MonoBehaviour {
	// Cursor data
	public GameObject mainCamera;
	CameraScript camScript;

	// Wall data
	public GameObject wallPrefab;
	GameObject wall;

	// Menu state
	public enum state {
		Idle,
		Create_Wall,
		Move_Wall,
		Move_Edge
	};
	public state curr_state;

	// Flag (has operation begun?)
	bool operating;

	// Move data
	Vector3 move_start_cam_pos;
	Vector3 move_start_wall_pos;

	// Initialization
	void Start()
	{
		camScript = mainCamera.GetComponent<CameraScript>();
		operating = false;
	}

	// Update is called once per frame
	void Update()
	{
		switch (curr_state)
		{
		case state.Idle:
			break;
		case state.Create_Wall:
			Handle_Create_Wall ();
			break;
		case state.Move_Wall:
			Handle_Move_Wall ();
			break;
		case state.Move_Edge:
			Handle_Move_Edge ();
			break;
		}			
	}

	/// <summary>
	/// Create wall
	/// Manipulation is done on tmpWall (to ignore raycast)
	/// </summary>
	#region Create_Wall
	void Handle_Create_Wall()
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
			if (operating)
				adjust();
		}
	}

	void setStart()
	{
		operating = true;

		// Set start
		Vector3 startPosition = camScript.worldPoint;
		startPosition += new Vector3(0, wallPrefab.transform.Find("Middle").localScale.y/2, 0);	// Align wall to ground

		// Create wall
		wall = (GameObject)Instantiate(wallPrefab, startPosition, Quaternion.identity);
	}

	void setEnd()
	{
		operating = false;

		// Get wall params
		GameObject start = wall.transform.Find("Start").gameObject;
		GameObject middle = wall.transform.Find("Middle").gameObject;
		GameObject end = wall.transform.Find("End").gameObject;

		// Set end
		end.transform.position = new Vector3(camScript.worldPoint.x, wallPrefab.transform.Find("Middle").localScale.y/2, camScript.worldPoint.z);

		// Restore raycast behavior
		start.layer = LayerMask.NameToLayer("Default");
		middle.layer = LayerMask.NameToLayer("Default");
		end.layer = LayerMask.NameToLayer("Default");
	}

	void adjust()
	{
		// Get wall params
		GameObject end = wall.transform.Find("End").gameObject;

		// Set end
		end.transform.position = new Vector3(camScript.worldPoint.x, wallPrefab.transform.Find("Middle").localScale.y/2, camScript.worldPoint.z);

		// Adjust wall
		adjustWall();
	}

	// Adjust Middle according to Start & End
	void adjustWall()
	{
		// Get wall params
		GameObject start = wall.transform.Find("Start").gameObject;
		GameObject middle = wall.transform.Find("Middle").gameObject;
		GameObject end = wall.transform.Find("End").gameObject;

		// Make start & end face each other
		start.transform.LookAt(end.transform.position);
		end.transform.LookAt(start.transform.position);

		// Calculate distance between start & end
		float distance = Vector3.Distance(start.transform.position, end.transform.position);

		// Adjust wall transform
		middle.transform.position = start.transform.position + distance / 2 * start.transform.forward;
		middle.transform.rotation = start.transform.rotation;
		middle.transform.localScale = new Vector3(middle.transform.localScale.x, middle.transform.localScale.y, distance);
	}
	#endregion

	/// <summary>
	/// Move wall
	/// </summary>
	#region Move_Wall
	void Handle_Move_Wall()
	{
		// Start move wall
		if (Input.GetMouseButtonDown(0) && camScript.curr_game_object.tag == "Middle")
		{
			operating = true;

			// Save start data
			wall = camScript.curr_game_object.transform.parent.gameObject;
			move_start_cam_pos = camScript.worldPoint;
			move_start_wall_pos = wall.transform.position;

			// Ignore raycast while moving
			wall.transform.Find("Start").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			wall.transform.Find("Middle").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			wall.transform.Find("End").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}

		// End move
		else if (Input.GetMouseButtonUp(0) && operating)
		{
			operating = false;

			// Restore raycast behavior
			wall.transform.Find("Start").gameObject.layer = LayerMask.NameToLayer("Default");
			wall.transform.Find("Middle").gameObject.layer = LayerMask.NameToLayer("Default");
			wall.transform.Find("End").gameObject.layer = LayerMask.NameToLayer("Default");
		}

		// During move
		else
		{
			if (operating) 
			{
				Vector3 delta = new Vector3(camScript.worldPoint.x - move_start_cam_pos.x,0,camScript.worldPoint.z - move_start_cam_pos.z);
				wall.transform.position = move_start_wall_pos + delta;
			}
				
		}
	}
	#endregion

	/// <summary>
	/// Move Edge
	/// </summary>
	#region Move_Edge

	void Handle_Move_Edge()
	{
		// Start move edge
		if (Input.GetMouseButtonDown(0) && camScript.curr_game_object.tag == "Wall_Edge")
		{
			operating = true;

			// Save start data
			GameObject edge = camScript.curr_game_object;
			wall = camScript.curr_game_object.transform.parent.gameObject;
			GameObject start = wall.transform.Find("Start").gameObject;
			GameObject middle = wall.transform.Find("Middle").gameObject;
			GameObject end = wall.transform.Find("End").gameObject;

			// Edge should be End
			if (end.GetInstanceID() != edge.GetInstanceID()){
				start.transform.position = end.transform.position;
				end.transform.position = edge.transform.position;
			}

			// Ignore raycast while moving
			start.layer = LayerMask.NameToLayer("Ignore Raycast");
			middle.layer = LayerMask.NameToLayer("Ignore Raycast");
			end.layer = LayerMask.NameToLayer("Ignore Raycast");
		}

		// End move edge
		else if (Input.GetMouseButtonUp(0) && operating)
		{
			operating = false;

			setEnd ();
		}

		// During move edge
		else
		{
			if (operating) 
			{
				adjust ();
			}

		}
	}

	#endregion
}

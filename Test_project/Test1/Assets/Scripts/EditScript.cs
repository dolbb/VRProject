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
	Vector3 startPosition;
	Vector3 endPosition;

	// Window data
	public GameObject windowPrefab;
	GameObject window;

	// Menu state
	public enum state {
		Idle,
		Create_Wall,
		Move_Wall,
		Move_Edge,
		Add_Window
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
		case state.Add_Window:
			Handle_Add_Window ();
			break;
		}			
	}

	/// <summary>
	/// Create wall
	/// </summary>
	#region Create_Wall
	void Handle_Create_Wall()
	{
		if (Input.GetMouseButtonDown(0))
		{
			create_wall();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			setWall();
		}
		else
		{
			if (operating)
				adjust();
		}
	}

	void create_wall()
	{
		operating = true;

		// Get startPosition
		startPosition = camScript.worldPoint;
		startPosition += new Vector3(0, wallPrefab.transform.Find("Middle").localScale.y/2, 0);	// Align wall to ground

		// Create wall
		wall = (GameObject)Instantiate(wallPrefab, startPosition, Quaternion.identity);
	}

	void setWall()
	{
		operating = false;

		// Get wall params
		GameObject start = wall.transform.Find("Start").gameObject;
		GameObject middle = wall.transform.Find("Middle").gameObject;
		GameObject end = wall.transform.Find("End").gameObject;

		// Restore raycast behavior
		start.layer = LayerMask.NameToLayer("Default");
		middle.layer = LayerMask.NameToLayer("Default");
		end.layer = LayerMask.NameToLayer("Default");
	}

	void adjust()
	{
		// Calculate new_end_position
		endPosition = new Vector3(camScript.worldPoint.x, wallPrefab.transform.Find("Middle").localScale.y/2, camScript.worldPoint.z);

		// Calcualte (new end in relation to start)
		Vector3 direction = endPosition - startPosition;
		float distance = Vector3.Distance(startPosition, endPosition);

		// Get preFab middle length size
		float prefabz = wallPrefab.transform.Find("Middle").localScale.z;

		// Adjust wall transform
		wall.transform.position = startPosition + (direction / 2);
		wall.transform.rotation = Quaternion.LookRotation(direction);
		wall.transform.localScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, distance/prefabz);
	}
	
	#endregion

	/// <summary>
	/// Move wall
	/// </summary>
	#region Move_Wall
	void Handle_Move_Wall()
	{
		// Start
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

		// End
		else if (Input.GetMouseButtonUp(0) && operating)
		{
			operating = false;

			// Restore raycast behavior
			wall.transform.Find("Start").gameObject.layer = LayerMask.NameToLayer("Default");
			wall.transform.Find("Middle").gameObject.layer = LayerMask.NameToLayer("Default");
			wall.transform.Find("End").gameObject.layer = LayerMask.NameToLayer("Default");

			// Override global variable
			wall = null;
		}

		// During
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
		// Start
		if (Input.GetMouseButtonDown(0) && camScript.curr_game_object.tag == "Wall_Edge")
		{
			operating = true;

			// Save start data
			wall = camScript.curr_game_object.transform.parent.gameObject;

			// Save start data
			GameObject edge = camScript.curr_game_object;
			wall = camScript.curr_game_object.transform.parent.gameObject;
			GameObject start = wall.transform.Find("Start").gameObject;
			GameObject middle = wall.transform.Find("Middle").gameObject;
			GameObject end = wall.transform.Find("End").gameObject;

			// Edge should be End
			if (end.GetInstanceID() != edge.GetInstanceID()){
				startPosition = end.transform.position;
				end.transform.position = start.transform.position;
				start.transform.position = startPosition;
			}

			startPosition = start.transform.position;

			// Ignore raycast while moving
			start.layer = LayerMask.NameToLayer("Ignore Raycast");
			middle.layer = LayerMask.NameToLayer("Ignore Raycast");
			end.layer = LayerMask.NameToLayer("Ignore Raycast");

		}

		// End
		else if (Input.GetMouseButtonUp(0) && operating)
		{
			operating = false;

			setWall ();
		}

		// During
		else
		{
			if (operating) 
			{
				adjust ();
			}

		}
	}

	#endregion

	/// <summary>
	/// Add Window
	/// </summary>
	#region Add_Window

	void Handle_Add_Window()
	{
		// Hovering over "middle"
		if (camScript.curr_game_object.tag == "Middle") {

			// Create window
			if (!window) {
				window = (GameObject)Instantiate(windowPrefab);
			}

			// Get middle
			GameObject middle = camScript.curr_game_object;

			// Adjust window transform
			window.transform.position = camScript.worldPoint;
			window.transform.rotation = middle.transform.rotation;

			// Attach to wall
			if (Input.GetMouseButtonUp(0))
			{
				// Restore raycast behavior
				window.layer = LayerMask.NameToLayer("Default");

				// Save in "Windows" field
				window.transform.parent = middle.transform.parent.transform.Find("Windows").transform;

				// The window
				window = null;
			}
		}
		
		// Not hovering over "middle" 
		else {
			// Make the window disappear
			if (window)
				Destroy(window);
			window = null;
		}
	}
	#endregion

}

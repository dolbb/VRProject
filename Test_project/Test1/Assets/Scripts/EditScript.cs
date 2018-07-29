using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditScript : MonoBehaviour {

	public GameObject mainCamera;
	CameraScript camScript;

	bool creating;
	public GameObject start;
	public GameObject end;

	public GameObject tmpWallPrefab;	// Ignores raycast
	GameObject tmpWall;
	public GameObject wallPrefab;		// Allows raycast
	GameObject wall;

	bool mooving;
	Vector3 move_start_cam_pos;
	Vector3 move_start_wall_pos;
	GameObject moving_wall;

	public enum state {
		Idle,
		Create_Wall,
		Move_Wall,
		Move_Edge
	};
	public state curr_state;


	// Initialization
	void Start()
	{
		camScript = mainCamera.GetComponent<CameraScript>();
		creating = false;
		mooving = false;
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
			if (creating)
				adjust();
		}
	}

	void setStart()
	{
		creating = true;

		// Set start
		start.transform.position = camScript.worldPoint;
		start.transform.position += new Vector3(0, tmpWallPrefab.transform.localScale.y/2, 0);

		// Create tmpWall
		tmpWall = (GameObject)Instantiate(tmpWallPrefab, start.transform.position, Quaternion.identity);
	}

	void setEnd()
	{
		creating = false;

		// Set end
		end.transform.position = camScript.worldPoint;
		end.transform.position += new Vector3(0, tmpWallPrefab.transform.localScale.y/2, 0);

		// Replace tmpWall with wall (To restore raycast behavior)
		wall = (GameObject)Instantiate(wallPrefab, start.transform.position, Quaternion.identity);
		wall.transform.position =	tmpWall.transform.position;
		wall.transform.rotation =	tmpWall.transform.rotation; 
		wall.transform.localScale =	tmpWall.transform.localScale;
		Destroy (tmpWall);
	}

	void adjust()
	{
		// Set end
		end.transform.position = camScript.worldPoint;
		end.transform.position += new Vector3(0, tmpWallPrefab.transform.localScale.y/2, 0);

		// Adjust wall
		adjustWall();
	}

	void adjustWall()
	{
		// Make start & end face each other
		start.transform.LookAt(end.transform.position);
		end.transform.LookAt(start.transform.position);

		// Calculate distance between start & end
		float distance = Vector3.Distance(start.transform.position, end.transform.position);

		// Adjust tmpWall transform
		tmpWall.transform.position = start.transform.position + distance / 2 * start.transform.forward;
		tmpWall.transform.rotation = start.transform.rotation;
		tmpWall.transform.localScale = new Vector3(tmpWall.transform.localScale.x, tmpWall.transform.localScale.y, distance);
	}
	#endregion

	/// <summary>
	/// Move wall
	/// </summary>
	#region Move_Wall
	void Handle_Move_Wall(){
		// Start move
		if (Input.GetMouseButtonDown(0) && camScript.curr_game_object.tag == "Wall")
		{
			mooving = true;

			// Save start data
			moving_wall = camScript.curr_game_object;
			move_start_cam_pos = camScript.worldPoint;
			move_start_wall_pos = moving_wall.transform.position;

			// Replace wall with tmpWall (To ignore raycast while moving)
			tmpWall = (GameObject)Instantiate(tmpWallPrefab, start.transform.position, Quaternion.identity);
			tmpWall.transform.position =	moving_wall.transform.position;
			tmpWall.transform.rotation =	moving_wall.transform.rotation; 
			tmpWall.transform.localScale =	moving_wall.transform.localScale;
			Destroy (moving_wall);

		}

		// End move
		else if (Input.GetMouseButtonUp(0) && mooving)
		{
			mooving = false;

			// Replace tmpWall with wall (To restore raycast behavior)
			wall = (GameObject)Instantiate(wallPrefab, start.transform.position, Quaternion.identity);
			wall.transform.position =	tmpWall.transform.position;
			wall.transform.rotation =	tmpWall.transform.rotation; 
			wall.transform.localScale =	tmpWall.transform.localScale;
			Destroy (tmpWall);
		}

		// During move
		else
		{
			if (mooving) 
			{
				Vector3 delta = new Vector3(camScript.worldPoint.x - move_start_cam_pos.x,0,camScript.worldPoint.z - move_start_cam_pos.z);
				tmpWall.transform.position = move_start_wall_pos + delta;
			}
				
		}
	}
	#endregion

	/// <summary>
	/// Move Edge
	/// </summary>
	#region Move_Edge
	#endregion
}

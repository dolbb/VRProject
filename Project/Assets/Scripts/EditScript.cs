using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class EditScript : MonoBehaviour {
	// Cursor data
	public GameObject controllerData;
    controllerDataScript controllerDataScript;

    // Canvas data
    public GameObject menu_canvas;

    // Wall data
    public GameObject wallPrefab;
	GameObject wall;
	Vector3 startPosition;
	Vector3 endPosition;

	// Window data
	public GameObject windowPrefab;
	GameObject window;

    // Menu state
    public enum Mode
    {
        Idle,
        Create_Wall,
        Move,
        Add_Window
    };
    public Mode curr_mode;

    // Internal state
    public enum state
    {
		Idle,
		Create_Wall,
		Move_Wall,
		Move_Edge,
		Add_Window
	};
	public state curr_state;    // Todo: not public

    // Flag (has operation begun?)
    public bool operating;      // Todo: not public

	// Move data
	Vector3 move_start_cam_pos;
	Vector3 move_start_wall_pos;

	// Initialization
	void Start()
	{
        controllerDataScript = controllerData.GetComponent<controllerDataScript>();
		operating = false;
	}

    public void Set_Mode_Move()
    {
        curr_mode = Mode.Move;
    }

    public void Set_Mode_Add_Wall()
    {
        curr_mode = Mode.Create_Wall;
    }

    public void Set_Mode_Add_Window()
    {
        curr_mode = Mode.Add_Window;
    }

    // Update is called once per frame
    void Update()
	{
        if (menu_canvas.activeSelf)
        {
            curr_state = state.Idle;
        }

        // Translate Mode and pointed object to state
        if (!menu_canvas.activeSelf && !operating)
        {
            switch (curr_mode)
            {
                case Mode.Idle:
                    curr_state = state.Idle;
                    break;
                case Mode.Create_Wall:
                    curr_state = state.Create_Wall;
                    break;
                case Mode.Move:
                    if (controllerDataScript.curr_game_object.tag == "Middle")
                        curr_state = state.Move_Wall;
                    else
                        curr_state = state.Move_Edge;
                    break;
                case Mode.Add_Window:
                    curr_state = state.Add_Window;
                    break;
            }
        }

        // Handle current state
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
			//Handle_Add_Window ();
			break;
		}			
	}

	/// <summary>
	/// Create wall
	/// </summary>
	#region Create_Wall
	void Handle_Create_Wall()
	{
		if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
		{
            create_wall();
        }
		else if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger))
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

        // Attach to Wall_Edge
        if (controllerDataScript.curr_game_object.tag == "Wall_Edge")
        {
            // Get attached wall params
            GameObject wall = controllerDataScript.curr_game_object.transform.parent.gameObject;
            GameObject start = wall.transform.Find("Start").gameObject;
            GameObject end = wall.transform.Find("End").gameObject;

            // Calcualte 
            Vector3 direction = end.transform.position - start.transform.position;
            float wall_width = wall.transform.localScale.x;

            startPosition = controllerDataScript.curr_game_object.transform.position;

            // Adjust startpos position
            if (controllerDataScript.curr_game_object.name == "Start")
            {
                startPosition += direction.normalized * wall_width / 2;
            }
            else
            {
                startPosition -= direction.normalized * wall_width / 2;
            }
        }

        // No attachment
        else
            startPosition = controllerDataScript.worldPoint;

        // Adjust startPosition height
        startPosition.y = wallPrefab.transform.Find("Middle").localScale.y / 2;

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
		endPosition = new Vector3(controllerDataScript.worldPoint.x, wallPrefab.transform.Find("Middle").localScale.y/2, controllerDataScript.worldPoint.z);

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
		if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger) && controllerDataScript.curr_game_object.tag == "Middle")
		{
			operating = true;

			// Save start data
			wall = controllerDataScript.curr_game_object.transform.parent.gameObject;
			move_start_cam_pos = controllerDataScript.worldPoint;
			move_start_wall_pos = wall.transform.position;

			// Ignore raycast while moving
			wall.transform.Find("Start").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			wall.transform.Find("Middle").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			wall.transform.Find("End").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}

		// End
		else if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger) && operating)
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
				Vector3 delta = new Vector3(controllerDataScript.worldPoint.x - move_start_cam_pos.x,0,controllerDataScript.worldPoint.z - move_start_cam_pos.z);
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
		if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger) && controllerDataScript.curr_game_object.tag == "Wall_Edge")
		{
			operating = true;

			// Save start data
			wall = controllerDataScript.curr_game_object.transform.parent.gameObject;

			// Save start data
			GameObject edge = controllerDataScript.curr_game_object;
			wall = controllerDataScript.curr_game_object.transform.parent.gameObject;
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
		else if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger) && operating)
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
		if (controllerDataScript.curr_game_object.tag == "Middle") {

			// Create window
			if (!window) {
				window = (GameObject)Instantiate(windowPrefab);
			}

			// Get middle
			GameObject middle = controllerDataScript.curr_game_object;
            GameObject wall = controllerDataScript.curr_game_object.transform.parent.gameObject;
            GameObject start = wall.transform.Find("Start").gameObject;

            // Get line
            Vector3 lineVec = controllerDataScript.direction;
            Vector3 linePoint = controllerDataScript.worldPoint;

            // Get plane
            Vector3 planeNormal = controllerDataScript.normal;
            Vector3 planePoint = start.transform.position;

            // Adjust window transform
            Vector3 intersection = controllerDataScript.worldPoint;
            LinePlaneIntersection(out intersection, linePoint, lineVec, planeNormal, planePoint);
            window.transform.position = /*controllerDataScript.worldPoint;//*/ intersection;
            window.transform.position = controllerDataScript.worldPoint - planeNormal.normalized * wall.transform.localScale.x / 2;
            window.transform.rotation = middle.transform.rotation;

			// Attach to wall
			if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
			{
				// Restore raycast behavior
				window.layer = LayerMask.NameToLayer("Default");

				// Save in "Windows" field
				window.transform.parent = middle.transform.parent.transform.Find("Windows").transform;

				// The window
				window = null;
			}

            if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Pad))
            {
                // Edge case: menu was activated while in "Add_Window" mode
                if (window)
                    Destroy(window);
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

    #region Helpers

    //create a vector of direction "vector" with length "size"
    public static Vector3 SetVectorLength(Vector3 vector, float size)
    {

        //normalize the vector
        Vector3 vectorNormalized = Vector3.Normalize(vector);

        //scale the vector
        return vectorNormalized *= size;
    }


    //Get the intersection between a line and a plane. 
    //If the line and plane are not parallel, the function outputs true, otherwise false.
    public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {

        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;
        intersection = Vector3.zero;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineVec, planeNormal);

        //line and plane are not parallel
        if (dotDenominator != 0.0f)
        {
            length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point
            vector = SetVectorLength(lineVec, length);

            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;

            return true;
        }

        //output not valid
        else
        {
            return false;
        }
    }

    #endregion

}

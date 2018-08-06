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
	//GameObject wall;

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
    struct MoveData
    {
        public GameObject Wall;
        public GameObject Start;
        public GameObject End;
        public Vector3 startPos;
    }
    MoveData moveData;
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

        // Create new moveData
        moveData = new MoveData();

        // Attach to Wall_Edge
        if (controllerDataScript.curr_game_object.tag == "Wall_Edge")
        {
            moveData.startPos = controllerDataScript.curr_game_object.transform.position;
        }

        // Attach to ground
        else
        {
            moveData.startPos = controllerDataScript.worldPoint;

            // Adjust height
            moveData.startPos.y = wallPrefab.transform.localScale.y / 2;
        }

        // Create wall
        moveData.Wall = (GameObject)Instantiate(wallPrefab, moveData.startPos, Quaternion.identity);
        moveData.Wall.transform.SetParent(GameObject.Find("Model").transform);

        // Update moveData
        moveData.Start = moveData.Wall.transform.Find("Start").gameObject;
        moveData.End = moveData.Wall.transform.Find("End").gameObject;
    }

	void setWall()
	{
		operating = false;

		// Get Middle
		GameObject middle = moveData.Wall.transform.Find("Middle").gameObject;

        // Restore raycast behavior
        moveData.Start.layer = LayerMask.NameToLayer("Default");
		middle.layer = LayerMask.NameToLayer("Default");
        moveData.End.layer = LayerMask.NameToLayer("Default");
    }

    /// <summary>
    /// Adjusts wall according to start & end
    /// </summary>
    void adjust()
	{
        // Get wall params
        GameObject middle = moveData.Wall.transform.Find("Middle").gameObject;
        GameObject wall_Mesh = moveData.Wall.transform.Find("Wall_Mesh").gameObject;

        // Get start & end
        moveData.Start.transform.position = moveData.startPos;
        moveData.End.transform.position = new Vector3(controllerDataScript.worldPoint.x, wallPrefab.transform.localScale.y / 2, controllerDataScript.worldPoint.z);

        // Attach to Wall_Edge
        if (controllerDataScript.curr_game_object.tag == "Wall_Edge")
        {
            moveData.End.transform.position = controllerDataScript.curr_game_object.transform.position;
        }

        // Calcualte
        Vector3 direction = moveData.End.transform.position - moveData.Start.transform.position;
        float middle_length = Vector3.Distance(moveData.End.transform.position, moveData.Start.transform.position) - moveData.Wall.transform.localScale.x;
        float wall_mesh_length = Vector3.Distance(moveData.End.transform.position, moveData.Start.transform.position) + moveData.Wall.transform.localScale.x;

        // middle & wall_mesh
        middle.transform.position = wall_Mesh.transform.position = moveData.Start.transform.position + (direction / 2);
        middle.transform.localScale = new Vector3(middle.transform.localScale.x, middle.transform.localScale.y, middle_length);
        wall_Mesh.transform.localScale = new Vector3(middle.transform.localScale.x, middle.transform.localScale.y, wall_mesh_length);

        // wall
        if (moveData.Start.name == "End")
            moveData.Wall.transform.rotation = Quaternion.LookRotation(-direction);
        else
            moveData.Wall.transform.rotation = Quaternion.LookRotation(direction);
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

            // Create new moveData
            moveData = new MoveData();

            // Save start data
            moveData.Wall = controllerDataScript.curr_game_object.transform.parent.gameObject;
			move_start_cam_pos = controllerDataScript.worldPoint;
			move_start_wall_pos = moveData.Wall.transform.position;

            // Ignore raycast while moving
            moveData.Wall.transform.Find("Start").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            moveData.Wall.transform.Find("Middle").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            moveData.Wall.transform.Find("End").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}

		// End
		else if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger) && operating)
		{
			operating = false;

			// Restore raycast behavior
			moveData.Wall.transform.Find("Start").gameObject.layer = LayerMask.NameToLayer("Default");
			moveData.Wall.transform.Find("Middle").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.Wall.transform.Find("End").gameObject.layer = LayerMask.NameToLayer("Default");

            // Override global variable
            moveData.Wall = null;
		}

		// During
		else
		{
			if (operating) 
			{
				Vector3 delta = new Vector3(controllerDataScript.worldPoint.x - move_start_cam_pos.x,0,controllerDataScript.worldPoint.z - move_start_cam_pos.z);
                moveData.Wall.transform.position = move_start_wall_pos + delta;
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
            moveData.Wall = controllerDataScript.curr_game_object.transform.parent.gameObject;

			// Save start data
			GameObject edge = controllerDataScript.curr_game_object;
            moveData.Wall = controllerDataScript.curr_game_object.transform.parent.gameObject;
			GameObject start = moveData.Wall.transform.Find("Start").gameObject;
			GameObject middle = moveData.Wall.transform.Find("Middle").gameObject;
			GameObject end = moveData.Wall.transform.Find("End").gameObject;

            // Ignore raycast while moving
            start.layer = LayerMask.NameToLayer("Ignore Raycast");
            middle.layer = LayerMask.NameToLayer("Ignore Raycast");
            end.layer = LayerMask.NameToLayer("Ignore Raycast");

            // If looking at "start"
            if (edge.name == "Start")
            {
                moveData.Start = end;
                moveData.End = start;
                moveData.startPos = end.transform.position;
            }

            else
            {
                moveData.Start = start;
                moveData.End = end;
                moveData.startPos = start.transform.position;
            }
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

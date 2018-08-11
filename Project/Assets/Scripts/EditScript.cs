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

    // Floor_Plan data
    public GameObject panelTop;
    public GameObject panelDown;
    public GameObject panelLeft;
    public GameObject panelRight;
    List<Vector2> wallCoordinates;
    public GameObject coordinatePrefab;
    List<GameObject> wallsInFloorPlan;
    bool updatingFloorPlan;

    // Menu state
    public enum Mode
    {
        Idle,
        Create_Wall,
        Move,
        Add_Window,
        Add_Door,
        Delete,
        Get_Floor_Plan,
        Add_Floor_Plan
    };
    public Mode curr_mode;

    // Internal state
    public enum state
    {
		Idle,
		Create_Wall,
		Move_Wall,
		Move_Edge,
        Move_Window,
        Move_Window_Edge,
        Add_Window,
        Add_Door,
        Delete,
        Get_Floor_Plan,
        Add_Floor_Plan
    };
	public state curr_state;    // Todo: not public

    // Flag (has operation begun?)
    public bool operating;      // Todo: not public

    // Move data
    struct MoveData
    {
        public Vector3 startPos;

        // Wall
        public GameObject Wall;
        public GameObject Start;
        public GameObject End;

        // Window
        public GameObject window;
        public int frame;
    }
    MoveData moveData;
    Vector3 move_start_cam_pos;
	Vector3 move_start_wall_pos;

    // Move window data
    struct MoveWindowData
    {
        public GameObject Wall;
        public GameObject window;
        public Vector3 startPos;

    }
    MoveData moveWindowData;

    // Initialization
    void Start()
	{
        controllerDataScript = controllerData.GetComponent<controllerDataScript>();
        wallCoordinates = new List<Vector2>();
        updatingFloorPlan = false;
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

    public void Set_Mode_Add_Door()
    {
        curr_mode = Mode.Add_Door;
    }

    public void Set_Mode_Delete()
    {
        curr_mode = Mode.Delete;
    }

    public void Set_Mode_Get_Floor_Plan()
    {
        wallCoordinates = new List<Vector2>();
        wallsInFloorPlan = new List<GameObject>();
        curr_mode = Mode.Get_Floor_Plan;
        curr_state = state.Get_Floor_Plan;
    }

    public void Set_Mode_Add_Floor_Plan()
    {
        curr_mode = Mode.Add_Floor_Plan;
    }

    // Update is called once per frame
    void Update()
	{
        if (updatingFloorPlan)
            update_Floor_Plan();

        if (menu_canvas.activeSelf && curr_mode!=Mode.Get_Floor_Plan)
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
                    else if (controllerDataScript.curr_game_object.tag == "Window_Middle")
                        curr_state = state.Move_Window;
                    else if (controllerDataScript.curr_game_object.tag == "Window_Edge")
                        curr_state = state.Move_Window_Edge;
                    else 
                        curr_state = state.Move_Edge;
                    break;
                case Mode.Add_Window:
                    curr_state = state.Add_Window;
                    break;
                case Mode.Add_Door:
                    curr_state = state.Add_Door;
                    break;
                case Mode.Delete:
                    curr_state = state.Delete;
                    break;
                case Mode.Get_Floor_Plan:
                    curr_state = state.Get_Floor_Plan;
                    break;
                case Mode.Add_Floor_Plan:
                    curr_state = state.Add_Floor_Plan;
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
        case state.Move_Window:
            Handle_Move_Window();
            break;
        case state.Move_Window_Edge:
            Handle_Move_Window_Edge();
            break;
            case state.Move_Edge:
			Handle_Move_Edge ();
			break;
		case state.Add_Window:
			//Handle_Add_Window ();
			break;
        case state.Add_Door:
            //Handle_Add_Door ();
            break;
        case state.Delete:
            Handle_Delete();
            break;
        case state.Get_Floor_Plan:
            Handle_Get_Floor_Plan();
            break;
            case state.Add_Floor_Plan:
            Handle_Add_Floor_Plan();
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
    void adjust(bool adjustHeight = true)
	{
        // Get wall params
        GameObject middle = moveData.Wall.transform.Find("Middle").gameObject;
        GameObject wall_Mesh = moveData.Wall.transform.Find("Wall_Mesh").gameObject;

        // Calculate endPos
        Vector3 pointedEndPos;
        if (adjustHeight)
            pointedEndPos = new Vector3(controllerDataScript.worldPoint.x, wallPrefab.transform.localScale.y / 2, controllerDataScript.worldPoint.z);
        else
            pointedEndPos = controllerDataScript.worldPoint;
        Vector3 endPos;

        float distX = Mathf.Abs(moveData.startPos.x - pointedEndPos.x);
        float distZ = Mathf.Abs(moveData.startPos.z - pointedEndPos.z);

        // Attach endPos to x or z axis
        if (distX <= distZ)
        {
            endPos = new Vector3(moveData.startPos.x, pointedEndPos.y, pointedEndPos.z);
        }
        else
        {
            endPos = new Vector3(pointedEndPos.x, pointedEndPos.y, moveData.startPos.z);
        }

        // Get start & end
        moveData.Start.transform.position = moveData.startPos;
        moveData.End.transform.position = endPos;

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
        Vector3 oldMiddlePosition = middle.transform.position;
        middle.transform.position = wall_Mesh.transform.position = moveData.Start.transform.position + (direction / 2);
        middle.transform.localScale = new Vector3(middle.transform.localScale.x, middle.transform.localScale.y, middle_length);
        wall_Mesh.transform.localScale = new Vector3(wallPrefab.transform.Find("Wall_Mesh").localScale.x, middle.transform.localScale.y, wall_mesh_length);
        
        // Door
        Vector3 displacement = middle.transform.position - oldMiddlePosition;
        
        // Update doors
        foreach (Transform child in moveData.Wall.transform)
        {
            if (child.tag == "Door")
            {
                child.transform.position += displacement;
            }
        }

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

    /// <summary>
    /// Move wall
    /// </summary>
    #region Move_Window
    void Handle_Move_Window()
    {
        // Start
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger) && controllerDataScript.curr_game_object.tag == "Window_Middle")
        {
            operating = true;

            // Create new moveData
            moveData = new MoveData();

            // Save start data
            moveData.window = controllerDataScript.curr_game_object.transform.parent.gameObject;
            moveData.Wall = moveData.window.transform.parent.gameObject;
            move_start_cam_pos = moveData.window.transform.position;

            // Ignore raycast while moving
            moveData.window.transform.Find("Window_Middle").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            moveData.window.transform.Find("Frame_1").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            moveData.window.transform.Find("Frame_2").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            moveData.window.transform.Find("Frame_3").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            moveData.window.transform.Find("Frame_4").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        }

        // End
        else if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger) && operating)
        {
            operating = false;

            // Restore raycast behavior
            moveData.window.transform.Find("Window_Middle").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.window.transform.Find("Frame_1").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.window.transform.Find("Frame_2").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.window.transform.Find("Frame_3").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.window.transform.Find("Frame_4").gameObject.layer = LayerMask.NameToLayer("Default");

            // Override global variable
            moveData.window = null;
            moveData.Wall = null;
        }

        // During
        else
        {
            if (operating)
            {
                Vector3 delta;

                //Vector3 eulerAngles = moveData.Wall.transform.eulerAngles;
                float rotationY = moveData.Wall.transform.eulerAngles.y;

                if (rotationY == 90f || rotationY == 270f)
                {
                    delta = new Vector3(controllerDataScript.worldPoint.x - move_start_cam_pos.x, 0f, 0f);
                }

                else
                {
                    delta = new Vector3(0f, 0f, controllerDataScript.worldPoint.z - move_start_cam_pos.z);
                }

                // Looking at "Window" or "Door" ?
                if(moveData.window.tag == "Window")
                {
                    delta += new Vector3(0f, controllerDataScript.worldPoint.y - move_start_cam_pos.y, 0f);
                }

                moveData.window.transform.position = move_start_cam_pos + delta;
            }

        }
    }
    #endregion

    /// <summary>
    /// Move wall
    /// </summary>
    #region Move_Window_Edge
    void Handle_Move_Window_Edge()
    {
        // Start
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger) && controllerDataScript.curr_game_object.tag == "Window_Edge")
        {
            operating = true;

            // Create new moveData
            moveData = new MoveData();

            // Save start data
            moveData.window = controllerDataScript.curr_game_object.transform.parent.gameObject;
            moveData.Wall = moveData.window.transform.parent.gameObject;
            move_start_cam_pos = moveData.window.transform.position;

            // Ignore raycast while moving
            //moveData.window.transform.Find("Window_Middle").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            //moveData.window.transform.Find("Frame_1").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            //moveData.window.transform.Find("Frame_2").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            //moveData.window.transform.Find("Frame_3").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            //moveData.window.transform.Find("Frame_4").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            switch (controllerDataScript.curr_game_object.name)
            {
                case "Frame_1":
                    moveData.frame = 1;
                    break;
                case "Frame_2":
                    moveData.frame = 2;
                    break;
                case "Frame_3":
                    moveData.frame = 3;
                    break;
                case "Frame_4":
                    moveData.frame = 4;
                    break;
            }
            
        }

        // End
        else if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger) && operating)
        {
            operating = false;

            // Restore raycast behavior
            moveData.window.transform.Find("Window_Middle").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.window.transform.Find("Frame_1").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.window.transform.Find("Frame_2").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.window.transform.Find("Frame_3").gameObject.layer = LayerMask.NameToLayer("Default");
            moveData.window.transform.Find("Frame_4").gameObject.layer = LayerMask.NameToLayer("Default");

            // Override global variable
            moveData.window = null;
            moveData.Wall = null;
        }

        // During
        else
        {
            if (operating)
            {
                Vector3 delta;

                //Vector3 eulerAngles = moveData.Wall.transform.eulerAngles;
                float rotationY = moveData.Wall.transform.eulerAngles.y;
                float distance;

                // Wall aligned to x axis
                if (rotationY == 90f || rotationY == 270f)
                {
                    if (moveData.frame == 1 || moveData.frame == 3)
                    {
                        float sizeY = Mathf.Abs(2 * (controllerDataScript.worldPoint.y - move_start_cam_pos.y));
                        moveData.window.transform.localScale = new Vector3(moveData.window.transform.localScale.x, sizeY/3, moveData.window.transform.localScale.z);
                    }

                    else if (moveData.frame == 2 || moveData.frame == 4)
                    {
                        float sizeX = Mathf.Abs(2 * (controllerDataScript.worldPoint.x - move_start_cam_pos.x));
                        moveData.window.transform.localScale = new Vector3(moveData.window.transform.localScale.x, moveData.window.transform.localScale.y, sizeX/4);
                    }
                }

                // Wall aligned to z axis
                else
                {
                    if (moveData.frame == 1 || moveData.frame == 3)
                    {
                        float sizeY = Mathf.Abs(2 * (controllerDataScript.worldPoint.y - move_start_cam_pos.y));
                        moveData.window.transform.localScale = new Vector3(moveData.window.transform.localScale.x, sizeY/3, moveData.window.transform.localScale.z);
                    }

                    else if (moveData.frame == 2 || moveData.frame == 4)
                    {
                        float sizeZ = Mathf.Abs(2 * (controllerDataScript.worldPoint.z - move_start_cam_pos.z));
                        moveData.window.transform.localScale = new Vector3(moveData.window.transform.localScale.x, moveData.window.transform.localScale.y, sizeZ/4);
                    }
                }
            }

        }
    }
    #endregion

    /// <summary>
    /// Move wall
    /// </summary>
    #region Move_Window_Edge
    void Handle_Delete()
    {
        // Start
        if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger))
        {
            string tag = controllerDataScript.curr_game_object.tag;
            
            // Wall
            if (tag == "Middle" || tag == "Wall_Edge" || controllerDataScript.curr_game_object.name == "Wall_Mesh")
            {
                Destroy(controllerDataScript.curr_game_object.transform.parent.gameObject);
            }

            // Window
            if (tag == "Window_Middle" || tag == "Window_Edge" || controllerDataScript.curr_game_object.name == "Window_Mesh")
            {
                Destroy(controllerDataScript.curr_game_object.transform.parent.gameObject);
            }

        }
    }

    #endregion

    /// <summary>
    /// Move wall
    /// </summary>
    #region Get_Floor_Plan
    void Handle_Get_Floor_Plan()
    {
        // If selected a new coordinate
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            float x,z;

            // Get midPoint
            var xDirection = panelRight.transform.position - panelLeft.transform.position;
            var midPoint = panelLeft.transform.position + xDirection * 0.5f;

            // Get xModule
            Vector3 NearestPointOnX = NearestPointOnLine(panelLeft.transform.position, xDirection, controllerDataScript.worldPoint);
            float xModule = Vector3.Distance(NearestPointOnX, midPoint) / Vector3.Distance(panelLeft.transform.position, panelRight.transform.position);

            // Get X coordinate
            if (Vector3.Distance(NearestPointOnX, panelLeft.transform.position) < Vector3.Distance(NearestPointOnX, panelRight.transform.position))
                x = -xModule;
            else
                x = xModule;

            // Get zModule
            Vector3 NearestPointOnZ = NearestPointOnLine(panelDown.transform.position, panelDown.transform.position - panelTop.transform.position, controllerDataScript.worldPoint);
            float zModule = Vector3.Distance(NearestPointOnZ, midPoint) / Vector3.Distance(panelDown.transform.position, panelTop.transform.position); ;

            // Get Z coordinate
            if (Vector3.Distance(NearestPointOnZ, panelDown.transform.position) < Vector3.Distance(NearestPointOnZ, panelTop.transform.position))
                z = -zModule;
            else
                z = zModule;

            // Store (x,z)
            wallCoordinates.Add(new Vector2(x,z));

            // Create a small coordinate
            GameObject coordinate = (GameObject)Instantiate(coordinatePrefab, controllerDataScript.worldPoint, Quaternion.identity);
            coordinate.transform.SetParent(GameObject.Find("Coordinates").transform);
            coordinate.transform.localScale = new Vector3(8f, 8f, 8f);
        }
    }
    #endregion

    /// <summary>
    /// Add_Floor_Plan
    /// </summary>
    #region Add_Floor_Plan
    void Handle_Add_Floor_Plan()
    {
        // Create container for created walls
        wallsInFloorPlan = new List<GameObject>();

        // Create walls
        Vector2 startCoordinate = new Vector2(8 * wallCoordinates[0].x, 8 * wallCoordinates[0].y);
        Vector2 endCoordinate;

        for (int i = 1; i < wallCoordinates.Count-1; i++)
        {
            // Get endCoordinate
            endCoordinate = new Vector2(8 * wallCoordinates[i].x, 8 * wallCoordinates[i].y);

            // Calculate startPos & endPos
            Vector3 startPos = new Vector3(startCoordinate.x, -wallPrefab.transform.localScale.y / 2, startCoordinate.y);
            Vector3 endPos = new Vector3(endCoordinate.x, -wallPrefab.transform.localScale.y / 2, endCoordinate.y);

            float distX = Mathf.Abs(startPos.x - endPos.x);
            float distZ = Mathf.Abs(startPos.z - endPos.z);

            // Attach endPos to x or z axis
            if (distX <= distZ)
            {
                endPos = new Vector3(startPos.x, endPos.y, endPos.z);
            }
            else
            {
                endPos = new Vector3(endPos.x, endPos.y, startPos.z);
            }


            // Create wall
            GameObject Wall = (GameObject)Instantiate(wallPrefab, startPos, Quaternion.identity);
            Wall.transform.SetParent(GameObject.Find("Model").transform);
            wallsInFloorPlan.Add(Wall);

            // Get wall params
            GameObject Start = Wall.transform.Find("Start").gameObject;
            GameObject middle = Wall.transform.Find("Middle").gameObject;
            GameObject End = Wall.transform.Find("End").gameObject;
            GameObject wall_Mesh = Wall.transform.Find("Wall_Mesh").gameObject;

            // Calcualte
            Vector3 direction = endPos - startPos;
            float middle_length = direction.magnitude - Wall.transform.localScale.x;
            float wall_mesh_length = direction.magnitude + Wall.transform.localScale.x;

            // Get start & end
            Start.transform.localPosition = Vector3.zero;
            End.transform.localPosition = new Vector3 (0f, 0f, direction.magnitude);

            // middle & wall_mesh
            middle.transform.localPosition = wall_Mesh.transform.localPosition = new Vector3(0f, 0f, direction.magnitude / 2);
            middle.transform.localScale = new Vector3(middle.transform.localScale.x, middle.transform.localScale.y, middle_length);
            wall_Mesh.transform.localScale = new Vector3(wallPrefab.transform.Find("Wall_Mesh").localScale.x, middle.transform.localScale.y, wall_mesh_length);

            Wall.transform.rotation = Quaternion.LookRotation(direction);

            ///////////////////////

            startCoordinate = new Vector2 (endPos.x, endPos.z);

            // Restore raycast behavior
            Start.layer = LayerMask.NameToLayer("Default");
            middle.layer = LayerMask.NameToLayer("Default");
            End.layer = LayerMask.NameToLayer("Default");
        }

        wallCoordinates.Clear();
        curr_mode = Mode.Idle;
        updatingFloorPlan = true;
    }

    void update_Floor_Plan()
    {
        // Iterate over wallsInFloorPlan
        foreach (GameObject wall in wallsInFloorPlan)
        {
            // Check height
            if (wall.transform.position.y < wallPrefab.transform.localScale.y / 2)
            {
                wall.transform.position = new Vector3(wall.transform.position.x, wall.transform.position.y + 0.01f, wall.transform.position.z);
            }

            else if (wall.transform.position.y > wallPrefab.transform.localScale.y / 2)
            {
                wall.transform.position = new Vector3(wall.transform.position.x, wallPrefab.transform.localScale.y / 2, wall.transform.position.z);
                updatingFloorPlan = false;
            }
        }
    }

    #endregion

    /// <summary>
    /// Helpers
    /// </summary>
    #region Helpers

    //linePnt - point the line passes through
    //lineDir - unit vector in direction of line, either direction works
    //pnt - the point to find nearest on line for
    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        Vector3 v = pnt - linePnt;
        float d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

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

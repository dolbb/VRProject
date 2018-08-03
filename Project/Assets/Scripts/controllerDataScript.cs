using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class controllerDataScript : MonoBehaviour
{

    // Output: Data of current pointer
    public Vector3 worldPoint;
    public GameObject curr_game_object;
    public float distance;
    public Vector3 direction;
    public Vector3 normal;

    // Input: VRTK pointer
    VRTK.VRTK_Pointer rightPointer;

    // Use this for initialization
    void Start()
    {
        // Find pointer
        rightPointer = GameObject.Find("RightControllerScriptAlias").GetComponent<VRTK.VRTK_Pointer>();
    }

    // Update is called once per frame
    void Update()
    {
        updateWorldPoint();
    }

    void updateWorldPoint()
    {     
        // Get rightPointer hit
        RaycastHit hit = rightPointer.pointerRenderer.GetDestinationHit(); ;

        // Update rightPointer data
        worldPoint = hit.point;
        if (hit.collider)
        {
            curr_game_object = hit.collider.gameObject;
            distance = hit.distance;

           
            direction = new Vector3(hit.transform.rotation.x, hit.transform.rotation.y, hit.transform.rotation.z);
            normal = hit.normal;
        }

        //rightPointer.pointerRenderer.transform.rotation;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {

    public float speed;
    private Rigidbody rb;

    // Use this for initialization
	void Start () {
        speed = 5;
        rb = GetComponent<Rigidbody>();
    }

    
    // FixedUpdate is called before calculations
    void FixedUpdate()
    {
        float moveHorizontal    = Input.GetAxis("Horizontal");
        float moveVertical      = Input.GetAxis("Vertical");
        Vector3 movment = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movment*speed);
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{

    public Rigidbody2D rb;
    public int accel;
    public float dragCoefficient;
    float rollingCoefficient;
    public float rotateRate;
    public Transform trans;


    // Start is called before the first frame update
    void Start()
    {
        rollingCoefficient = dragCoefficient * 30;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        
        float angle = Mathf.Rad2Deg * Mathf.Atan(rb.velocity.y/rb.velocity.x);

        Debug.Log("Velocity: " + angle + " and Direction: " + (rb.rotation));

        float translation = Input.GetAxis("Vertical");
        float rotation = -1 * Input.GetAxis("Horizontal") * rb.velocity.magnitude;
        float drag = dragCoefficient * rb.velocity.magnitude * rb.velocity.magnitude;
        float rolling = rollingCoefficient * rb.velocity.magnitude;

        

        rb.AddForce(trans.up * translation * accel);
        rb.AddForce(drag * rb.velocity.normalized * -1);
        rb.AddForce(rolling * rb.velocity.normalized * -1);
        trans.Rotate(new Vector3(0,0,rotation * rotateRate));

    }
}

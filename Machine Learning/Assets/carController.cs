using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class carController : Agent
{
    public Rigidbody2D rb;
    public int accel;
    public float dragCoefficient;
    float rollingCoefficient;
    public float rotateRate;
    public Transform trans;
    public int steps;
    float timeElapsed;
    public GameObject Romano;


    public int rayCount;
    public int angleInc;

    GameObject[] Romanos;


    // Start is called before the first frame update
    void Start()
    {
        rollingCoefficient = dragCoefficient * 30;
        trans.position = new Vector3 (1.8f, Random.Range(2.8f, 4.2f), 0);
        rb = GetComponent<Rigidbody2D>();
        angleInc = 360 / rayCount;
        Romanos = new GameObject[rayCount];
        SpriteRenderer sr;
         for (int i = 0; i < rayCount; i++)
         {
            Romanos[i] = Instantiate(Romano, trans.position, Quaternion.Euler(trans.TransformDirection(new Vector3(Mathf.Cos(i*angleInc), Mathf.Sin(i*angleInc)))));
            sr = Romanos[i].gameObject.GetComponent<SpriteRenderer>();
            sr.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
         }
    }

    public override void OnEpisodeBegin()
    {
        trans.eulerAngles = new Vector3(0, 0, -90);
        rb.velocity = new Vector2(0, 0);
        trans.position = new Vector3(1.8f, Random.Range(2.8f, 4.2f), 0);
        steps = 0;
        timeElapsed = 0;
        // create/create game object for the square
        // spawn at rb.position 
        // spriterenderer enabled is false
    }


    // Update is called once per frame
    public override void OnActionReceived(float[] vectorAction)
    {
        //Debug.Log(vectorAction[1]);
        steps += 1;
        timeElapsed += Time.deltaTime;
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        //Debug.Log("Movin it: " + vectorAction[0] + ", " + vectorAction[1]);
        var forward = Mathf.RoundToInt(vectorAction[0]);
        var turn = Mathf.RoundToInt(vectorAction[1]);
        //Debug.Log(forward + " and " + turn);
        float horizontal = 0f;
        float vertical = 0f;
        float breaking = 0f;

        switch (forward)
        {
            case 1:
                vertical = 1f;
                AddReward(1f / 100f * rb.velocity.magnitude);
                break;
            case 2:
                breaking = 1f;
                break;
        }
        switch (turn)
        {
            case 1:
                horizontal = 1f;
                break;
            case 2:
                horizontal = -1f;
                break;
        }


        float translation = vertical;
        float rotation = -1 * horizontal * rb.velocity.magnitude;
        float drag = dragCoefficient * rb.velocity.magnitude * rb.velocity.magnitude;
        float breakForce = breaking * rb.velocity.magnitude * rb.velocity.magnitude;
        float rolling = rollingCoefficient * rb.velocity.magnitude;

        

        rb.AddForce(trans.up * translation * accel);
        rb.AddForce(drag * rb.velocity.normalized * -1);
        rb.AddForce(rolling * rb.velocity.normalized * -1);
        rb.AddForce(breakForce * rb.velocity.normalized * -1);
        trans.Rotate(new Vector3(0,0,rotation * rotateRate));
        
        //if (trans.position.z < 0 || steps > 2000)
        if (trans.position.z < 0)
        {
            EndEpisode();
        }
        AddReward(1f / 200f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other);
        if (other.tag == "Walls")
        {
            
            EndEpisode();
        }
        else if (other.tag=="Checkpoint")
        {
            AddReward(2f);
        }

    }

    public override float[] Heuristic()
    {

        

        //RaycastHit2D hit;
        //hit = Physics2D.Raycast(trans.position, trans.TransformDirection(Vector3.up));
        //Debug.Log("Creebo: " + hit.collider + ", " + hit.distance + ", " + hit.point);
        //Debug.Log(trans.TransformDirection(Vector3.up));

        // sprite renderer enabled 

        // Square dimensions and direction 

        // For loop ends here

        var action = new float[2];
        float verticle = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        switch (verticle)
        {
            case -1:
                action[0] = 2;
                break;
            default:
                action[0] = verticle;
                break;
        }
        switch (horizontal)
        {
            case -1:
                action[1] = 2;
                break;
            default:
                action[1] = horizontal;
                break;
        }
        return action;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity);
        float observation;
        for (int i = 0; i < rayCount; i++)
        {
            Romanos[i].transform.rotation = trans.rotation;
            Romanos[i].transform.Rotate(0, 0, i * angleInc);
            Romanos[i].transform.position = trans.position;
            observation = Physics2D.Raycast(trans.position, Quaternion.Euler(0f,0f,trans.rotation.eulerAngles.z+ i*angleInc)*Vector3.up).distance;
            sensor.AddObservation(observation);
            
            Romanos[i].transform.localScale = new Vector3(0.1f,observation,0);
            Romanos[i].transform.position += Romanos[i].transform.rotation*Vector3.up*observation/2;
            //Debug.Log(i + ", " + Physics2D.Raycast(trans.position, trans.TransformDirection(new Vector3(Mathf.Cos(i), Mathf.Sin(i)))).distance);
        }
    }
}

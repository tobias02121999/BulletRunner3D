using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    // Initialize the public enums
    public enum States { STATIONARY, DRIVING }

    // Initialize the public variables
    public States state;
    public GameObject[] carModels;
    public GameObject player;
    public float interactionDistance;
    public Transform driverPosition, exitPosition;
    public Transform steeringWheel;
    public float maxThrust;
    public float acceleration;
    public float friction;
    public float wheelSensitivity;
    public float maxWheelAngle;
    public Transform forward;
    public float maxCarAngle;

    // Initialize the private variables
    float wheelRotation;
    float thrust;
    Rigidbody rb;
    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RunState(); // Run the current car state
    }

    // Run the current car state
    void RunState()
    {
        switch (state)
        {
            case States.STATIONARY:
                DisplayModel(0); // Display the correct car model
                CheckForPlayer(); // Check if the player enters or exits the car
                break;

            case States.DRIVING:
                DisplayModel(1); // Display the correct car model
                CheckForPlayer(); // Check if the player enters or exits the car
                Thrust(); // Thrust the car forwards
                Turn(); // Turn the car
                Friction(); // Slow the car down over time
                break;
        }
    }

    // Display the correct car model
    void DisplayModel(int modelID)
    {
        var length = carModels.Length;
        for (var i = 0; i < length; i++)
            carModels[i].SetActive((i == modelID));
    }

    // Check if the player enters the car
    void CheckForPlayer()
    {
        var playerPos = player.transform.position;
        var dist = Vector3.Distance(transform.position, playerPos);

        if (dist <= interactionDistance && Input.GetButtonDown("Interact"))
        {
            if (state == States.STATIONARY)
            {
                DisablePlayer(); // Disable the player and put them in the driver seat
                state = States.DRIVING;
            }
            else
            {
                EnablePlayer(); // Enable the player and put them outside of the car
                state = States.STATIONARY;
            }
        }
    }

    // Disable the player and put them in the driver seat
    void DisablePlayer()
    {
        var obj = player.GetComponent<Player>();
        obj.state = Player.States.DRIVING;
        obj.Freeze();

        player.transform.parent = driverPosition;
        player.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    // Enable the player and put them outside of the car
    void EnablePlayer()
    {
        var obj = player.GetComponent<Player>();
        obj.state = Player.States.DEFAULT;
        obj.Unfreeze();

        player.transform.parent = null;
        player.transform.position = exitPosition.position;
    }

    // Thrust the car forwards
    void Thrust()
    {
        var speed = Input.GetAxis("Vertical") * acceleration;
        thrust = Mathf.Clamp(thrust + speed, -maxThrust, maxThrust);

        if (isGrounded)
            forward.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        var newVelocity = forward.forward * thrust;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
    }

    // Turn the car
    void Turn()
    {
        if (isGrounded)
        {
            var speed = (Input.GetAxis("Horizontal") * wheelSensitivity) * (thrust / maxThrust);
            wheelRotation += Mathf.Clamp(speed, -maxWheelAngle, maxWheelAngle);
        }
        else
            wheelRotation = 0f;

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, wheelRotation, transform.eulerAngles.z);
    }

    // Slow the car down over time
    void Friction()
    {
        if (thrust >= friction)
            thrust -= friction;

        if (thrust <= -friction)
            thrust += friction;
    }

    // Check if the car is grounded
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.relativeVelocity.magnitude >= 5)
            thrust = 0f;
    }

    // Check if the car is not grounded
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}

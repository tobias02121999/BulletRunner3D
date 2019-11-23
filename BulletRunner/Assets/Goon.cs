using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goon : MonoBehaviour
{
    // Initialize the public enums
    public enum states { IDLE, CHASE, STRAFE, FLEE }

    // Initialize the public variables
    public states state;
    public GameObject sightRangeSphere, strafeRangeSphere;
    public GameObject player;
    public Vector2 wanderDuration, strafeDuration;
    public float wanderSpeed, strafeSpeed;
    public Transform reference;

    // Initialize the private variables
    float sightRange, strafeRange;
    int wanderAlarm, strafeAlarm;
    bool isWandering;
    int strafeDir;

    // Start is called before the first frame update
    void Start()
    {
        sightRange = sightRangeSphere.transform.localScale.x / 2f;
        strafeRange = strafeRangeSphere.transform.localScale.x / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        RunState(); // Run the current goon state
    }

    // Run the current goon state
    void RunState()
    {
        switch (state)
        {
            case states.IDLE:
                Scout(); // Scout the area for nearby enemies
                Wander(); // Randomly wander around the place
                break;

            case states.CHASE:
                Scout(); // Scout the area for nearby enemies
                Move(); // Move forward
                Aim(); // Aim at the player
                break;

            case states.STRAFE:
                Scout(); // Scout the area for nearby enemies
                Strafe(); // Move left and right
                Aim(); // Aim at the player
                break;

            case states.FLEE:
                break;
        }
    }

    // Scout the area for nearby enemies
    void Scout()
    {
        var playerPos = player.transform.position;
        var dist = Vector3.Distance(transform.position, playerPos);

        if (dist > sightRange)
            state = states.IDLE;

        if (dist <= sightRange && dist > strafeRange)
            state = states.CHASE;

        if (dist <= strafeRange)
            state = states.STRAFE;
    }

    // Randomly wander around the place
    void Wander()
    {
        wanderAlarm--;

        if (wanderAlarm <= 0f)
        {
            var rand = Mathf.Round(Random.Range(0f, 1f));
            if (rand == 0)
                isWandering = true;
            else
                isWandering = false;

            var newRot = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0f, newRot, 0f);

            wanderAlarm = Mathf.RoundToInt(Random.Range(wanderDuration.x, wanderDuration.y));
        }

        if (isWandering)
        {
            var step = wanderSpeed * Time.deltaTime;
            transform.position += transform.forward * step;
        }
    }

    // Move forward
    void Move()
    {
        var step = wanderSpeed * Time.deltaTime;
        transform.position += transform.forward * step;
    }

    // Aim at the player
    void Aim()
    {
        reference.LookAt(player.transform.position);
        transform.rotation = Quaternion.Euler(0f, reference.eulerAngles.y, 0f);
    }

    // Move left and right
    void Strafe()
    {
        strafeAlarm--;

        if (strafeAlarm <= 0f)
        {
            strafeDir = Mathf.RoundToInt(Random.Range(0f, 1f));
            strafeAlarm = Mathf.RoundToInt(Random.Range(strafeDuration.x, strafeDuration.y));
        }

        if (strafeDir == 0)
            transform.position += transform.right * strafeSpeed * Time.deltaTime;
        else
            transform.position -= transform.right * strafeSpeed * Time.deltaTime;
    }
}

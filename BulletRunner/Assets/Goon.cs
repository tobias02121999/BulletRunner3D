using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goon : Entity
{
    // Initialize the public enums
    public enum States { IDLE, CHASE, STRAFE, FLEE }

    // Initialize the public variables
    public States state;
    public GameObject sightRangeSphere, strafeRangeSphere, fleeRangeSphere;
    public GameObject player;
    public Vector2 wanderDuration, strafeDuration;
    public float wanderSpeed, chaseSpeed, strafeSpeed, fleeSpeed;
    public Transform reference;
    public Transform projectilePivot;
    public GameObject projectilePrefab;
    public Animator gunAnimator;
    public GameObject corpsePrefab;

    // Initialize the private variables
    float sightRange, strafeRange, fleeRange;
    int wanderAlarm, strafeAlarm, fireAlarm;
    public int fireRate;
    bool isWandering;
    int strafeDir;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Initialize the entity

        sightRange = sightRangeSphere.transform.localScale.x / 2f;
        strafeRange = strafeRangeSphere.transform.localScale.x / 2f;
        fleeRange = fleeRangeSphere.transform.localScale.x / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        RunState(); // Run the current goon state
        Die(); // Die
    }

    // Run the current goon state
    void RunState()
    {
        switch (state)
        {
            case States.IDLE:
                Scout(); // Scout the area for nearby enemies
                Wander(); // Randomly wander around the place
                break;

            case States.CHASE:
                Scout(); // Scout the area for nearby enemies
                Move(chaseSpeed); // Move forward
                Aim(); // Aim at the player
                Shoot(); // Shoot forward
                break;

            case States.STRAFE:
                Scout(); // Scout the area for nearby enemies
                Strafe(); // Move left and right
                Aim(); // Aim at the player
                Shoot(); // Shoot forward
                break;

            case States.FLEE:
                Scout(); // Scout the area for nearby enemies
                Move(-fleeSpeed); // Move forward
                Aim(); // Aim at the player
                Shoot(); // Shoot forward
                break;
        }
    }

    // Scout the area for nearby enemies
    void Scout()
    {
        var playerPos = player.transform.position;
        var dist = Vector3.Distance(transform.position, playerPos);

        if (dist > sightRange)
            state = States.IDLE;
        else
        {
            if (dist <= strafeRange)
            {
                if (dist <= fleeRange)
                    state = States.FLEE;
                else
                    state = States.STRAFE;
            }
            else
                state = States.CHASE;
        }
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
    void Move(float speed)
    {
        var step = speed * Time.deltaTime;
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

    // Shoot forward
    void Shoot()
    {
        fireAlarm--;

        if (fireAlarm <= 0)
        {
            var obj = Instantiate(projectilePrefab, projectilePivot);
            obj.transform.parent = null;

            gunAnimator.Play("GunShoot");

            fireAlarm = fireRate;
        }
    }

    // Die
    void Die()
    {
        if (health <= 0)
        {
            var obj = Instantiate(corpsePrefab, transform);
            obj.transform.parent = null;

            Destroy(this.gameObject);
        }
    }
}

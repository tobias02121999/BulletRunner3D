using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Initialize the public variables
    public float speed;

    // Initialize the private variables
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update is called once per fixed frame
    void FixedUpdate()
    {
        Move(); // Move the projectile forward
    }

    // Move the projectile forward
    void Move()
    {
        rb.velocity = transform.forward * speed * Time.deltaTime;
    }
}

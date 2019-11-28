using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Initialize the public variables
    public float speed;
    public bool isFriendly;
    public GameObject bloodPrefab;

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

    // Deal damage
    void OnTriggerEnter(Collider other)
    {
        // Deal damage
        var entity = other.gameObject.GetComponentInParent<Entity>();
        if (entity != null && entity.isFriendly != isFriendly)
        {
            var obj = Instantiate(bloodPrefab, transform);
            obj.transform.parent = null;

            entity.health--;
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Wall"))
            Destroy(this.gameObject);
    }
}

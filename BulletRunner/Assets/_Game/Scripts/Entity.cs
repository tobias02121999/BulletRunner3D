using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Initialize the public variables
    public int maxHealth;
    public bool isFriendly;

    [HideInInspector]
    public int health;

    // Initialize the entity
    public void Initialize()
    {
        health = maxHealth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    // Initialize the public enums
    public enum States { DEFAULT, DRIVING }

    // Initialize the public variables
    public States state;
    public float lookSensitivity;
    public float gunLerpRate;
    public float movementSpeed;
    public float leanIntensity;
    public Transform camTransform, projectilePivot;
    public GameObject gunPivot;
    public GameObject[] guns;
    public GameObject projectilePrefab;
    public Animator gunAnimator;

    // Initialize the private variables
    Vector2 rot;
    int weaponID;
    public int fireRate;
    int fireAlarm;
    Rigidbody rb;
    CapsuleCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Initialize the entity

        gunPivot.transform.parent = null;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        RunState(); // Run the current player state
    }

    // Run the current player state
    void RunState()
    {
        switch (state)
        {
            case States.DEFAULT:
                Look(new Vector2(-Mathf.Infinity, Mathf.Infinity), new Vector2(-90f, 90f)); // Rotate the player and the camera
                Move(); // Move the player around
                SwitchGun(); // Switch the current gun
                Shoot(); // Shoot with the current gun
                Lean(); // Lean the camera towards the current horizontal movement direction
                LockCursor(); // Lock the cursor in the center of the screen, and hide it
                break;

            case States.DRIVING:
                HideGun(); // Hide the players current gun
                Look(new Vector2(-35f, 45f), new Vector2(-15f, 7.5f)); // Rotate the player and the camera
                LockCursor(); // Lock the cursor in the center of the screen, and hide it
                break;
        }
    }

    // Rotate the player and the camera
    void Look(Vector2 clampHor, Vector2 clampVer)
    {
        var targetRotX = rot.x - Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
        var targetRotY = rot.y + Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;

        rot.x = Mathf.Clamp(targetRotX, clampVer.x, clampVer.y);
        rot.y = Mathf.Clamp(targetRotY, clampHor.x, clampHor.y);

        transform.localRotation = Quaternion.Euler(0f, rot.y, 0f);
        camTransform.rotation = Quaternion.Euler(rot.x, camTransform.eulerAngles.y, camTransform.eulerAngles.z);

        var targetRot = Quaternion.LookRotation(camTransform.forward);
        gunPivot.transform.rotation = Quaternion.Slerp(gunPivot.transform.rotation, targetRot, gunLerpRate);
    }

    // Move the player around
    void Move()
    {
        transform.position += transform.forward * Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        gunPivot.transform.position = camTransform.position;
    }

    // Lean the camera towards the current horizontal movement direction
    void Lean()
    {
        camTransform.rotation = Quaternion.Euler(camTransform.localEulerAngles.x, camTransform.eulerAngles.y, -Input.GetAxis("Horizontal") * leanIntensity);
    }

    // Switch the current gun
    void SwitchGun()
    {
        var maxID = guns.Length - 1;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (weaponID <= maxID - 1)
                weaponID++;
            else
                weaponID = 0;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (weaponID >= 1)
                weaponID--;
            else
                weaponID = maxID;
        }

        for (var i = 0; i <= maxID; i++)
            guns[i].SetActive(i == weaponID);
    }

    // Shoot with the current gun
    void Shoot()
    {
        fireAlarm--;

        if (Input.GetButtonDown("Fire1") && fireAlarm <= 0)
        {
            var obj = Instantiate(projectilePrefab, projectilePivot);
            obj.transform.parent = null;
            obj.GetComponent<Projectile>().isFriendly = true;

            gunAnimator.Play("GunShoot");

            fireAlarm = fireRate;
        }
    }

    // Lock the cursor in the center of the screen, and hide it
    void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Freeze the player in its current position
    public void Freeze()
    {
        rb.isKinematic = true;
        collider.enabled = false;
    }

    // Unfreeze the player
    public void Unfreeze()
    {
        rb.isKinematic = false;
        collider.enabled = true;
    }

    // Hide the players current gun
    void HideGun()
    {
        var length = guns.Length;
        for (var i = 0; i < length; i++)
            guns[i].SetActive(false);
    }
}

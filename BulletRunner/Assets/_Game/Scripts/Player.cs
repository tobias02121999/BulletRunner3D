using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Initialize the public variables
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

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Initialize the player
    }

    // Update is called once per frame
    void Update()
    {
        Look(); // Rotate the player and the camera
        Move(); // Move the player around
        SwitchGun(); // Switch the current gun
        Shoot(); // Shoot with the current gun
        Lean(); // Lean the camera towards the current horizontal movement direction
        LockCursor(); // Lock the cursor in the center of the screen, and hide it
    }

    // Initialize the player
    void Initialize()
    {
        gunPivot.transform.parent = null;
    }

    // Rotate the player and the camera
    void Look()
    {
        rot.x -= Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
        rot.y += Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
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
        {
            guns[i].SetActive(i == weaponID);
        }
    }

    // Shoot with the current gun
    void Shoot()
    {
        fireAlarm--;

        if (Input.GetButtonDown("Fire1") && fireAlarm <= 0)
        {
            var obj = Instantiate(projectilePrefab, projectilePivot);
            obj.transform.parent = null;

            //gunAnimator.SetBool("isShooting", true);
            gunAnimator.Play("GunShoot");

            fireAlarm = fireRate;
        }
        else
        {
            //gunAnimator.Play("GunIdle");

            //gunAnimator.SetBool("isShooting", false);
        }
    }

    // Lock the cursor in the center of the screen, and hide it
    void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

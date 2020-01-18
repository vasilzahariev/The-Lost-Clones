using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStarfighter : MonoBehaviour
{
    public Camera Camera;

    public float MaxSpeed;
    public float RotationSpeed;
    public float SideRotationSpeed;

    private Rigidbody rg;

    private const float maxMouseRotatorX = 40f;
    private const float maxMouseRotatorY = 40f;

    private float speed;
    private float mouseRotatorX;
    private float mouseRotatorY;

    private bool leftRotate;
    private bool rightRotate;

    void Start()
    {
        this.rg = this.gameObject.GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;

        this.speed = 0f;
        this.mouseRotatorX = 0f;
        this.mouseRotatorY = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.leftRotate = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            this.leftRotate = false;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            this.rightRotate = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            this.rightRotate = false;
        }
    }

    private void FixedUpdate()
    {
        this.Move();
        this.Rotate();
    }

    private void Move()
    {
        float z = (this.rightRotate ? -1 : (this.leftRotate ? 1 : 0)) * this.SideRotationSpeed * Time.fixedDeltaTime;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (this.speed + vertical > this.MaxSpeed)
        {
            this.speed = this.MaxSpeed;
        }
        else if (this.speed + vertical < 0)
        {
            this.speed = 0f;
        }
        else
        {
            this.speed += vertical * 100f;
        }

        this.rg.velocity = transform.forward * this.speed * Time.fixedDeltaTime;

        //this.rg.velocity = this.transform.forward * (this.forwards ? 1f : (this.backwards ? -1f : 0f)) * this.Speed * Time.fixedDeltaTime;
        this.transform.Rotate(0f, 0f, z);
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Add something like a restriction for how much mouseX is needed or something like this
        if (this.mouseRotatorX + (mouseX) > maxMouseRotatorX)
        {
            this.mouseRotatorX = maxMouseRotatorX;
        }
        else if (this.mouseRotatorX + (mouseX) < -maxMouseRotatorX)
        {
            this.mouseRotatorX = -maxMouseRotatorX;
        }
        else
        {
            this.mouseRotatorX += mouseX;
        }

        if (this.mouseRotatorY + (mouseY) > maxMouseRotatorY)
        {
            this.mouseRotatorY = maxMouseRotatorY;
        }
        else if (this.mouseRotatorY + (mouseY) < -maxMouseRotatorY)
        {
            this.mouseRotatorY = -maxMouseRotatorY;
        }
        else
        {
            this.mouseRotatorY += mouseY;
        }

        this.transform.Rotate(-this.mouseRotatorY * Time.fixedDeltaTime * this.RotationSpeed, this.mouseRotatorX * Time.fixedDeltaTime * this.RotationSpeed, 0f);
    }
}

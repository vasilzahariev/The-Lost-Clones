using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera Camera;

    public float Speed;
    public float BackwardsSpeed;
    public float RotationSpeed;
    public float JumpForce;

    private Animator animator;
    private Rigidbody rg;

    private float h;
    private float v;
    private bool forward;
    private bool prevForward;
    private bool jump;

    void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.rg = this.GetComponent<Rigidbody>();

        this.forward = false;
        this.jump = false;
    }

    void Update()
    {
        if (Input.GetAxis("Vertical") < 0f)
        {
            this.h = Input.GetAxis("Horizontal") * this.BackwardsSpeed;
            this.v = Input.GetAxis("Vertical") * this.BackwardsSpeed;

            this.prevForward = this.forward;

            this.forward = false;
        }
        else
        {
            this.h = Input.GetAxis("Horizontal") * this.Speed;
            this.v = Input.GetAxis("Vertical") * this.Speed;

            this.prevForward = this.forward;

            this.forward = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            this.jump = true;
        }
    }

    void FixedUpdate()
    {
        this.Rotate();
        this.Move();
        this.Jump();
    }

    private void Rotate()
    {
        Vector3 forward = this.Camera.transform.forward;
        Vector3 right = this.Camera.transform.right;

        Vector3 desiredMoveDirection = forward;

        desiredMoveDirection.y = 0f;

        Quaternion prevRotation = this.transform.rotation;

        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), this.RotationSpeed);
    }

    private void Move()
    {
        this.transform.Translate(this.h * Time.fixedDeltaTime, 0f, this.v * Time.fixedDeltaTime);

        if (this.v > 0f && this.forward && this.prevForward)
        {
            this.animator.SetFloat("Speed", 1f);
        }
        else if (this.v > 0f && !this.forward && this.prevForward)
        {
            this.animator.SetFloat("Speed", -1f);
        }
        else if (this.v < 0f && !this.forward && this.prevForward)
        {
            this.animator.SetFloat("Speed", -1f);
        }
        else if (this.v < 0f && this.forward && !this.prevForward)
        {
            this.animator.SetFloat("Speed", 1f);
        }
        else if (this.v == 0f && this.forward && this.forward)
        {
            this.animator.SetFloat("Speed", 0f);
        }
    }

    private void Jump()
    {
        if (this.jump)
        {
            this.rg.AddForce(this.transform.up * this.JumpForce * this.rg.mass);

            this.jump = false;
        }
    }
}

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


    private bool forward;
    private bool prevForward;
    private bool walking;
    private bool backwards;
    private bool jump; // Detects if there is a current jump
    private bool jumping; // Detects if the player is still jumping
    private bool falling;
    private bool running;

    private float h;
    private float v;
    private float maxJumpForce;
    private float currentJumpForce;
    private float defaultSpeed;
    private float defaultBackwardsSpeed;

    void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.rg = this.GetComponent<Rigidbody>();

        //this.forward = false;
        this.walking = false;
        this.backwards = false;
        this.jump = false;
        this.jumping = false;
        this.falling = false;
        this.running = false;

        this.maxJumpForce = this.JumpForce * 40;
        this.currentJumpForce = 0f;
        this.defaultSpeed = this.Speed;
        this.defaultBackwardsSpeed = this.BackwardsSpeed;
    }

    void Update()
    {
        if (this.jumping)
        {
            this.Speed = this.defaultSpeed / 2;
            this.BackwardsSpeed = this.defaultBackwardsSpeed / 2;
        }
        else
        {
            this.Speed = this.defaultSpeed;
            this.BackwardsSpeed = this.defaultBackwardsSpeed;
        }

        if (this.running)
        {
            this.Speed = this.defaultSpeed * 2;
        }
        else
        {
            this.Speed = this.defaultSpeed;
        }

        if (Input.GetAxis("Vertical") < 0f)
        {
            this.h = Input.GetAxis("Horizontal") * this.BackwardsSpeed;
            this.v = Input.GetAxis("Vertical") * this.BackwardsSpeed;
        }
        else
        {
            this.h = Input.GetAxis("Horizontal") * this.Speed;
            this.v = Input.GetAxis("Vertical") * this.Speed;
        }

        if (Input.GetButtonDown("Jump") && !this.falling)
        {

            this.jumping = true;

            this.animator.SetBool("IsJumping", this.jumping);
        }

        if (Input.GetButton("Jump") && this.currentJumpForce != this.maxJumpForce && this.jumping)
        {
            this.jump = true;

            this.currentJumpForce += this.JumpForce;
        }

        if (Input.GetButtonUp("Jump") || this.currentJumpForce == this.maxJumpForce)
        {
            this.jumping = false;

            this.animator.SetBool("IsFalling", true);
        }

        if (Input.GetButton("Run"))
        {
            this.running = true;
            this.animator.SetBool("IsRunning", this.running);
        }

        if (Input.GetButtonUp("Run"))
        {
            this.running = false;

            this.animator.SetBool("IsRunning", this.running);
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

        // Moving forward and backward
        if (this.v > 0.0f)
        {
            this.backwards = false;
            this.walking = true;

            this.animator.SetFloat("Speed", 1f);
        }
        else if (this.v < 0.0f)
        {
            this.walking = false;
            this.backwards = true;

            this.animator.SetFloat("Speed", -1f);
        }
        else if (this.v == 0f)
        {
            this.walking = false;
            this.backwards = false;

            this.animator.SetFloat("Speed", 0f);
        }

        // Moving sideways
        if (this.h > 0f)
        {
            this.animator.SetBool("IsMovingLeft", false);
            this.animator.SetBool("IsMovingRight", true);
        }
        else if (this.h > 0f && (this.walking || this.backwards))
        {
            this.animator.SetBool("IsMovingLeft", false);
            this.animator.SetBool("IsMovingRight", true);
        }
        else if (this.h < 0f)
        {
            this.animator.SetBool("IsMovingRight", false);
            this.animator.SetBool("IsMovingLeft", true);
        }
        else if (this.h < 0f && (this.walking || this.backwards))
        {
            this.animator.SetBool("IsMovingRight", false);
            this.animator.SetBool("IsMovingLeft", true);
        }
        else if (this.h == 0f)
        {
            this.animator.SetBool("IsMovingRight", false);
            this.animator.SetBool("IsMovingLeft", false);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.currentJumpForce = 0f;
            this.jumping = false;
            this.falling = false;
            this.animator.SetBool("IsJumping", this.jumping);
            this.animator.SetBool("IsFalling", this.falling);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.falling = false;

            this.animator.SetBool("IsFalling", this.falling);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.falling = true;

            this.animator.SetBool("IsFalling", this.falling);
        }
    }
}

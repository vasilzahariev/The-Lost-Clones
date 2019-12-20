using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera Camera;

    public float Speed;
    public float BackwardsSpeed;
    public float SideWaysSpeed;
    public float RunningSpeed;
    public float BackwardsRunningSpeed;
    public float SideWaysRunningSpeed;
    public float RotationSpeed;
    public float JumpForce;

    private Rigidbody rg;
    private Animator animator;
    private Player player;

    private bool walking;
    private bool backwards;
    private bool running;
    private bool right;
    private bool left;
    private bool jump;
    private bool jumping;
    private bool isInAir;

    private float h;
    private float v;

    void Start()
    {
        this.rg = this.GetComponent<Rigidbody>();
        this.animator = this.GetComponent<Animator>();
        this.player = this.GetComponent<Player>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        this.h = Input.GetAxis("Horizontal");
        this.v = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && !this.jumping)
        {
            this.jump = true;
            this.jumping = true;
        }

        if (Input.GetButtonDown("Run"))
        {
            this.running = true;
        }

        if (Input.GetButtonUp("Run"))
        {
            this.running = false;
        }
    }

    void FixedUpdate()
    {
        if (!this.player.IsDead)
        {
            this.Rotate();

            this.Move();

            this.AnimationParser();
        }
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
        this.walking = false;
        this.backwards = false;

        if (this.v > 0f)
        {
            this.v = this.running ? this.RunningSpeed : this.Speed;

            this.walking = true;
        }
        else if (this.v < 0f)
        {
            this.v = -(this.running ? this.BackwardsRunningSpeed : this.BackwardsSpeed);

            this.backwards = true;
        }

        this.right = false;
        this.left = false;

        if (this.h > 0f)
        {
            this.right = true;
        }
        else if (this.h < 0f)
        {
            this.left = true;
        }

        if (this.h != 0f)
        {
            if (this.running && !this.walking)
            {
                this.h = this.SideWaysRunningSpeed;
            }
            else
            {
                this.h = this.SideWaysSpeed;
            }
        }

        if (this.left)
        {
            this.h *= -1;
        }

        //this.h *= this.running ? this.SideWaysRunningSpeed : this.SideWaysSpeed;

        if (this.walking && this.left)
        {
            this.transform.Rotate(0f, -15f, 0f);
        }
        else if (this.walking && this.right)
        {
            this.transform.Rotate(0f, 15f, 0f);
        }
        else if (this.backwards && this.left)
        {
            this.transform.Rotate(0f, 15f, 0f);
        }
        else if (this.backwards && this.right)
        {
            this.transform.Rotate(0f, -15f, 0f);
        }

        this.rg.MovePosition(this.transform.position + (this.transform.forward * this.v * Time.fixedDeltaTime) + (this.transform.right * this.h * Time.fixedDeltaTime));
    }

    public void Jump()
    {
        if (this.jump && !this.player.IsDead)
        {
            this.rg.AddForce(this.transform.up * this.JumpForce);

            this.jump = false;
        }
    }

    private void AnimationParser()
    {
        this.animator.SetBool("IsWalking", this.walking);
        this.animator.SetBool("IsBackwards", this.backwards);
        this.animator.SetBool("IsMovingRight", this.right);
        this.animator.SetBool("IsMovingLeft", this.left);
        this.animator.SetBool("IsJumping", this.jumping);
        this.animator.SetBool("IsRunning", this.running);
        this.animator.SetBool("IsInAir", this.isInAir);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (this.jumping)
            {
                this.jump = false;
                this.jumping = false;
            }
            else
            {
                this.isInAir = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!this.jumping && collision.gameObject.CompareTag("Ground"))
        {
            this.isInAir = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!this.jumping && collision.gameObject.CompareTag("Ground"))
        {
            this.isInAir = true;
        }
    }
}

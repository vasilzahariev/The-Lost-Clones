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
    private bool isGoingToTouchTheGround;
    private bool isJumpFalling;
    private bool isJumpLanding;
    private bool canRaycastForLanding;

    private float h;
    private float v;
    private int counter;

    void Start()
    {
        this.rg = this.GetComponent<Rigidbody>();
        this.animator = this.GetComponent<Animator>();
        this.player = this.GetComponent<Player>();

        this.counter = 0;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        this.h = Input.GetAxis("Horizontal");
        this.v = Input.GetAxis("Vertical");

        //if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Longs_Jump_Platformer_Start"))
        //{
        //    Debug.Log("Start " + this.isJumpFalling + " " + this.isJumpLanding + " " + ++this.counter);
        //}

        //if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Longs_Jump_Platformer_Fall"))
        //{
        //    Debug.Log("Falling " + this.isJumpFalling + " " + this.isJumpLanding + " " + ++this.counter);
        //}

        //if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Longs_Jump_Platformer_Land"))
        //{
        //    Debug.Log("Landing " + this.isJumpFalling + " " + this.isJumpLanding + " " + ++this.counter);
        //}

        //RaycastHit hit;

        //if (Physics.Raycast(this.transform.position, -this.transform.up, out hit, 1f))
        //{
        //    if (this.isJumpFalling)
        //    {
        //        this.isJumpFalling = false;
        //        this.isJumpLanding = true;

        //        this.animator.SetBool("IsJumpFalling", this.isJumpFalling);
        //        this.animator.SetBool("IsJumpLanding", this.isJumpLanding);
        //    }
        //    else if (this.jumping && !this.isJumpLanding)
        //    {
        //        //Debug.Log(hit.collider.gameObject.name);
        //    }
        //}

        if (Input.GetButtonDown("Jump") && !this.jumping)
        {
            this.jump = true;
            this.jumping = true;
        }

        if (Input.GetButtonDown("Run") && this.walking)
        {
            this.running = true;
        }

        if (Input.GetButton("Run") && this.walking)
        {
            this.running = true;
        }

        if (Input.GetButtonUp("Run") || !this.walking)
        {
            this.running = false;
        }
    }

    void FixedUpdate()
    {
        if (!this.player.IsDead)
        {
            this.Rotate();

            if (!this.player.attacking)
            {
                this.Move();
            }

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
            //if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Longs_WalkFwd") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Longs_SprintLoop"))
            //{
            //    this.v = this.running ? this.RunningSpeed : this.Speed;
            //}
            //else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Longs_WalkFwdStart") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Longs_SprintStart"))
            //{
            //    this.v = this.running ? this.RunningSpeed / 2 : this.Speed / 2;
            //}
            //Longs_WalkFwdStart

            this.v = this.running ? this.RunningSpeed : this.Speed;

            this.walking = true;
        }
        else if (this.v < 0f)
        {
            this.v = -this.BackwardsSpeed;

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
            if (this.walking || this.running)
            {
                this.h = this.SideWaysSpeed / 2;
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

        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            this.rg.MovePosition(this.transform.position + (this.transform.forward * this.v * Time.fixedDeltaTime) + (this.transform.right * this.h * Time.fixedDeltaTime));
        }
    }

    public void Jump()
    {
        if (this.jump && !this.player.IsDead)
        {
            this.rg.AddForce(this.transform.up * this.JumpForce);

            this.jump = false;
        }
    }

    public void JumpFall()
    {
        this.isJumpFalling = true;
       // Debug.Log("Test");
    }

    public void JumpLand()
    {
        this.isJumpLanding = false;
    }

    public void AllowRaycastingForLanding()
    {
        Debug.Log("Test");
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
        this.animator.SetBool("IsJumpFalling", this.isJumpFalling);
        this.animator.SetBool("IsJumpLanding", this.isJumpLanding);
        this.animator.SetBool("Jump", this.jump);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (this.jumping)
            {
                this.jump = false;
                this.jumping = false;
                this.isJumpFalling = false;
                this.isJumpLanding = true;

                //this.animator.SetBool("IsJumpFalling", this.isJumpFalling);
                //this.animator.SetBool("IsJumpLanding", this.isJumpLanding);
            }
            else
            {
                this.isInAir = false;
            }
        }

        //Debug.Log(this.animator.GetBool("IsJumpFalling"));

        //float angle = Vector3.Angle(collision.contacts[0].normal, Vector3.up);

        //if (Mathf.Approximately(angle, 0) && this.jumping)
        //{
        //    this.jump = false;
        //    this.jumping = false;
        //    this.isJumpFalling = false;
        //    this.isJumpLanding = false;
        //}

    }

    private void OnCollisionStay(Collision collision)
    {
        //if (this.jumping)
        //{
        //    this.jumping = false;
        //    this.isJumpFalling = false;
        //    this.isJumpLanding = false;
        //}

        if (!this.jumping && collision.gameObject.CompareTag("Ground"))
        {
            //if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Longs_Jump_Platformer_Fall"))
            //{
            //    this.isJumpFalling = false;
            //    this.isJumpLanding = true;
            //}

            this.isJumpFalling = false;
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

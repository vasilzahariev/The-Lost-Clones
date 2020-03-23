using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Properties

    public Camera Camera;

    public float RotationSpeed;

    [HideInInspector]
    public bool Slide;

    [HideInInspector]
    public bool IsSliding;

    [HideInInspector]
    public bool Jumping;

    #endregion

    #region Fields

    private Animator animator;
    private Rigidbody rg;

    private float h;
    private float v;

    private bool forward;
    private bool backwards;
    private bool left;
    private bool right;
    private bool running;
    private bool jump;

    #endregion

    #region MonoMethods

    void Start()
    {
        this.animator = this.gameObject.GetComponent<Animator>();
        this.rg = this.gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.h = Input.GetAxis("Horizontal");
        this.v = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Run"))
        {
            this.running = true;
        }
        else if (Input.GetButtonUp("Run"))
        {
            this.running = false;
        }
        else if (Input.GetButton("Run"))
        {
            this.running = true;
        }

        if (Input.GetButtonDown("Slide") && !this.IsSliding && !this.Slide)
        {
            this.Slide = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            this.jump = true;
        }

        //Debug.Log("Jumping: " + this.Jumping + ". Jump: " + this.jump);
    }

    private void FixedUpdate()
    {
        //if (Input.GetButton("Fire2"))
        //{
        //    this.Rotate();
        //}

        if (!this.IsSliding)
        {
            this.Rotate();
        }

        this.Move();

        this.AnimationParser();
    }

    #endregion

    // TODO: Add a little air mobility
    #region Methods
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
        if (this.v > 0)
        {
            this.forward = true;
            this.backwards = false;
        }
        else if (this.v < 0)
        {
            this.backwards = true;
            this.forward = false;
        }
        else
        {
            this.forward = false;
            this.backwards = false;
        }

        if (this.h > 0)
        {
            this.right = true;
            this.left = false;
        }
        else if (this.h < 0)
        {
            this.left = true;
            this.right = false;
        }
        else
        {
            this.left = false;
            this.right = false;
        }

        if (!this.forward && !this.backwards && !this.left && !this.right && this.running)
        {
            this.running = false;
        }

        if (!this.IsSliding)
        {
            if (!this.running)
            {
                this.Slide = false;
            }
            else
            {
                if (this.Slide && (this.backwards || this.left || this.right))
                {
                    this.Slide = false;
                }
            }
        }
    }

    public void MakeTheJump()
    {
        this.rg.AddForce(this.transform.up * 500f);
    }

    private void AnimationParser()
    {
        this.animator.SetBool("Forward", this.forward);
        this.animator.SetBool("Backwards", this.backwards);
        this.animator.SetBool("Left", this.left);
        this.animator.SetBool("Right", this.right);
        this.animator.SetBool("IsRunning", this.running);
        this.animator.SetBool("Slide", this.Slide);
        this.animator.SetBool("IsSliding", this.Slide);
        this.animator.SetBool("Jump", this.jump);
        this.animator.SetBool("IsJumping", this.Jumping);
    }

    #endregion

    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Test");

        if (collision.gameObject.CompareTag("Ground"))
        {
            this.jump = false;
            this.Jumping = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && this.Jumping == true)
        {
            this.jump = false;
            this.Jumping = false;
        }
    }
    #endregion
}

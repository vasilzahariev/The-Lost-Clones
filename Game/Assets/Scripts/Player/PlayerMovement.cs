using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Properties

    public Camera Camera;
    public GameObject Parent;

    public float RotationSpeed;
    public float JumpForce;
    public float AirSpeed;
    public float AirRunSpeed;

    public float RotationMultiplier;


    [HideInInspector]
    public bool Slide;

    [HideInInspector]
    public bool IsSliding;

    [HideInInspector]
    public bool Jumping;

    [HideInInspector]
    public bool CanStopSliding;

    #endregion

    #region Fields

    private Player player;

    private Animator animator;
    private Rigidbody rg;

    private float h;
    private float v;

    private float rotVal;

    private bool forward;
    private bool backwards;
    private bool left;
    private bool right;
    private bool running;
    private bool jump;
    private bool isInAir;
    private bool wasInAir;

    private bool move;

    #endregion

    #region MonoMethods

    void Start()
    {
        this.player = this.gameObject.GetComponent<Player>();

        this.animator = this.gameObject.GetComponent<Animator>();
        this.rg = this.gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (this.player.IsConsoleActive)
        {
            return;
        }

        this.h = Input.GetAxis("Horizontal");
        this.v = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.W))
        {
            this.move = true;

            this.rotVal = 0f;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            this.move = false;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.move = true;

            this.rotVal = 180f;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            this.move = false;
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.move = true;

            this.rotVal = -90f;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            this.move = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.move = true;

            this.rotVal = 90f;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            this.move = false;
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            this.rotVal = 45f;
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            this.rotVal = -45f;
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            this.rotVal = 135f;
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            this.rotVal = -135f;
        }

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    this.gameObject.transform.Rotate(0f, 180f, 0f);
        //}

        //if (Input.GetKey(KeyCode.D))
        //{
        //    this.gameObject.transform.Rotate(0f, 90f * Time.deltaTime * this.RotationMultiplier, 0f);
        //}


        if (!this.IsSliding)
        {
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
        }

        if (Input.GetButtonDown("Slide") && !this.IsSliding && !this.Slide)
        {
            this.Slide = true;
        }

        if (Input.GetButtonDown("Jump") && (!this.jump && !this.IsSliding))
        {
            this.jump = true;
        }


        // Maybe redo it, if you think is so ugly, that you can't even watch it
        if (this.CanStopSliding && this.IsSliding && !this.Slide)
        {
            this.Slide = true;
        }
    }

    private void FixedUpdate()
    {
        if (this.move || this.isInAir || this.Jumping)
        {
            this.Rotate();
        }

        if (this.move && !this.isInAir && !this.Jumping)
        {
            this.transform.Rotate(0f, this.rotVal, 0f);
        }

        if (this.player.IsConsoleActive)
        {
            this.forward = false;
            this.backwards = false;
            this.left = false;
            this.right = false;
            this.running = false;

            return;
        }

        if (this.Jumping || this.isInAir)
        {
            this.AirMove();
        }
        else
        {
            //this.Move();
        }
    }

    private void LateUpdate()
    {
        this.AnimationParser();
    }

    #endregion

    #region Methods
    

    /// <summary>
    /// This method controls the rotation of the player, using the rotation of the camera and mouse.
    /// </summary>
    private void Rotate()
    {
        Vector3 forward = this.Camera.transform.forward;
        Vector3 right = this.Camera.transform.right;

        Vector3 desiredMoveDirection = forward;

        desiredMoveDirection.y = 0f;

        Quaternion prevRotation = this.transform.rotation;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(desiredMoveDirection), this.RotationSpeed);
    }


    /// <summary>
    /// This method determens what movements the player can execute.
    /// </summary>
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

    /// <summary>
    /// This method controls the movement of the player when in air.
    /// </summary>
    private void AirMove()
    {
        float speed = this.running ? this.AirRunSpeed : this.AirSpeed;

        this.rg.MovePosition(this.transform.position +
                            (this.transform.forward * this.v * speed * Time.fixedDeltaTime) +
                            (this.transform.right * this.h * speed * Time.fixedDeltaTime));
    }

    /// <summary>
    /// This method is called when the jump_start animation is executed, so the player can jump using a rigidbody.
    /// </summary>
    public void MakeTheJump()
    {
        this.rg.AddForce(this.transform.up * this.JumpForce);
    }

    /// <summary>
    /// This method controls what values the animation controller parameters have
    /// </summary>
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
        this.animator.SetBool("CanStopSliding", this.CanStopSliding);
        this.animator.SetBool("IsInAir", this.isInAir);
        this.animator.SetBool("Move", this.move);
    }

    #endregion

    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("SlideArea"))
        {
            if (this.Jumping)
            {
                this.jump = false;
                this.Jumping = false;
            }
            
            if (this.isInAir)
            {
                this.isInAir = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("SlideArea"))
        {
            if (this.Jumping)
            {
                this.jump = false;
                this.Jumping = false;
            }
            
            if (this.isInAir)
            {
                this.isInAir = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("SlideArea")) &&
            !this.isInAir && !this.Jumping)
        {
            this.isInAir = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SlideArea") && this.IsSliding)
        {
            this.CanStopSliding = false;

            this.wasInAir = this.isInAir;
            this.isInAir = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("SlideArea") && this.IsSliding)
        {
            this.CanStopSliding = false;

            this.isInAir = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SlideArea") && this.IsSliding)
        {
            this.CanStopSliding = true;

            this.isInAir = this.wasInAir;
        }
    }

    #endregion
}

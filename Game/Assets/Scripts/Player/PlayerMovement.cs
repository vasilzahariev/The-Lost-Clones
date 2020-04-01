using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Properties

    public Camera Camera;
    public Transform PointForRotation;

    public float RotationSpeed;
    public float JumpForce;
    public float DashingForce;
    public float DashingForceUp;
    public float AirSpeed;
    public float AirRunSpeed;


    [HideInInspector]
    public bool Slide;

    [HideInInspector]
    public bool IsSliding;

    [HideInInspector]
    public bool Jumping;

    [HideInInspector]
    public bool CanStopSliding;

    [HideInInspector]
    public bool Dash;

    [HideInInspector]
    public bool Dashing;

    [HideInInspector]
    public bool CanDash;

    #endregion

    #region Fields

    private Player player;

    private Animator animator;
    private Rigidbody rg;

    private float h;
    private float v;
    private float rotVal;
    private float currentJumpForce;

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

        this.currentJumpForce = 0f;

        this.CanDash = true;
    }

    void Update()
    {
        if (this.player.IsConsoleActive)
        {
            return;
        }

        this.h = Input.GetAxis("Horizontal");
        this.v = Input.GetAxis("Vertical");

        if (!this.player.IsTargetAcquired && !this.IsSliding)
        {
            this.NoTargetMovementInput();
        }

        if (!this.IsSliding)
        {
            if (Input.GetButtonDown("Run"))
            {
                this.running = true;
            }
            else if (Input.GetButton("Run"))
            {
                this.running = true;
            }
            else if (Input.GetButtonUp("Run"))
            {
                this.running = false;
            }
        }

        if (Input.GetButtonDown("Slide") &&
            !this.IsSliding &&
            !this.Slide &&
            !this.jump &&
            !this.Jumping &&
            !this.isInAir &&
            !this.Dashing &&
            !this.Dash)
        {
            this.Slide = true;
        }

        if (Input.GetKeyDown(KeyCode.C) &&
            this.CanDash &&
            !this.Dashing &&
            !this.Dash &&
            !this.IsSliding &&
            !this.Slide &&
            !this.player.IsTargetAcquired)
        {
            this.Dash = true;
        }

        if (Input.GetButtonDown("Jump") &&
            !this.jump &&
            !this.IsSliding &&
            !this.Slide &&
            !this.Dash &&
            !this.Dashing &&
            !this.isInAir)
        {
            this.jump = true;
        }

        if (this.CanStopSliding && this.IsSliding && !this.Slide)
        {
            this.Slide = true;
        }
    }

    private void FixedUpdate()
    {
        if (this.player.IsConsoleActive)
        {
            this.forward = false;
            this.backwards = false;
            this.left = false;
            this.right = false;
            this.running = false;

            return;
        }

        if (this.Dashing)
        {
            this.rg.AddForce(this.transform.forward * this.DashingForce);
        }

        if (!this.player.IsTargetAcquired)
        {
            if (this.move || this.isInAir || this.Jumping)
            {
                this.Rotate();
            }

            if (this.move && !this.Jumping && !this.isInAir)
            {
                this.transform.Rotate(0f, this.rotVal, 0f);
            }

            if (this.Jumping || this.isInAir)
            {
                this.transform.Rotate(0f, this.rotVal, 0f);
                this.AirMove();
            }
        }
        else
        {
            this.Rotate();

            if (this.Jumping || this.isInAir)
            {
                this.AirMove();
            }
            else
            {
                this.Move();
            }
        }
    }

    private void LateUpdate()
    {
        this.AnimationParser();
    }

    #endregion

    #region Methods

    private void NoTargetMovementInput()
    {
        if (Input.GetKeyUp(KeyCode.W) ||
            Input.GetKeyUp(KeyCode.S) ||
            Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.D))
        {
            this.move = false;
        }

        if (Input.GetKey(KeyCode.W))
        {
            this.move = true;

            this.rotVal = 0f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.move = true;

            this.rotVal = 180f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.move = true;

            this.rotVal = -90f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.move = true;

            this.rotVal = 90f;
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
    }

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
        float speed = this.Dashing && !this.player.IsTargetAcquired ? this.AirRunSpeed : this.AirSpeed;

        if (!this.player.IsTargetAcquired)
        {
            if (this.rotVal == 180f)
            {
                this.v = -this.v;
                this.h = 0f;
            }
            else if (this.rotVal == 90f)
            {
                float oldH = this.h;

                this.v = this.h;
                this.h = 0f;
            }
            else if (this.rotVal == -90f)
            {
                float oldH = this.h;

                this.v = -this.h;
                this.h = 0f;
            }
            else if (this.rotVal == 45f || this.rotVal == -45f)
            {
                this.h = 0f;
            }
            else if (this.rotVal == 135f || this.rotVal == -135f)
            {
                this.v = -this.v;
                this.h = 0f;
            }
        }

        this.rg.MovePosition(this.transform.position +
                            (this.transform.forward * this.v * speed * Time.fixedDeltaTime) +
                            (this.transform.right * this.h * speed * Time.fixedDeltaTime));
    }

    /// <summary>
    /// This method is called when the jump_start animation is executed, so the player can jump using a rigidbody.
    /// </summary>
    private void Land()
    {
        this.jump = false;
        this.Jumping = false;
        this.isInAir = false;
        this.currentJumpForce = 0f;
    }

    public void MakeTheJump()
    {
        this.rg.AddForce(this.transform.up * this.JumpForce);
    }

    public void MakeThemZero()
    {
        if (this.player.IsTargetAcquired)
        {
            this.move = false;
            this.running = false;
            this.rotVal = 0f;
            this.CanStopSliding = true;
            this.IsSliding = false;
            this.Slide = false;
            this.Dashing = false;
            this.Dash = false;
        }
        else
        {
            this.forward = false;
            this.backwards = false;
            this.left = false;
            this.right = false;
            this.running = false;
        }
    }

    public void HoldOnDash()
    {
        this.rg.AddForce(this.transform.up * this.DashingForceUp);
        this.rg.velocity = Vector3.zero;

        StartCoroutine(this.DashWithoutGravity());
    }

    private IEnumerator DashWithoutGravity()
    {
        this.rg.useGravity = false;

        yield return new WaitForSecondsRealtime(0.4f);

        // TODO: When you add artificial gravity make sure to change this one
        this.rg.useGravity = true;
    }

    public void ReloadDash()
    {
        StartCoroutine(this.WaitForReloadDash());
    }

    private IEnumerator WaitForReloadDash()
    {
        yield return new WaitForSecondsRealtime(1f);

        this.CanDash = true;
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
        this.animator.SetBool("Dash", this.Dash);
        this.animator.SetBool("IsDashing", this.Dashing);
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
                this.Land();
            }

            if (this.isInAir)
            {
                this.Land();
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
                this.Land();
            }

            if (this.isInAir)
            {
                this.Land();
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

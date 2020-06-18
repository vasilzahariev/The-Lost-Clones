using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class controlls the movement of the player
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    #region Properties

    public Transform PointForRotation;

    public float JumpForce;
    public float DashingForce;
    public float DashingForceUp;

    public bool Slide { get; set; }

    public bool IsSliding { get; set; }

    public bool Jumping { get; set; }

    public bool CanStopSliding { get; set; }

    public bool Dash { get; set; }

    public bool Dashing { get; set; }

    public bool CanDash { get; set; }

    public bool Dodge { get; set; }

    public bool Dodging { get; set; }

    public bool CanDodge { get; set; }

    #endregion

    #region Fields

    [SerializeField] private Camera _camera;

    private Player _player;

    private LightsaberController _lightsaberController;

    private Animator _animator;
    private Rigidbody _rg;

    private PlayerInput _input;

    private Vector2 _movementInput;

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _airSpeed;
    [SerializeField] private float _airRunSpeed;

    private float _h;
    private float _v;
    private float _rotVal;
    private float _dodgePressedTime;

    private bool _forward;
    private bool _backwards;
    private bool _left;
    private bool _right;
    private bool _running;
    private bool _jump;
    private bool _isInAir;
    private bool _wasInAir;
    private bool _move;
    private bool _isDodgePressed;
    private bool _resetDodgePressed;

    #endregion

    #region MonoMethods

    void Start()
    {
        this._player = this.gameObject.GetComponent<Player>();
        this._lightsaberController = this._player.GetComponentInChildren<LightsaberController>();

        this._animator = this.gameObject.GetComponent<Animator>();
        this._rg = this.gameObject.GetComponent<Rigidbody>();

        this.CanDash = true;
        this.CanDodge = true;
    }

    void Update()
    {
        if (this._player.IsConsoleActive)
        {
            return;
        }

        this._h = Input.GetAxis("Horizontal");
        this._v = Input.GetAxis("Vertical");

        if (!this._player.IsTargetAcquired && !this.IsSliding)
        {
            this.NoTargetMovementInput();
        }

        if (!this._lightsaberController.IsBlocking && !this.IsSliding && Input.GetButton("Run"))
        {
            this._running = true;
        }

        if (Input.GetButton("Run") && this._lightsaberController.IsBlocking)
            this._running = false;

        if (this.CanStopSliding && this.IsSliding && !this.Slide && !this._player.IsTargetAcquired)
        {
            this.Slide = true;
        }
    }

    private void FixedUpdate()
    {
        if (this.Jumping && this._isInAir)
        {
            this._isInAir = false;
        }

        if (this._player.IsConsoleActive)
        {
            this._forward = false;
            this._backwards = false;
            this._left = false;
            this._right = false;
            this._running = false;

            return;
        }

        if (this._lightsaberController.Attacking ||
            this._lightsaberController.HeavyAttacking)
        {
            this._move = false;
        }

        if (this.Dashing && !this._player.IsTargetAcquired)
        {
            this._rg.AddForce(this.transform.forward * this.DashingForce * this._rg.mass);
        }

        if (!this._player.IsTargetAcquired)
        {
            if (this._move || this._isInAir || this.Jumping)
            {
                this.Rotate();
            }

            if (this._move && !this.Jumping && !this._isInAir)
            {
                this.transform.Rotate(0f, this._rotVal, 0f);
            }

            if (this.Jumping || this._isInAir)
            {
                this.transform.Rotate(0f, this._rotVal, 0f);
                this.AirMove();
            }
        }
        else
        {
            //this.Rotate();

            if (this.Jumping || this._isInAir)
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
        if (_lightsaberController.Attacking ||
            _lightsaberController.HeavyAttacking ||
            _lightsaberController.StealthKilling ||
            _lightsaberController.ShouldExecuteStealthKill)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.W) ||
            Input.GetKeyUp(KeyCode.S) ||
            Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.D))
        {
            this._move = false;
        }

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            this._move = true;

            this._rotVal = 0f;
        }

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            this._move = true;

            this._rotVal = 180f;
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            this._move = true;

            this._rotVal = -90f;
        }

        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            this._move = true;

            this._rotVal = 90f;
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A))
        {
            this._rotVal = 45f;
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            this._rotVal = -45f;
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A))
        {
            this._rotVal = 135f;
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D))
        {
            this._rotVal = -135f;
        }
    }

    /// <summary>
    /// This method controls the rotation of the player, using the rotation of the camera and mouse.
    /// </summary>
    private void Rotate()
    {
        Vector3 forward = this._camera.transform.forward;
        Vector3 right = this._camera.transform.right;

        Vector3 desiredMoveDirection = forward;

        desiredMoveDirection.y = 0f;

        Quaternion prevRotation = this.transform.rotation;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(desiredMoveDirection), this._rotationSpeed);
    }


    /// <summary>
    /// This method determens what movements the player can execute.
    /// </summary>
    private void Move()
    {
        if (this._v > 0)
        {
            this._forward = true;
            this._backwards = false;
        }
        else if (this._v < 0)
        {
            this._backwards = true;
            this._forward = false;
        }
        else
        {
            this._forward = false;
            this._backwards = false;
        }

        if (this._h > 0)
        {
            this._right = true;
            this._left = false;
        }
        else if (this._h < 0)
        {
            this._left = true;
            this._right = false;
        }
        else
        {
            this._left = false;
            this._right = false;
        }

        if (!this._forward && !this._backwards && !this._left && !this._right && this._running)
        {
            this._running = false;
        }
    }

    /// <summary>
    /// This method controls the movement of the player when in air.
    /// </summary>
    private void AirMove()
    {
        if (this._lightsaberController.AirAttacking)
        {
            return;
        }

        float speed = this.Dashing && !this._player.IsTargetAcquired ? this._airRunSpeed : this._airSpeed;

        if (!this._player.IsTargetAcquired)
        {
            if (this._rotVal == 180f)
            {
                this._v = -this._v;
                this._h = 0f;
            }
            else if (this._rotVal == 90f)
            {
                float oldH = this._h;

                this._v = this._h;
                this._h = 0f;
            }
            else if (this._rotVal == -90f)
            {
                float oldH = this._h;

                this._v = -this._h;
                this._h = 0f;
            }
            else if (this._rotVal == 45f || this._rotVal == -45f)
            {
                this._h = 0f;
            }
            else if (this._rotVal == 135f || this._rotVal == -135f)
            {
                this._v = -this._v;
                this._h = 0f;
            }
        }

        this._rg.MovePosition(this.transform.position +
                            (this.transform.forward * this._v * speed * Time.fixedDeltaTime) +
                            (this.transform.right * this._h * speed * Time.fixedDeltaTime));
    }

    private void Land()
    {
        this._jump = false;
        this.Jumping = false;
        this._isInAir = false;
    }

    private bool IsMovingAtADirection()
    {
        return (this._forward && !this._backwards && !this._left && !this._right) ||
               (!this._forward && this._backwards && !this._left && !this._right) ||
               (!this._forward && !this._backwards && this._left && !this._right) ||
               (!this._forward && !this._backwards && !this._left && this._right) ||
               (this._forward && !this._backwards && !this._left && this._right) ||
               (this._forward && !this._backwards && this._left && !this._right) ||
               (!this._forward && this._backwards && !this._left && this._right) ||
               (!this._forward && this._backwards && this._left && !this._right);
    }

    private bool CheckIfTheCollisionIsFromUnder(Collision collision)
    {
        bool down = false;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);

            if (contact.normal.y >= 0.7f)
            {
                down = true;
            }
        }

        return down;
    }

    public void TakeMovementInput(Vector2 newValues)
    {
        this._movementInput = newValues;
    }

    public void TakeRunInput(bool run)
    {
        if (!this.IsSliding)
            this._running = this._lightsaberController.IsBlocking ? false : run;
    }

    public void TakeSlideInput()
    {
        if (_running &&
            !this.IsSliding &&
            !this.Slide &&
            !_jump &&
            !this.Jumping &&
            !_isInAir &&
            !this.Dashing &&
            !this.Dash &&
            !_player.IsTargetAcquired &&
            !_lightsaberController.Attacking &&
            !_lightsaberController.HeavyAttacking &&
            !_lightsaberController.StealthKilling &&
            !_lightsaberController.ShouldExecuteStealthKill)
        {
            this.Slide = true;
        }
    }

    public void TakeDashInput()
    {
        if (this.CanDash &&
            !this.Dashing &&
            !this.Dash &&
            !this.IsSliding &&
            !this.Slide &&
            !this._player.IsTargetAcquired &&
            !this._lightsaberController.Attacking &&
            !this._lightsaberController.HeavyAttacking &&
            !this._lightsaberController.IsAttackRecovering &&
            !this._lightsaberController.IsBlocking &&
            !_lightsaberController.StealthKilling &&
            !_lightsaberController.ShouldExecuteStealthKill)
        {
            this.Dash = true;
        }
    }

    public void TakeRollInput()
    {
        if (!this.Dodge &&
            !this.Dodging &&
            !this._isInAir &&
            !this._jump &&
            !this.Jumping &&
            this._player.IsTargetAcquired &&
            this.IsMovingAtADirection() &&
            this.CanDodge &&
            !this._lightsaberController.Attacking &&
            !this._lightsaberController.HeavyAttacking &&
            !this._lightsaberController.IsAttackRecovering &&
            !_lightsaberController.StealthKilling &&
            !_lightsaberController.ShouldExecuteStealthKill)
        {
            this.Dodge = true;
        }
    }

    public void TakeJumpInput()
    {
        if (!this._jump &&
            !this.IsSliding &&
            !this.Slide &&
            !this.Dash &&
            !this.Dashing &&
            !this._isInAir &&
            !this.Dodge &&
            !this.Dodging &&
            !this._lightsaberController.Attacking &&
            !this._lightsaberController.HeavyAttacking &&
            !_lightsaberController.StealthKilling &&
            !_lightsaberController.ShouldExecuteStealthKill)
        {
            this._jump = true;
        }
    }

    /// <summary>
    /// This method is called when the jump_start animation is executed, so the player can jump using a rigidbody.
    /// </summary>
    public void MakeTheJump()
    {
        this._rg.AddForce(this.transform.up * this.JumpForce * this._rg.mass);
    }

    public void MakeThemZero()
    {
        if (this._player.IsTargetAcquired)
        {
            this._move = false;
            this._running = false;
            this._rotVal = 0f;
            this.CanStopSliding = true;
            this.IsSliding = false;
            this.Slide = false;
            this.Dashing = false;
            this.Dash = false;

            this.transform.Rotate(0f, this._rotVal, 0f);
        }
        else
        {
            this._forward = false;
            this._backwards = false;
            this._left = false;
            this._right = false;
            this._running = false;
        }
    }

    public void MakeThemZeroWhenSliding()
    {
        this._move = false;
        this._running = false;
    }

    public void HoldOnDash()
    {
        if (this._isInAir || this.Jumping)
        {
            this._rg.AddForce(this.transform.up * this.DashingForceUp * this._rg.mass);
        }
        else
        {
            this._rg.AddForce(this.transform.up * this.DashingForceUp / 2f * this._rg.mass);
        }

        this._rg.velocity = Vector3.zero;

        StartCoroutine(this.DashWithoutGravity());
    }

    private IEnumerator DashWithoutGravity()
    {
        this._rg.useGravity = false;

        yield return new WaitForSecondsRealtime(0.4f);

        this._rg.useGravity = true;
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

    public void ReloadDodge()
    {
        StartCoroutine(this.WaitForReloadDodge());
    }

    private IEnumerator WaitForReloadDodge()
    {
        yield return new WaitForSecondsRealtime(0f);

        this.CanDodge = true;
    }

    public void StartSlideResize()
    {
        this.gameObject.GetComponent<CapsuleCollider>().height /= 2;
        this.gameObject.GetComponent<CapsuleCollider>().center /= 2;
    }

    public void EndSlideResize()
    {
        this.gameObject.GetComponent<CapsuleCollider>().height *= 2;
        this.gameObject.GetComponent<CapsuleCollider>().center *= 2;
    }

    public bool IsMoving()
    {
        return this._move || this._forward || this._backwards || this._left || this._right;
    }

    public bool IsRunning()
    {
        return this._running;
    }

    public bool IsInAir()
    {
        return this._isInAir;
    }

    public void SetInAir(bool value)
    {
        this._isInAir = value;
    }

    public bool IsMovingForward()
    {
        if ((_player.IsTargetAcquired && _forward) || (!_player.IsTargetAcquired && _move))
            return true;
        else
            return false;
    }

    /// <summary>
    /// This method controls what values the animation controller parameters have
    /// </summary>
    private void AnimationParser()
    {
        this._animator.SetBool("Forward", this._forward);
        this._animator.SetBool("Backwards", this._backwards);
        this._animator.SetBool("Left", this._left);
        this._animator.SetBool("Right", this._right);
        this._animator.SetBool("IsRunning", this._running);
        this._animator.SetBool("Slide", this.Slide);
        this._animator.SetBool("IsSliding", this.Slide);
        this._animator.SetBool("Jump", this._jump);
        this._animator.SetBool("IsJumping", this.Jumping);
        this._animator.SetBool("CanStopSliding", this.CanStopSliding);
        this._animator.SetBool("IsInAir", this._isInAir);
        this._animator.SetBool("WasInAir", this._wasInAir);
        this._animator.SetBool("Move", this._move);
        this._animator.SetBool("Dash", this.Dash);
        this._animator.SetBool("IsDashing", this.Dashing);
        this._animator.SetBool("Dodge", this.Dodge);
        this._animator.SetBool("IsDodging", this.Dodging);
    }

    #endregion

    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (this.CheckIfTheCollisionIsFromUnder(collision) &&
            collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log($"Test at {Time.time}");
            this.Land();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (this.CheckIfTheCollisionIsFromUnder(collision) &&
            (collision.gameObject.CompareTag("Ground")))
        {
            if (this.Jumping)
            {
                this.Land();
            }

            if (this._isInAir)
            {
                this.Land();

                this._wasInAir = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("SlideArea")) &&
            !this._isInAir &&
            !this.Jumping)
        {
            this._isInAir = true;
            this._wasInAir = false;
            this._lightsaberController.MakeThemZero();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SlideArea") && this.IsSliding)
        {
            this.CanStopSliding = false;

            this._wasInAir = this._isInAir;
            this._isInAir = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("SlideArea") && this.IsSliding)
        {
            this.CanStopSliding = false;

            this._isInAir = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SlideArea") && this.IsSliding)
        {
            this.CanStopSliding = true;

            this._isInAir = this._wasInAir;
        }
    }

    #endregion
}

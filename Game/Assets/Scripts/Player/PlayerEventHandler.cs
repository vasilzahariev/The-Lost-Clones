using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    private Player _player;
    private PlayerMovement _playerMovement;
    private AudioManager _audioManager;
    private LightsaberController _lightsaberController;
    private Animator _animator;

    private bool _wasPlayingJumpRumble;
    private bool _wasArtificialGravityActivated;

    #region MonoMethods

    void Awake()
    {
        this._player = this.gameObject.GetComponent<Player>();
        this._playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        this._audioManager = FindObjectsOfType<AudioManager>()[0];
        this._lightsaberController = this._player.GetComponentInChildren<LightsaberController>();
        this._animator = this._player.gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!this._playerMovement.IsMoving() && this._audioManager.IsPlaying("RockWalk"))
        {
            this._audioManager.Stop("RockWalk");
        }

        if (!this._playerMovement.IsRunning() && this._audioManager.IsPlaying("RockSprint"))
        {
            this._audioManager.Stop("RockSprint");
        }

        if (this._playerMovement.IsInAir() && !this._audioManager.IsPlaying("ForceJumpRumble") && !this._playerMovement.Jumping && !this._playerMovement.Dashing)
        {
            this._audioManager.Play("ForceJumpRumble");
        }
        else if (!this._playerMovement.IsInAir() && this._audioManager.IsPlaying("ForceJumpRumble") && !this._playerMovement.Jumping)
        {
            this._audioManager.Stop("ForceJumpRumble");
        }
    }

    private void FixedUpdate()
    {
        if (this._lightsaberController.IsHeavyAttackRecovering &&
            !this._animator.GetCurrentAnimatorStateInfo(0).IsName("Dual_HeavyAttack_End") &&
            !this._animator.GetCurrentAnimatorStateInfo(0).IsName("Dual_HeavyAttack"))
        {
            this._lightsaberController.IsHeavyAttackRecovering = false;
        }

        if (this._lightsaberController.Attacking ||
            this._lightsaberController.AirAttacking ||
            this._lightsaberController.HeavyAttacking)
        {
            this.TurnTrails("on");
        }

        if (!this._lightsaberController.AirAttacking &&
            !this._player.gameObject.GetComponent<Rigidbody>().useGravity &&
            !this._playerMovement.Dashing)
        {
            this._player.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }

        //if (!this.lightsaberController.Attacking &&
        //    !this.lightsaberController.AirAttacking)
        //{
        //    this.TurnTrails("off");
        //}
    }

    #endregion

    #region Move

    public void Step()
    {
        if (this._playerMovement.IsRunning())
        {
            this._audioManager.Play("RockSprint");
        }
        else
        {
            this._audioManager.Play("RockWalk");
        }
    }

    #endregion

    #region Slide
    public void StartSliding()
    {
        this._playerMovement.IsSliding = true;
        this._playerMovement.CanStopSliding = true;

        this._playerMovement.MakeThemZeroWhenSliding();

        this._playerMovement.StartSlideResize();

        this._audioManager.Play("ForceDash");
    }

    public void EndSlide()
    {
        this._playerMovement.Slide = false;
        this._playerMovement.IsSliding = false;

        this._playerMovement.EndSlideResize();
    }

    #endregion

    #region Jump

    public void StartJump()
    {
        this._audioManager.Play("ForceJump");

        this._playerMovement.MakeTheJump();
    }

    public void StartJumping()
    {
        this._playerMovement.Jumping = true;

        this._audioManager.Play("ForceJumpRumble");
    }

    public void EndJump()
    {
        this._audioManager.Play("RockLand");
        this._audioManager.Stop("ForceJumpRumble");
    }

    #endregion

    #region Dash

    public void StartDash()
    {
        this._playerMovement.Dashing = true;
        this._playerMovement.CanDash = false;

        this._audioManager.Play("ForceDash");

        if (!this._player.ArtificialGravity)
        {
            this._playerMovement.HoldOnDash();
        }

        this._wasPlayingJumpRumble = this._audioManager.IsPlaying("ForceJumpRumble");
        this._audioManager.Stop("ForceJumpRumble");
    }

    public void EndDash()
    {
        if (this._playerMovement.Jumping && this._playerMovement.IsInAir())
        {
            this._playerMovement.SetInAir(false);
        }

        this._playerMovement.Dash = false;
        this._playerMovement.Dashing = false;

        this._playerMovement.ReloadDash();

        if (this._wasPlayingJumpRumble)
        {
            this._audioManager.Play("ForceJumpRumble");

            this._wasPlayingJumpRumble = false;
        }
    }

    #endregion

    #region Dodge

    public void StartDodge()
    {
        this._playerMovement.Dodging = true;
        this._playerMovement.CanDodge = false;
    }

    public void PlayDodgeSound()
    {
        this._audioManager.Play("RockRoll");
    }

    public void EndDodge()
    {
        this._playerMovement.Dodge = false;
        this._playerMovement.Dodging = false;

        this._playerMovement.ReloadDodge();
    }

    #endregion

    #region Attacks

    public void StartAttack()
    {
        this._lightsaberController.CurrentlyPlayingAttack = this._lightsaberController.CurrentAttack;
        this._lightsaberController.CanDealDamage(true);

        this._lightsaberController.CanTransitionAttack = false;
    }

    public void StopAttack()
    {
        if (this._lightsaberController.CurrentlyPlayingAttack == this._lightsaberController.CurrentAttack)
        {
            this._lightsaberController.Attacking = false;
            this._lightsaberController.CurrentAttack = 0;
            this._lightsaberController.CurrentlyPlayingAttack = 0;

            this._lightsaberController.CanDealDamage(false);
        }

        this._lightsaberController.CanTransitionAttack = true;
    }

    public void StartHeavyAttack()
    {
        this._lightsaberController.HeavyAttacking = true;
        this._lightsaberController.CanDealDamage(true);
    }

    public void StopHeavyAttack()
    {
        this._lightsaberController.HeavyAttacking = false;
        this._lightsaberController.CanDealDamage(false);
    }

    public void StartAirAttack()
    {
        this._lightsaberController.CurrentlyPlayingAirAttack = this._lightsaberController.CurrentAirAttack;
        this._lightsaberController.CanDealDamage(true);

        if (this._lightsaberController.CurrentlyPlayingAirAttack == 1)
        {
            Rigidbody playerRigidbody = this._player.gameObject.GetComponent<Rigidbody>();

            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.useGravity = false;
        }

        this._lightsaberController.CanTransitionAttack = false;
    }

    public void StopAirAttack()
    {
        if (this._lightsaberController.CurrentlyPlayingAirAttack == this._lightsaberController.CurrentAirAttack)
        {
            this._lightsaberController.AirAttacking = false;
            this._lightsaberController.CurrentAirAttack = 0;
            this._lightsaberController.CurrentlyPlayingAirAttack = 0;
            this._lightsaberController.CanDealDamage(false);

            Rigidbody playerRigidbody = this._player.gameObject.GetComponent<Rigidbody>();

            playerRigidbody.useGravity = true;
        }

        this._lightsaberController.CanTransitionAttack = true;
    }

    public void ChangeHand(string hand)
    {
        this._lightsaberController.SetToHand(hand);
    }

    public void StartAttackRecovery(string attackType)
    {
        if (attackType == "heavy")
            this._lightsaberController.IsHeavyAttackRecovering = true;
        else
            this._lightsaberController.IsAttackRecovering = true;
    }

    public void EndAttackRecovery(string attackType)
    {
        if (attackType == "heavy")
        {
            this._lightsaberController.IsHeavyAttackRecovering = false;
        }
        else
        {
            this._lightsaberController.IsHeavyAttackRecovering = false;
            this._lightsaberController.IsAttackRecovering = false;
        }
    }

    #endregion

    #region Lightsaber

    public void TurnTrails(string activeState)
    {
        switch (activeState)
        {
            case "on":
                this._lightsaberController.TrailsTurner(true);

                break;
            case "off":
                this._lightsaberController.TrailsTurner(false);

                break;
            default:
                Debug.LogError("The given active state is wrong");

                break;
        }
    }

    #endregion
}

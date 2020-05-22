using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    private Player player;
    private PlayerMovement playerMovement;
    private AudioManager audioManager;
    private LightsaberController lightsaberController;
    private Animator animator;

    private bool wasPlayingJumpRumble;
    private bool wasArtificialGravityActivated;

    #region MonoMethods

    void Awake()
    {
        this.player = this.gameObject.GetComponent<Player>();
        this.playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        this.audioManager = FindObjectsOfType<AudioManager>()[0];
        this.lightsaberController = this.player.GetComponentInChildren<LightsaberController>();
        this.animator = this.player.gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!this.playerMovement.IsMoving() && this.audioManager.IsPlaying("RockWalk"))
        {
            this.audioManager.Stop("RockWalk");
        }

        if (!this.playerMovement.IsRunning() && this.audioManager.IsPlaying("RockSprint"))
        {
            this.audioManager.Stop("RockSprint");
        }

        if (this.playerMovement.IsInAir() && !this.audioManager.IsPlaying("ForceJumpRumble") && !this.playerMovement.Jumping && !this.playerMovement.Dashing)
        {
            this.audioManager.Play("ForceJumpRumble");
        }
        else if (!this.playerMovement.IsInAir() && this.audioManager.IsPlaying("ForceJumpRumble") && !this.playerMovement.Jumping)
        {
            this.audioManager.Stop("ForceJumpRumble");
        }
    }

    private void FixedUpdate()
    {
        if (this.lightsaberController.IsHeavyAttackRecovering &&
            !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Dual_HeavyAttack_End") &&
            !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Dual_HeavyAttack"))
        {
            this.lightsaberController.IsHeavyAttackRecovering = false;
        }

        if (this.lightsaberController.Attacking ||
            this.lightsaberController.AirAttacking ||
            this.lightsaberController.HeavyAttacking)
        {
            this.TurnTrails("on");
        }

        if (!this.lightsaberController.AirAttacking &&
            !this.player.gameObject.GetComponent<Rigidbody>().useGravity &&
            !this.playerMovement.Dashing)
        {
            this.player.gameObject.GetComponent<Rigidbody>().useGravity = true;
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
        if (this.playerMovement.IsRunning())
        {
            this.audioManager.Play("RockSprint");
        }
        else
        {
            this.audioManager.Play("RockWalk");
        }
    }

    #endregion

    #region Slide
    public void StartSliding()
    {
        this.playerMovement.IsSliding = true;
        this.playerMovement.CanStopSliding = true;

        this.playerMovement.MakeThemZeroWhenSliding();

        this.playerMovement.StartSlideResize();

        this.audioManager.Play("ForceDash");
    }

    public void EndSlide()
    {
        this.playerMovement.Slide = false;
        this.playerMovement.IsSliding = false;

        this.playerMovement.EndSlideResize();
    }

    #endregion

    #region Jump

    public void StartJump()
    {
        this.audioManager.Play("ForceJump");

        this.playerMovement.MakeTheJump();
    }

    public void StartJumping()
    {
        this.playerMovement.Jumping = true;

        this.audioManager.Play("ForceJumpRumble");
    }

    public void EndJump()
    {
        this.audioManager.Play("RockLand");
        this.audioManager.Stop("ForceJumpRumble");
    }

    #endregion

    #region Dash

    public void StartDash()
    {
        this.playerMovement.Dashing = true;
        this.playerMovement.CanDash = false;

        this.audioManager.Play("ForceDash");

        if (!this.player.ArtificialGravity)
        {
            this.playerMovement.HoldOnDash();
        }

        this.wasPlayingJumpRumble = this.audioManager.IsPlaying("ForceJumpRumble");
        this.audioManager.Stop("ForceJumpRumble");
    }

    public void EndDash()
    {
        if (this.playerMovement.Jumping && this.playerMovement.IsInAir())
        {
            this.playerMovement.SetInAir(false);
        }

        this.playerMovement.Dash = false;
        this.playerMovement.Dashing = false;

        this.playerMovement.ReloadDash();

        if (this.wasPlayingJumpRumble)
        {
            this.audioManager.Play("ForceJumpRumble");

            this.wasPlayingJumpRumble = false;
        }
    }

    #endregion

    #region Dodge

    public void StartDodge()
    {
        this.playerMovement.Dodging = true;
        this.playerMovement.CanDodge = false;
    }

    public void PlayDodgeSound()
    {
        this.audioManager.Play("RockRoll");
    }

    public void EndDodge()
    {
        this.playerMovement.Dodge = false;
        this.playerMovement.Dodging = false;

        this.playerMovement.ReloadDodge();
    }

    #endregion

    #region Attacks

    public void StartAttack()
    {
        this.lightsaberController.CurrentlyPlayingAttack = this.lightsaberController.CurrentAttack;
        this.lightsaberController.CanDealDamage(true);

        this.lightsaberController.CanTransitionAttack = false;
    }

    public void StopAttack()
    {
        if (this.lightsaberController.CurrentlyPlayingAttack == this.lightsaberController.CurrentAttack)
        {
            this.lightsaberController.Attacking = false;
            this.lightsaberController.CurrentAttack = 0;
            this.lightsaberController.CurrentlyPlayingAttack = 0;

            this.lightsaberController.CanDealDamage(false);
        }

        this.lightsaberController.CanTransitionAttack = true;
    }

    public void StartHeavyAttack()
    {
        this.lightsaberController.HeavyAttacking = true;
        this.lightsaberController.CanDealDamage(true);
    }

    public void StopHeavyAttack()
    {
        this.lightsaberController.HeavyAttacking = false;
        this.lightsaberController.CanDealDamage(false);
    }

    public void StartAirAttack()
    {
        this.lightsaberController.CurrentlyPlayingAirAttack = this.lightsaberController.CurrentAirAttack;
        this.lightsaberController.CanDealDamage(true);

        if (this.lightsaberController.CurrentlyPlayingAirAttack == 1)
        {
            Rigidbody playerRigidbody = this.player.gameObject.GetComponent<Rigidbody>();

            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.useGravity = false;
        }

        this.lightsaberController.CanTransitionAttack = false;
    }

    public void StopAirAttack()
    {
        if (this.lightsaberController.CurrentlyPlayingAirAttack == this.lightsaberController.CurrentAirAttack)
        {
            this.lightsaberController.AirAttacking = false;
            this.lightsaberController.CurrentAirAttack = 0;
            this.lightsaberController.CurrentlyPlayingAirAttack = 0;
            this.lightsaberController.CanDealDamage(false);

            Rigidbody playerRigidbody = this.player.gameObject.GetComponent<Rigidbody>();

            playerRigidbody.useGravity = true;
        }

        this.lightsaberController.CanTransitionAttack = true;
    }

    public void ChangeHand(string hand)
    {
        this.lightsaberController.SetToHand(hand);
    }

    public void StartAttackRecovery(string attackType)
    {
        if (attackType == "heavy")
            this.lightsaberController.IsHeavyAttackRecovering = true;
        else
            this.lightsaberController.IsAttackRecovering = true;
    }

    public void EndAttackRecovery(string attackType)
    {
        if (attackType == "heavy")
        {
            this.lightsaberController.IsHeavyAttackRecovering = false;
        }
        else
        {
            this.lightsaberController.IsHeavyAttackRecovering = false;
            this.lightsaberController.IsAttackRecovering = false;
        }
    }

    #endregion

    #region Lightsaber

    public void TurnTrails(string activeState)
    {
        switch (activeState)
        {
            case "on":
                this.lightsaberController.TrailsTurner(true);

                break;
            case "off":
                this.lightsaberController.TrailsTurner(false);

                break;
            default:
                Debug.LogError("The given active state is wrong");

                break;
        }
    }

    #endregion
}

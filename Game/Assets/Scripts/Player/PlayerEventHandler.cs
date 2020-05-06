using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    private Player player;
    private PlayerMovement playerMovement;
    private AudioManager audioManager;
    private LightsaberController lightsaberController;

    private bool wasPlayingJumpRumble;

    #region MonoMethods

    void Awake()
    {
        this.player = this.gameObject.GetComponent<Player>();
        this.playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        this.audioManager = FindObjectsOfType<AudioManager>()[0];
        this.lightsaberController = this.player.GetComponentInChildren<LightsaberController>();
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

        this.lightsaberController.CanTransitionAttack = false;

        if (this.playerMovement.Jumping || this.playerMovement.IsInAir())
        {
            Rigidbody rg = this.gameObject.GetComponent<Rigidbody>();

            rg.velocity = Vector3.zero;
            rg.useGravity = false;

            this.lightsaberController.AirAttack = true;
        }
    }

    public void StopAttack()
    {
        if (this.lightsaberController.CurrentlyPlayingAttack == this.lightsaberController.CurrentAttack)
        {
            this.lightsaberController.Attacking = false;
            this.lightsaberController.CurrentAttack = 0;
            this.lightsaberController.CurrentlyPlayingAttack = 0;

            if (this.playerMovement.Jumping || this.playerMovement.IsInAir())
            {
                Rigidbody rg = this.gameObject.GetComponent<Rigidbody>();

                rg.useGravity = true;

                this.lightsaberController.AirAttack = this.player.ArtificialGravity;
            }
        }

        this.lightsaberController.CanTransitionAttack = true;
    }

    public void StartHeavyAttack()
    {
        this.lightsaberController.HeavyAttacking = true;
    }

    public void StopHeavyAttack()
    {
        this.lightsaberController.HeavyAttacking = false;
    }

    public void ChangeHand(string hand)
    {
        this.lightsaberController.SetToHand(hand);
    }

    #endregion
}

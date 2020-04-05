using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private AudioManager audioManager;

    private bool move;
    private bool canMove;

    #region MonoMethods

    void Awake()
    {
        this.playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        this.audioManager = FindObjectsOfType<AudioManager>()[0];
    }

    private void Update()
    {
        if (!this.playerMovement.Jumping &&
            !this.playerMovement.Dashing &&
            !this.playerMovement.IsSliding &&
            !this.playerMovement.Dodging &&
            !this.playerMovement.IsInAir() &&
            !this.audioManager.IsPlaying("RockLand") &&
            this.move)
        {
            if (this.playerMovement.IsMoving() && !this.playerMovement.IsRunning() && !this.audioManager.IsPlaying("RockWalk"))
            {
                this.audioManager.Play("RockWalk");
            }
            else if (!this.playerMovement.IsMoving() && !this.playerMovement.IsRunning() && this.audioManager.IsPlaying("RockWalk"))
            {
                this.audioManager.Stop("RockWalk");
            }

            if (this.playerMovement.IsMoving() && this.playerMovement.IsRunning() && !this.audioManager.IsPlaying("RockSprint"))
            {
                this.audioManager.Play("RockSprint");
            }
            else if ((!this.playerMovement.IsMoving() ^ !this.playerMovement.IsRunning()) && this.audioManager.IsPlaying("RockSprint"))
            {
                this.audioManager.Stop("RockSprint");
            }
        }

        if (this.playerMovement.IsInAir() && !this.audioManager.IsPlaying("ForceJumpRumble") && !this.playerMovement.Jumping)
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

    public void StartMove()
    {
        this.move = true;
    }

    public void EndMove()
    {
        if (!this.playerMovement.IsMoving())
        {
            this.move = false;
        }
    }

    public void CanMove()
    {
        this.move = true;
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

        this.canMove = false;
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

        this.canMove = false;
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

        this.playerMovement.HoldOnDash();

        this.canMove = false;
    }

    public void EndDash()
    {
        this.playerMovement.Dash = false;
        this.playerMovement.Dashing = false;

        this.playerMovement.ReloadDash();
        this.CanMove();
    }

    #endregion

    #region Dodge

    public void StartDodge()
    {
        this.playerMovement.Dodging = true;
        this.playerMovement.CanDodge = false;

        this.canMove = false;
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

        this.move = true;
    }

    #endregion
}

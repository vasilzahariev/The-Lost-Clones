using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;

    #region MonoMethods

    void Awake()
    {
        this.playerMovement = this.gameObject.GetComponent<PlayerMovement>();
    }

    #endregion

    #region Slide
    public void StartSliding()
    {
        this.playerMovement.IsSliding = true;
        this.playerMovement.CanStopSliding = true;

        this.playerMovement.MakeThemZero();

        this.gameObject.GetComponent<CapsuleCollider>().height /= 2;
        this.gameObject.GetComponent<CapsuleCollider>().center /= 2;
    }

    public void EndSlide()
    {
        this.playerMovement.Slide = false;
        this.playerMovement.IsSliding = false;

        this.gameObject.GetComponent<CapsuleCollider>().height *= 2;
        this.gameObject.GetComponent<CapsuleCollider>().center *= 2;
    }

    #endregion

    #region Jump

    public void StartJump()
    {
        this.playerMovement.MakeTheJump();
    }

    public void StartJumping()
    {
        this.playerMovement.Jumping = true;
    }

    #endregion

    #region Dash

    public void StartDash()
    {
        this.playerMovement.Dashing = true;
        this.playerMovement.CanDash = false;

        this.playerMovement.HoldOnDash();
    }

    public void EndDash()
    {
        this.playerMovement.Dash = false;
        this.playerMovement.Dashing = false;

        this.playerMovement.ReloadDash();
    }

    #endregion

    #region WeaponEqipment

    public void EqiptDual()
    {
        //
    }

    #endregion
}

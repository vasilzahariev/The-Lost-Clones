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

    #region Dodge

    public void StartDodge()
    {
        this.playerMovement.Dodging = true;
    }

    public void EndDodge()
    {
        this.playerMovement.Dodge = false;
        this.playerMovement.Dodging = false;
    }

    #endregion
}

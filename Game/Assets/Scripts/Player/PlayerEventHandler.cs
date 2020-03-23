using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;

    #region MonoMethods

    void Start()
    {
        this.playerMovement = this.gameObject.GetComponent<PlayerMovement>();
    }

    #endregion

    // TODO: Make it so when the player is sliding the capsule collider becomes smaller or rotates
    #region Slide
    public void StartSliding()
    {
        this.playerMovement.IsSliding = true;
    }

    public void EndSlide()
    {
        this.playerMovement.Slide = false;
        this.playerMovement.IsSliding = false;
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
}

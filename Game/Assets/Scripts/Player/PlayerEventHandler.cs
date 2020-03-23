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

    // TODO: Make it so, when the player stops sliding under a block, he continues sliding
    #region Slide
    public void StartSliding()
    {
        this.playerMovement.IsSliding = true;

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
}

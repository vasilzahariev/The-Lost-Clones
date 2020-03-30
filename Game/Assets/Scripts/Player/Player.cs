using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Properties

    public Console Console;

    public bool IsTargetAcquired;

    [HideInInspector]
    public bool IsConsoleActive;

    #endregion

    #region Fields

    private Animator animator;

    private PlayerMovement playerMovement;

    #endregion

    #region MonoMethods

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        this.IsTargetAcquired = false;
        this.IsConsoleActive = false;

        this.animator = this.gameObject.GetComponent<Animator>();
        this.playerMovement = this.gameObject.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            this.IsConsoleActive = !this.IsConsoleActive;

            this.Console.gameObject.SetActive(this.IsConsoleActive);

            this.Console.Focus();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.IsTargetAcquired = !this.IsTargetAcquired;

            this.playerMovement.MakeThemZero();
        }
    }

    #endregion

    #region Methods

    private void AnimationParser()
    {
        this.animator.SetBool("IsTargetAcquired", this.IsTargetAcquired);
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{
    #region Properties

    public Camera Camera;
    public CinemachineFreeLook freeLook;
    public Console Console;
    public GameObject Lightsaber;
    public GameObject Target;

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

            if (this.IsConsoleActive)
            {
                this.Console.Focus();
            }
        }

        if (this.IsConsoleActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab) &&
            !this.playerMovement.Slide &&
            !this.playerMovement.Dashing &&
            !this.playerMovement.Dodging)
        {
            if (!this.IsTargetAcquired && this.CanGetTarget())
            {
                this.IsTargetAcquired = true;

                this.playerMovement.MakeThemZero();

                this.GetTarget();
                this.freeLook.gameObject.SetActive(false);
            }
            else if (this.IsTargetAcquired)
            {
                this.IsTargetAcquired = false;

                this.playerMovement.MakeThemZero();

                this.freeLook.gameObject.SetActive(true);
                this.Target = null;
            }
        }

        if (this.IsTargetAcquired)
        {
            this.Camera.transform.LookAt(this.Target.GetComponent<Renderer>().bounds.center);
        }

        //Debug.Log($"{this.IsTargetAcquired} {this.playerMovement.Dashing} {this.playerMovement.IsSliding}");
    }

    private void FixedUpdate()
    {
    }

    private void LateUpdate()
    {
        this.AnimationParser();
    }

    #endregion

    #region Methods

    private void AnimationParser()
    {
        this.animator.SetBool("Target", this.IsTargetAcquired);
    }

    private bool CanGetTarget()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.Camera.transform.position, this.Camera.transform.forward, out hit, 25f))
        {
            return true;
        }

        return false;
    }

    private void GetTarget()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.Camera.transform.position, this.Camera.transform.forward, out hit, 25f))
        {
            this.Target = hit.transform.gameObject;
        }
    }

    #endregion
}

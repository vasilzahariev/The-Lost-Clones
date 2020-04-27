using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

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

    [HideInInspector]
    public bool ArtificialGravity;

    [Range(0, 500)]
    public float Health;

    [Range(0, 500)]
    public float LightsaberStamina;

    #endregion

    #region Fields

    private Animator animator;
    private Rigidbody rg;

    private PlayerMovement playerMovement;

    private bool canUseArtificialGravity;

    #endregion

    #region MonoMethods

    void Start()
    {
        this.Health = 100f;

        Cursor.lockState = CursorLockMode.Locked;

        this.IsTargetAcquired = false;
        this.IsConsoleActive = false;

        this.ArtificialGravity = false;

        this.animator = this.gameObject.GetComponent<Animator>();
        this.rg = this.gameObject.GetComponent<Rigidbody>();
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

        if (Input.GetKeyDown(KeyCode.T) && this.canUseArtificialGravity)
        {
            this.rg.useGravity = !this.rg.useGravity;
            this.ArtificialGravity = !this.ArtificialGravity;
        }
    }

    private void FixedUpdate()
    {
        if (!this.canUseArtificialGravity && this.ArtificialGravity)
        {
            this.rg.useGravity = true;
            this.ArtificialGravity = false;
        }
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

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Space"))
            this.canUseArtificialGravity = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Space"))
            this.canUseArtificialGravity = false;
    }

    #endregion
}

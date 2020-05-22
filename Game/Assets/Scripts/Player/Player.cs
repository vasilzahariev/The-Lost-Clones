using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class Player : MonoBehaviour, IDamagable<float>
{
    #region Properties

    public Camera Camera;
    public CinemachineFreeLook freeLook;
    public Console Console;
    public GameObject Lightsaber;
    public GameObject Target;

    public Vector3 CameraOffset;

    [Range(0f, 1f)]
    public float T;

    public bool IsTargetAcquired;

    [HideInInspector]
    public bool IsConsoleActive;

    [HideInInspector]
    public bool ArtificialGravity;

    [Range(0f, 500f)]
    public float Health;

    [Range(0f, 500f)]
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
                this.LockOnTarget();
            }
            else if (this.IsTargetAcquired)
            {
                this.UnlockTarget();
            }
        }

        if (this.IsTargetAcquired)
        {
            this.LookAtTarget();
        }

        if (this.IsTargetAcquired && this.Target.GetComponent<IKillable>().isDead())
        {
            this.UnlockTarget();
        }

        Debug.Log(this.Health);

        //if (Input.GetKeyDown(KeyCode.T) && this.canUseArtificialGravity)
        //{
        //    this.rg.useGravity = !this.rg.useGravity;
        //    this.ArtificialGravity = !this.ArtificialGravity;
        //}
    }

    private void FixedUpdate()
    {
        if (!this.canUseArtificialGravity && this.ArtificialGravity)
        {
            this.rg.useGravity = true;
            this.ArtificialGravity = false;
        }

        if (this.canUseArtificialGravity && this.ArtificialGravity && !this.rg.useGravity)
        {
            this.rg.useGravity = true;
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
            if (hit.transform.gameObject.GetComponent<ITargetable>() == null)
            {
                return false;
            }

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

            if (this.Target.GetComponent<IKillable>().isDead())
            {
                this.Target = null;
            }
        }
    }

    private void LookAtTarget()
    {
        Transform target = this.Target.GetComponent<ITargetable>().GetLookAt();

        float distance = Vector3.Distance(this.transform.position, target.position);

        Vector3 newCameraPos = new Vector3(this.transform.position.x + distance,
                                           this.transform.position.y + distance / 2,
                                           this.transform.position.z + -distance);

        Quaternion newCameraRot = new Quaternion(this.transform.rotation.x,
                                                 this.transform.rotation.y,
                                                 this.transform.rotation.z,
                                                 this.transform.rotation.w);


        Quaternion playerRot = this.transform.rotation;

        this.transform.LookAt(target);

        this.transform.rotation = new Quaternion(0f,
                                                 this.transform.rotation.y,
                                                 0f,
                                                 this.transform.rotation.w);
        this.Camera.transform.LookAt(target);
    }

    private void LockOnTarget()
    {
        this.GetTarget();

        if (this.Target == null)
            return;

        this.IsTargetAcquired = true;

        this.playerMovement.MakeThemZero();


        this.freeLook.gameObject.SetActive(false);

        Transform target = this.Target.GetComponent<ITargetable>().GetLookAt();

        Quaternion playerRot = this.transform.rotation;

        this.transform.LookAt(target);

        this.transform.rotation = new Quaternion(0f,
                                                 this.transform.rotation.y,
                                                 0f,
                                                 this.transform.rotation.w);

        float distance = Vector3.Distance(this.transform.position, target.position);

        Vector3 newCameraPos = this.freeLook.transform.position;

        this.Camera.transform.position = newCameraPos;
    }

    private void UnlockTarget()
    {
        this.IsTargetAcquired = false;

        this.playerMovement.MakeThemZero();

        this.Target = null;

        Vector3 cameraPos = this.Camera.transform.position;

        this.freeLook.gameObject.SetActive(true);

        this.freeLook.transform.position = cameraPos;
    }

    #endregion

    #region InterfaceMethods

    public void TakeDamage(float damage)
    {
        this.Health -= damage;
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

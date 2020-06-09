using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class Player : MonoBehaviour, IDamagable<float>, IShootable
{
    #region Properties

    public GameObject Target { get; private set; }

    public bool IsTargetAcquired { get; private set; }

    public bool IsConsoleActive { get; private set; }

    public bool ArtificialGravity { get; private set; }

    #endregion

    #region Fields

    [Header("Player Settings")]
    [Range(0f, 500f)]
    public float Health;

    [Range(0f, 500f)]
    public float LightsaberStamina;

    [Header("Camera Elements")]
    public Camera Camera;
    public CinemachineFreeLook freeLook;
    public Vector3 CameraOffset;

    [Range(0f, 1f)]
    public float T;
    
    [Header("UI Elements")]
    public Console Console;

    [Header("Lightsaber")]
    public GameObject Lightsaber;

    private Animator _animator;
    private Rigidbody _rg;
    private Transform _shootAt;

    private PlayerMovement _playerMovement;
    private LightsaberController _lightsaberController;

    private bool _canUseArtificialGravity;

    #endregion

    #region MonoMethods

    void Awake()
    {
        this.Health = 100f;

        Cursor.lockState = CursorLockMode.Locked;

        this.IsTargetAcquired = false;
        this.IsConsoleActive = false;

        this.ArtificialGravity = false;

        this._animator = this.gameObject.GetComponent<Animator>();
        this._rg = this.gameObject.GetComponent<Rigidbody>();
        this._shootAt = UnityHelper.GetChildWithName(this.gameObject, "ShootAt").transform;

        this._playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        this._lightsaberController = this.gameObject.GetComponentInChildren<LightsaberController>();
    }

    void Update()
    {
        if (this.IsConsoleActive)
        {
            return;
        }

        if (this.IsTargetAcquired)
        {
            this.LookAtTarget();
        }

        if (this.IsTargetAcquired && this.Target.GetComponent<IKillable>().isDead())
        {
            this.UnlockTarget();
        }
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
        this._animator.SetBool("Target", this.IsTargetAcquired);
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

        this._playerMovement.MakeThemZero();


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

        this._playerMovement.MakeThemZero();

        this.Target = null;

        Vector3 cameraPos = this.Camera.transform.position;

        this.freeLook.gameObject.SetActive(true);

        this.freeLook.transform.position = cameraPos;
    }

    public void TakeConsoleInput()
    {
        this.IsConsoleActive = !this.IsConsoleActive;

        this.Console.gameObject.SetActive(this.IsConsoleActive);

        if (this.IsConsoleActive)
        {
            this.Console.Focus();
        }
    }

    public void TakeTargetInput()
    {
        if (!this._playerMovement.Slide &&
            !this._playerMovement.Dashing &&
            !this._playerMovement.Dodging)
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
    }

    public LightsaberController GetLightsaberController()
    {
        return this._lightsaberController;
    }

    #endregion

    #region InterfaceMethods

    public void TakeDamage(float damage)
    {
        if (!this._lightsaberController.IsBlocking)
            this.Health -= damage;
    }

    public Transform GetShootAt()
    {
        return this._shootAt;
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberController : MonoBehaviour
{
    #region Consts

    const int BASICCOMBOANIMCOUNT = 3;
    const int AIRCOMBOANIMCOUNT = 2;

    #endregion

    #region Properties

    public GameObject LeftHand;
    public GameObject RightHand;

    public Transform LeftHandTransform;
    public Transform RightHandTransform;

    [HideInInspector]
    public bool Attacking;

    [HideInInspector]
    public int CurrentAttack;

    [HideInInspector]
    public int CurrentlyPlayingAttack;

    [HideInInspector]
    public bool CanTransitionAttack;

    [HideInInspector]
    public bool HeavyAttacking;

    [HideInInspector]
    public bool AirAttack;

    #endregion

    #region Fields

    private Lightsaber firstLightsaber;
    private Lightsaber secondLightsaber;

    private Player player;
    private PlayerMovement playerMovement;

    private Animator animator;

    private bool shouldExecuteAHeavyAttack;

    #endregion

    #region MonoMethods

    void Start()
    {
        this.firstLightsaber = this.gameObject.GetComponentsInChildren<Lightsaber>()[0];
        this.secondLightsaber = this.gameObject.GetComponentsInChildren<Lightsaber>()[1];

        this.firstLightsaber.Damage = 30f;
        this.secondLightsaber.Damage = 20f;

        this.player = Object.FindObjectOfType<Player>();
        this.playerMovement = this.player.gameObject.GetComponent<PlayerMovement>();

        this.animator = this.player.GetComponent<Animator>();

        this.Attacking = false;
    }

    void Update()
    {
        if (this.player.IsConsoleActive)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) &&
            this.player.LightsaberStamina > 0f &&
            this.CurrentAttack < this.CurrentlyPlayingAttack + 1 &&
            this.CurrentAttack < (this.AirAttack ? AIRCOMBOANIMCOUNT : BASICCOMBOANIMCOUNT) &&
            !this.playerMovement.Dashing &&
            !this.HeavyAttacking &&
            !this.shouldExecuteAHeavyAttack)
        {
            this.CurrentAttack++;
        }

        if (Input.GetMouseButtonDown(2) &&
            this.player.LightsaberStamina > 0f &&
            this.CurrentAttack == 0 &&
            !this.HeavyAttacking &&
            !this.Attacking &&
            !this.playerMovement.IsInAir() &&
            !this.playerMovement.Jumping &&
            !this.playerMovement.Dashing)
        {
            this.shouldExecuteAHeavyAttack = true;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            this.firstLightsaber.TurnTheBaldeOff();
            this.secondLightsaber.TurnTheBaldeOff();
        }

        Debug.Log($"{this.CurrentAttack} {this.CurrentlyPlayingAttack}");
    }

    private void FixedUpdate()
    {
        if (this.CurrentAttack == 1 && this.CurrentlyPlayingAttack == 0)
        {
            this.CanTransitionAttack = true;
        }

        if (this.CurrentAttack != 0)
        {
            this.Attacking = true;
        }

        if (this.HeavyAttacking)
        {
            this.shouldExecuteAHeavyAttack = false;
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
        this.animator.SetBool("IsAttacking", this.Attacking);
        this.animator.SetInteger("Attack", this.CurrentAttack);
        this.animator.SetBool("CanTransitionAttack", this.CanTransitionAttack);
        this.animator.SetBool("ShouldExecuteAHeavyAttack", this.shouldExecuteAHeavyAttack);
        this.animator.SetBool("HeavyAttacking", this.HeavyAttacking);
    }

    public void MakeThemZero()
    {
        this.Attacking = false;
        this.CurrentAttack = 0;
        this.CurrentlyPlayingAttack = 0;
    }

    public void SetToHand(string hand)
    {
        if (hand == "left")
        {
            this.gameObject.transform.parent = this.LeftHand.transform;

            this.gameObject.transform.position = this.LeftHandTransform.position;
            this.gameObject.transform.rotation = this.LeftHandTransform.rotation;
        }
        else if (hand == "right")
        {
            this.gameObject.transform.parent = this.RightHand.transform;

            this.gameObject.transform.position = this.RightHandTransform.position;
            this.gameObject.transform.rotation = this.RightHandTransform.rotation;
        }
    }

    #endregion

    #region Collision
    #endregion
}

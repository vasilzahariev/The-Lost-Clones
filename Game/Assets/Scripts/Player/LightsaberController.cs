using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberController : MonoBehaviour
{
    #region Consts

    const int BASICCOMBOANIMCOUNT = 3;
    const int AIRCOMBOANIMCOUNT = 2;
    const float BLOCKRECOVERYSECONDS = 0.3f;

    #endregion

    #region Properties

    [Header("Left Hand Information")]
    public GameObject LeftHand;
    public Transform LeftHandTransform;

    [Header("Right Hand Information")]
    public GameObject RightHand;
    public Transform RightHandTransform;

    [HideInInspector]
    public bool Attacking;

    [HideInInspector]
    public int CurrentAttack;

    [HideInInspector]
    public int CurrentlyPlayingAttack;

    [HideInInspector]
    public int CurrentAirAttack;

    [HideInInspector]
    public int CurrentlyPlayingAirAttack;

    [HideInInspector]
    public bool CanTransitionAttack;

    [HideInInspector]
    public bool HeavyAttacking;

    [HideInInspector]
    public bool AirAttacking;

    [HideInInspector]
    public bool IsAttackRecovering;

    [HideInInspector]
    public bool IsHeavyAttackRecovering;

    [HideInInspector]
    public bool IsBlocking;

    #endregion

    #region Fields

    private Lightsaber firstLightsaber;
    private Lightsaber secondLightsaber;

    private Player player;
    private PlayerMovement playerMovement;

    private Animator animator;

    private bool shouldExecuteAHeavyAttack;
    private bool canBlock = true;

    #endregion

    #region MonoMethods

    void Start()
    {
        this.firstLightsaber = this.gameObject.GetComponentsInChildren<Lightsaber>()[0];
        this.secondLightsaber = this.gameObject.GetComponentsInChildren<Lightsaber>()[1];

        this.firstLightsaber.Damage = 20f;
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

        if (Input.GetMouseButton(1) && !this.IsBlocking)
        {
            this.TakeBlockingInput(true);
        }

        

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    this.firstLightsaber.TurnTheBaldeOff();
        //    this.secondLightsaber.TurnTheBaldeOff();
        //}

        //Debug.Log($"{this.IsBlocking} {this.canBlock}");
    }

    private void FixedUpdate()
    {
        if (this.AirAttacking &&
            !this.playerMovement.Jumping &&
            !this.playerMovement.IsInAir() &&
            this.CurrentAirAttack != 0)
        {
            this.CurrentAirAttack = 0;
            this.CurrentlyPlayingAirAttack = 0;
            this.AirAttacking = false;
        }

        if (((this.Attacking &&
                    this.CurrentAttack != 0) ||
                (this.shouldExecuteAHeavyAttack ||
            this.HeavyAttacking)) &&
            (this.playerMovement.Jumping ||
            this.playerMovement.IsInAir()))
        {
            this.CurrentAttack = 0;
            this.CurrentlyPlayingAttack = 0;
            this.Attacking = false;
        }


        if (this.CurrentAttack == 1 && this.CurrentlyPlayingAttack == 0)
        {
            this.CanTransitionAttack = true;
        }

        if (this.CurrentAttack != 0)
        {
            this.Attacking = true;
        }

        if (this.CurrentAirAttack != 0)
        {
            this.AirAttacking = true;
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
        this.animator.SetInteger("AirAttack", this.CurrentAirAttack);
        this.animator.SetBool("IsAirAttacking", this.AirAttacking);
        this.animator.SetBool("CanTransitionAttack", this.CanTransitionAttack);
        this.animator.SetBool("ShouldExecuteAHeavyAttack", this.shouldExecuteAHeavyAttack);
        this.animator.SetBool("HeavyAttacking", this.HeavyAttacking);

        this.animator.SetBool("IsBlocking", this.IsBlocking);
    }

    private IEnumerator WaitBeforeBeingAbleToBlockAgain()
    {
        yield return new WaitForSecondsRealtime(BLOCKRECOVERYSECONDS);

        this.canBlock = true;
    }

    private void BasicAttacksInput()
    {
        if (!this.playerMovement.IsInAir() &&
            !this.playerMovement.Jumping &&
            this.player.LightsaberStamina > 0f &&
            this.CurrentAttack < this.CurrentlyPlayingAttack + 1 &&
            this.CurrentAttack < BASICCOMBOANIMCOUNT &&
            !this.playerMovement.Dashing &&
            !this.HeavyAttacking &&
            !this.shouldExecuteAHeavyAttack &&
            !this.IsHeavyAttackRecovering &&
            !this.playerMovement.IsSliding &&
            !this.playerMovement.Dodging &&
            !this.IsBlocking)
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
        !this.playerMovement.Dashing &&
        !this.IsHeavyAttackRecovering)
        {
            this.shouldExecuteAHeavyAttack = true;
        }
    }

    private void AirAttacksInput()
    {
        if ((this.playerMovement.IsInAir() ||
            this.playerMovement.Jumping) &&
            this.player.LightsaberStamina > 0f &&
            this.CurrentAirAttack < this.CurrentlyPlayingAirAttack + 1 &&
            this.CurrentAirAttack < AIRCOMBOANIMCOUNT &&
            !this.playerMovement.Dashing &&
            !this.HeavyAttacking &&
            !this.shouldExecuteAHeavyAttack &&
            !this.IsHeavyAttackRecovering &&
            !this.playerMovement.IsSliding &&
            !this.playerMovement.Dodging &&
            !this.IsBlocking)
        {
            this.CurrentAirAttack++;
        }
    }

    private void HeavyAttackInput()
    {
        if (this.player.LightsaberStamina > 0f &&
            this.CurrentAttack == 0 &&
            !this.HeavyAttacking &&
            !this.Attacking &&
            !this.playerMovement.IsInAir() &&
            !this.playerMovement.Jumping &&
            !this.playerMovement.Dashing &&
            !this.IsHeavyAttackRecovering &&
            !this.playerMovement.IsSliding &&
            !this.playerMovement.Dodging &&
            !this.IsBlocking)
        {
            this.shouldExecuteAHeavyAttack = true;
        }
    }

    private void StartBlockingInput()
    {
        if (this.canBlock &&
            !this.Attacking &&
            !this.HeavyAttacking &&
            !this.AirAttacking)
        {
            this.IsBlocking = true;
        }
    }

    private void EndBlockingInput()
    {
        this.IsBlocking = false;
        this.canBlock = false;

        StartCoroutine(this.WaitBeforeBeingAbleToBlockAgain());
    }

    public void TakeAttacksInput()
    {
        this.BasicAttacksInput();
        this.AirAttacksInput();
    }

    public void TakeHeavyAttackInput()
    {
        this.HeavyAttackInput();
    }

    public void TakeBlockingInput(bool value)
    {
        if (value)
            this.StartBlockingInput();
        else
            this.EndBlockingInput();
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

    public void TrailsTurner(bool activeState)
    {
        this.firstLightsaber.TrailsOnOff(activeState);
        this.secondLightsaber.TrailsOnOff(activeState);
    }

    public void CanDealDamage(bool canDealDamage)
    {
        this.firstLightsaber.SetCanDealDamage(canDealDamage);
        this.secondLightsaber.SetCanDealDamage(canDealDamage);
    }

    #endregion

    #region Collision

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberController : MonoBehaviour
{
    #region Consts

    const int BASIC_COMBO_ANIM_COUNT = 3;
    const int AIR_COMBO_ANIM_COUNT = 2;
    const float BLOCK_RECOVERY_SECONDS = 0.4f;
    public const float TIME_TO_PARRY = 0.3f;

    #endregion

    #region Properties

    /// <summary>
    /// TODO: Do some changes, so that you get the hand information in code
    /// </summary>

    [Header("Left Hand Information")]
    public GameObject LeftHand;
    public Transform LeftHandTransform;

    [Header("Right Hand Information")]
    public GameObject RightHand;
    public Transform RightHandTransform;

    public bool Attacking { get; set; }

    public int CurrentAttack { get; set; }

    public int CurrentlyPlayingAttack { get; set; }

    public int CurrentAirAttack { get; set; }

    public int CurrentlyPlayingAirAttack { get; set; }

    public bool CanTransitionAttack { get; set; }

    public bool HeavyAttacking { get; set; }

    public bool AirAttacking { get; set; }

    public bool IsAttackRecovering { get; set; }

    public bool IsHeavyAttackRecovering { get; set; }

    public bool IsBlocking { get; private set; }

    public float BlockingStarTime { get; private set; }

    #endregion

    #region Fields

    private Lightsaber _firstLightsaber;
    private Lightsaber _secondLightsaber;

    private Player _player;
    private PlayerMovement _playerMovement;

    private Animator _animator;

    private bool _shouldExecuteAHeavyAttack;
    private bool _canBlock = true;

    #endregion

    #region MonoMethods

    void Start()
    {
        this._firstLightsaber = this.gameObject.GetComponentsInChildren<Lightsaber>()[0];
        this._secondLightsaber = this.gameObject.GetComponentsInChildren<Lightsaber>()[1];

        this._firstLightsaber.Damage = 20f;
        this._secondLightsaber.Damage = 20f;

        this._player = Object.FindObjectOfType<Player>();
        this._playerMovement = this._player.gameObject.GetComponent<PlayerMovement>();

        this._animator = this._player.GetComponent<Animator>();

        this.Attacking = false;
    }

    void Update()
    {
        if (this._player.IsConsoleActive)
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
            !this._playerMovement.Jumping &&
            !this._playerMovement.IsInAir() &&
            this.CurrentAirAttack != 0)
        {
            this.CurrentAirAttack = 0;
            this.CurrentlyPlayingAirAttack = 0;
            this.AirAttacking = false;
        }

        if (((this.Attacking &&
                    this.CurrentAttack != 0) ||
                (this._shouldExecuteAHeavyAttack ||
            this.HeavyAttacking)) &&
            (this._playerMovement.Jumping ||
            this._playerMovement.IsInAir()))
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
            this._shouldExecuteAHeavyAttack = false;
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
        this._animator.SetBool("IsAttacking", this.Attacking);
        this._animator.SetInteger("Attack", this.CurrentAttack);
        this._animator.SetInteger("AirAttack", this.CurrentAirAttack);
        this._animator.SetBool("IsAirAttacking", this.AirAttacking);
        this._animator.SetBool("CanTransitionAttack", this.CanTransitionAttack);
        this._animator.SetBool("ShouldExecuteAHeavyAttack", this._shouldExecuteAHeavyAttack);
        this._animator.SetBool("HeavyAttacking", this.HeavyAttacking);

        this._animator.SetBool("IsBlocking", this.IsBlocking);
    }

    private IEnumerator WaitBeforeBeingAbleToBlockAgain()
    {
        yield return new WaitForSecondsRealtime(BLOCK_RECOVERY_SECONDS);

        this._canBlock = true;
    }

    private void BasicAttacksInput()
    {
        if (!this._playerMovement.IsInAir() &&
            !this._playerMovement.Jumping &&
            this._player.LightsaberStamina > 0f &&
            this.CurrentAttack < this.CurrentlyPlayingAttack + 1 &&
            this.CurrentAttack < BASIC_COMBO_ANIM_COUNT &&
            !this._playerMovement.Dashing &&
            !this.HeavyAttacking &&
            !this._shouldExecuteAHeavyAttack &&
            !this.IsHeavyAttackRecovering &&
            !this._playerMovement.IsSliding &&
            !this._playerMovement.Dodging &&
            !this.IsBlocking)
        {
            this.CurrentAttack++;
        }

        if (Input.GetMouseButtonDown(2) &&
        this._player.LightsaberStamina > 0f &&
        this.CurrentAttack == 0 &&
        !this.HeavyAttacking &&
        !this.Attacking &&
        !this._playerMovement.IsInAir() &&
        !this._playerMovement.Jumping &&
        !this._playerMovement.Dashing &&
        !this.IsHeavyAttackRecovering)
        {
            this._shouldExecuteAHeavyAttack = true;
        }
    }

    private void AirAttacksInput()
    {
        if ((this._playerMovement.IsInAir() ||
            this._playerMovement.Jumping) &&
            this._player.LightsaberStamina > 0f &&
            this.CurrentAirAttack < this.CurrentlyPlayingAirAttack + 1 &&
            this.CurrentAirAttack < AIR_COMBO_ANIM_COUNT &&
            !this._playerMovement.Dashing &&
            !this.HeavyAttacking &&
            !this._shouldExecuteAHeavyAttack &&
            !this.IsHeavyAttackRecovering &&
            !this._playerMovement.IsSliding &&
            !this._playerMovement.Dodging &&
            !this.IsBlocking)
        {
            this.CurrentAirAttack++;
        }
    }

    private void HeavyAttackInput()
    {
        if (this._player.LightsaberStamina > 0f &&
            this.CurrentAttack == 0 &&
            !this.HeavyAttacking &&
            !this.Attacking &&
            !this._playerMovement.IsInAir() &&
            !this._playerMovement.Jumping &&
            !this._playerMovement.Dashing &&
            !this.IsHeavyAttackRecovering &&
            !this._playerMovement.IsSliding &&
            !this._playerMovement.Dodging &&
            !this.IsBlocking)
        {
            this._shouldExecuteAHeavyAttack = true;
        }
    }

    private void StartBlockingInput()
    {
        if (this._canBlock &&
            !this.Attacking &&
            !this.HeavyAttacking &&
            !this.AirAttacking)
        {
            this.IsBlocking = true;
            this.BlockingStarTime = Time.time;
        }
    }

    private void EndBlockingInput()
    {
        this.IsBlocking = false;
        this._canBlock = false;

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
        if (this._playerMovement.Dashing)
            return;

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
        this._firstLightsaber.TrailsOnOff(activeState);
        this._secondLightsaber.TrailsOnOff(activeState);
    }

    public void CanDealDamage(bool canDealDamage)
    {
        this._firstLightsaber.SetCanDealDamage(canDealDamage);
        this._secondLightsaber.SetCanDealDamage(canDealDamage);
    }

    #endregion

    #region Collision

    #endregion
}

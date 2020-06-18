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
    const float RUN_ATTACK_COOLDOWN = 1.5f;

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

    public bool ShouldExecuteStealthKill { get; set; }

    public bool StealthKilling { get; set; }

    public bool FailedStealthKill { get; set; }

    public bool RunForwardAttacking { get; set; }

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
    private bool _canRunAttack = true;

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
        _animator.SetBool("IsAttacking", this.Attacking);
        _animator.SetInteger("Attack", this.CurrentAttack);
        _animator.SetInteger("AirAttack", this.CurrentAirAttack);
        _animator.SetBool("IsAirAttacking", this.AirAttacking);
        _animator.SetBool("CanTransitionAttack", this.CanTransitionAttack);
        _animator.SetBool("ShouldExecuteAHeavyAttack", this._shouldExecuteAHeavyAttack);
        _animator.SetBool("HeavyAttacking", this.HeavyAttacking);
        _animator.SetBool("ShouldExecuteStealthKill", this.ShouldExecuteStealthKill);
        _animator.SetBool("FailedStealthKill", this.FailedStealthKill);
        _animator.SetBool("RunForwardAttacking", this.RunForwardAttacking);

        this._animator.SetBool("IsBlocking", this.IsBlocking);
    }

    private IEnumerator WaitBeforeBeingAbleToBlockAgain()
    {
        yield return new WaitForSecondsRealtime(BLOCK_RECOVERY_SECONDS);

        this._canBlock = true;
    }

    private IEnumerator WaitRunAttackCooldown()
    {
        yield return new WaitForSecondsRealtime(RUN_ATTACK_COOLDOWN);
        _canRunAttack = true;
    }

    private void StealthKillInput()
    {
        if (_player.Target != null &&
            _player.Target.GetComponent<Enemy>() != null &&
            _player.Target.GetComponent<Enemy>().Target != _player.gameObject &&
            Vector3.Distance(this.transform.position, _player.Target.transform.position) <= 2.5f &&
            !_playerMovement.IsInAir() &&
            !_playerMovement.Jumping &&
            !this.ShouldExecuteStealthKill &&
            !this.FailedStealthKill &&
            !this.Attacking &&
            !this.AirAttacking &&
            !this.IsBlocking &&
            !_shouldExecuteAHeavyAttack &&
            !this.HeavyAttacking &&
            !_playerMovement.Dodging &&
            !_playerMovement.IsInAir() &&
            !_playerMovement.Jumping &&
            !_playerMovement.IsSliding &&
            !this.RunForwardAttacking)
        {
            Enemy enemy = _player.Target.GetComponent<Enemy>();

            this.ShouldExecuteStealthKill = true;

            enemy.CanDie = false;
            enemy.IsStealthKilled = true;

            enemy.transform.rotation = _player.transform.rotation;

            float distance = Vector3.Distance(this.transform.position, _player.Target.transform.position);

            if (distance > 1f)
            {
                float distanceToTravel = distance - 1f;

                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position,
                                                               _player.transform.position,
                                                               distanceToTravel);
            }

            _player.UnlockTarget();
        }
        
        if (_player.Target != null &&
            _player.Target.GetComponent<Enemy>() != null &&
            _player.Target.GetComponent<Enemy>().Target != _player.gameObject &&
            Vector3.Distance(this.transform.position, _player.Target.transform.position) > 2.5f &&
            Vector3.Distance(this.transform.position, _player.Target.transform.position) < 3f &&
            !_playerMovement.IsInAir() &&
            !_playerMovement.Jumping &&
            !this.ShouldExecuteStealthKill &&
            !this.Attacking &&
            !this.AirAttacking &&
            !this.IsBlocking &&
            !_shouldExecuteAHeavyAttack &&
            !this.HeavyAttacking &&
            !_playerMovement.Dodging &&
            !_playerMovement.IsInAir() &&
            !_playerMovement.Jumping &&
            !_playerMovement.IsSliding &&
            !this.RunForwardAttacking)
        {
            this.FailedStealthKill = true;
        }
    }

    private void RunAttacksInput()
    {
        if (_playerMovement.IsRunning() &&
            _playerMovement.IsMovingForward() &&
            !this.RunForwardAttacking &&
            _canRunAttack &&
            !_playerMovement.Jumping &&
            !_playerMovement.IsInAir() &&
            !this.ShouldExecuteStealthKill &&
            !this.StealthKilling &&
            !this.FailedStealthKill &&
            !this.Attacking &&
            !this.AirAttacking &&
            !this.IsBlocking &&
            !_shouldExecuteAHeavyAttack &&
            !this.HeavyAttacking &&
            !_playerMovement.Dodging &&
            !_playerMovement.IsSliding)
        {
            this.RunForwardAttacking = true;
        }
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
            !this.IsBlocking &&
            !this.ShouldExecuteStealthKill &&
            !this.FailedStealthKill &&
            !this.RunForwardAttacking)
        {
            this.CurrentAttack++;
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
            !this.IsBlocking &&
            !this.ShouldExecuteStealthKill &&
            !this.FailedStealthKill &&
            !this.RunForwardAttacking)
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
            !this.IsBlocking &&
            !this.ShouldExecuteStealthKill &&
            !this.FailedStealthKill &&
            !this.RunForwardAttacking)
        {
            this._shouldExecuteAHeavyAttack = true;
        }
    }

    private void StartBlockingInput()
    {
        if (this._canBlock &&
            !this.Attacking &&
            !this.HeavyAttacking &&
            !this.IsHeavyAttackRecovering &&
            !this.AirAttacking &&
            !this.ShouldExecuteStealthKill &&
            !this.FailedStealthKill &&
            !this.RunForwardAttacking)
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
        this.AirAttacksInput();
        this.StealthKillInput();
        this.RunAttacksInput();
        this.BasicAttacksInput();
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

    public void RunAttackCooldown()
    {
        _canRunAttack = false;
        StartCoroutine(this.WaitRunAttackCooldown());
    }

    #endregion

    #region Collision

    #endregion
}

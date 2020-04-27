using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberController : MonoBehaviour
{
    #region Properties

    [HideInInspector]
    public bool Attacking;

    [HideInInspector]
    public int CurrentAttack;

    [HideInInspector]
    public int CurrentlyPlayingAttack;

    [HideInInspector]
    public bool CanTransitionAttack;

    #endregion

    #region Fields

    private Lightsaber firstLightsaber;
    private Lightsaber secondLightsaber;

    private Player player;
    private PlayerMovement playerMovement;

    private Animator animator;

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
            this.player.LightsaberStamina > 0 &&
            this.CurrentAttack < this.CurrentlyPlayingAttack + 1 &&
            this.CurrentAttack < 4 &&
            !this.playerMovement.IsInAir() &&
            !this.playerMovement.Dashing)
        {
            this.CurrentAttack++;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            this.firstLightsaber.TurnTheBaldeOff();
            this.secondLightsaber.TurnTheBaldeOff();
        }
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
    }

    public void MakeThemZero()
    {
        this.Attacking = false;
        this.CurrentAttack = 0;
        this.CurrentlyPlayingAttack = 0;
    }

    #endregion

    #region Collision
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields

    private Player player;
    private PlayerMovement playerMovement;
    private LightsaberController lightsaberController;

    private PlayerInput input;

    #endregion

    #region MonoMethods

    private void OnEnable()
    {
        this.input.Enable();
    }

    private void Awake()
    {
        this.input = new PlayerInput();

        this.player = this.gameObject.GetComponent<Player>();
        this.playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        this.lightsaberController = this.gameObject.GetComponentInChildren<LightsaberController>();

        this.InputTaker();
    }

    private void OnDisable()
    {
        this.input.Disable();
    }

    #endregion

    #region Methods

    private void InputTaker()
    {
        this.input.Player.Movement.performed += ctx => this.playerMovement.TakeMovementInput(ctx.ReadValue<Vector2>());
        this.input.Player.Run.started += ctx => this.playerMovement.TakeRunInput(true);
        this.input.Player.Run.performed += ctx => this.playerMovement.TakeRunInput(true);
        this.input.Player.Run.canceled += ctx => this.playerMovement.TakeRunInput(false);
        this.input.Player.Slide.performed += ctx => this.playerMovement.TakeSlideInput();
        this.input.Player.Dash.performed += ctx => this.playerMovement.TakeDashInput();
        this.input.Player.Roll.performed += ctx => this.playerMovement.TakeRollInput();
        this.input.Player.Jump.performed += ctx => this.playerMovement.TakeJumpInput();

        this.input.Player.Console.performed += ctx => this.player.TakeConsoleInput();
        this.input.Player.Target.performed += ctx => this.player.TakeTargetInput();

        this.input.Player.Block.started += ctx => this.lightsaberController.TakeBlockingInput(true);
        this.input.Player.Block.canceled += ctx => this.lightsaberController.TakeBlockingInput(false);
        this.input.Player.Attack.performed += ctx => this.lightsaberController.TakeAttacksInput();
        this.input.Player.HeavyAttack.performed += ctx => this.lightsaberController.TakeHeavyAttackInput();
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields

    private Player _player;
    private PlayerMovement _playerMovement;
    private LightsaberController _lightsaberController;

    private PlayerInput _input;

    #endregion

    #region MonoMethods

    private void OnEnable()
    {
        this._input.Enable();
    }

    private void Awake()
    {
        this._input = new PlayerInput();

        this._player = this.gameObject.GetComponent<Player>();
        this._playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        this._lightsaberController = this.gameObject.GetComponentInChildren<LightsaberController>();

        this.InputTaker();
    }

    private void OnDisable()
    {
        this._input.Disable();
    }

    #endregion

    #region Methods

    private void InputTaker()
    {
        this._input.Player.Movement.performed += ctx => this._playerMovement.TakeMovementInput(ctx.ReadValue<Vector2>());
        this._input.Player.Run.started += ctx => this._playerMovement.TakeRunInput(true);
        this._input.Player.Run.performed += ctx => this._playerMovement.TakeRunInput(true);
        this._input.Player.Run.canceled += ctx => this._playerMovement.TakeRunInput(false);
        this._input.Player.Slide.performed += ctx => this._playerMovement.TakeSlideInput();
        this._input.Player.Dash.performed += ctx => this._playerMovement.TakeDashInput();
        this._input.Player.Roll.performed += ctx => this._playerMovement.TakeRollInput();
        this._input.Player.Jump.performed += ctx => this._playerMovement.TakeJumpInput();

        this._input.Player.Console.performed += ctx => this._player.TakeConsoleInput();
        this._input.Player.Target.performed += ctx => this._player.TakeTargetInput();

        this._input.Player.Block.started += ctx => this._lightsaberController.TakeBlockingInput(true);
        this._input.Player.Block.canceled += ctx => this._lightsaberController.TakeBlockingInput(false);
        this._input.Player.Attack.performed += ctx => this._lightsaberController.TakeAttacksInput();
        this._input.Player.HeavyAttack.performed += ctx => this._lightsaberController.TakeHeavyAttackInput();
    }

    #endregion
}

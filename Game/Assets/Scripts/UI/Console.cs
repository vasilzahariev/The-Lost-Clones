using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject _playerObj;

    private GameObject _canvas; // The canvas that holds all UI elements
    private TMP_InputField _consoleInput; // The Console Input Field
    private TMP_Text _consoleOutput; // The Console Output Text
    private TMP_Text _FPS; // The FPS object

    private Player _player;
    private PlayerMovement _playerMovement;

    private List<string> _commands; // A list of all the available commands
    private List<string> _setableVariables; // A list of all setable variables
    private List<string> _previousInputs; // A list of all the previous inputs

    private int _page; // Current page
    private int _pages; // Pages count
    private int _inputIndex; // Current input (because I save the previous inputs and I also have to save the current one, if you deside to go to a previous input, but maybe then come back)

    #endregion

    #region MonoMethods

    private void Awake()
    {
        _canvas = UnityHelper.GetParentWithName(this.gameObject, "Canvas");
        _consoleInput = UnityHelper.GetChildWithName(this.gameObject, "Input").GetComponent<TMP_InputField>();
        _consoleOutput = UnityHelper.GetChildWithName(this.gameObject, "Output").GetComponent<TMP_Text>();
        _FPS = UnityHelper.GetChildWithName(_canvas, "FPS").GetComponent<TMP_Text>();

        this._commands = new List<string>();
        this._setableVariables = new List<string>();
        this._previousInputs = new List<string>();

        this.FillAndSortCommands();
        this.FillAndSortSetableVariables();

        this._player = this._playerObj.GetComponent<Player>();
        this._playerMovement = this._playerObj.GetComponent<PlayerMovement>();

        this._page = 1;
        this._pages = 1;
        this._inputIndex = -1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.FillInput(Input.GetKeyDown(KeyCode.UpArrow) ? "up" : "down");
        }

        if (Input.GetButtonDown("Enter"))
        {
            this.ReadInput();
            this._inputIndex = -1;
        }
    }

    private void OnEnable()
    {
        this.Focus();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Fills the commands list with all the commands and then sorts them
    /// </summary>
    private void FillAndSortCommands()
    {
        this._commands.Add("help - Shows all comands");
        this._commands.Add("tp <x> <y> <z> - Teleports you to the given coordinates");
        this._commands.Add("clr - Clears the console");
        this._commands.Add("set <variable> <newValue> - Sets the given variable to the given value");
        this._commands.Add("set_help - Shows all variables that can be changed");
        this._commands.Add("show_fps <val> - Shows(val = 0) or hides(val = 1) the fps counter");
        this._commands.Add("previous - Goes to the previous console page");
        this._commands.Add("next - Goes to the previous console page");

        this._commands.Sort();
    }

    /// <summary>
    /// Fills the setable variables list with all variables that could be set here and then sorts them
    /// </summary>
    private void FillAndSortSetableVariables()
    {
        this._setableVariables.Add("jump_force");
        this._setableVariables.Add("dashing_force");
        this._setableVariables.Add("dashing_force_up");
        // TODO: Add
        this._setableVariables.Add("gravity - TODO");

        this._setableVariables.Sort();
    }

    /// <summary>
    /// Focuses on the Input Field
    /// </summary>
    public void Focus()
    {
        this._consoleInput.Select();
        this._consoleInput.ActivateInputField();
    }

    /// <summary>
    /// Whenever there is a change in the input field (add, remove, etc) this method is called
    /// </summary>
    public void OnValueChanged()
    {
        string input = this._consoleInput.text;

        if (input.Contains('`'))
        {
            this._consoleInput.text = this._consoleInput.text.Replace("`", "");
        }
    }

    /// <summary>
    /// Whenever the Console Input Field is selected
    /// </summary>
    public void OnSelect()
    {
        this.Focus();
    }

    /// <summary>
    /// This method is called whenever the arrow keys are used
    /// If the direction is up, previous inputs will be loaded
    /// If the direction is dow, more recent ones will be loaded
    /// </summary>
    /// <param name="direction"></param>
    private void FillInput(string direction)
    {
        if (direction == "up")
        {
            if (_inputIndex < this._previousInputs.Count - 1)
            {
                this._inputIndex++;

                this._consoleInput.text = this._previousInputs[this._previousInputs.Count - 1 - _inputIndex];
                this._consoleInput.caretPosition = this._consoleInput.text.Length;
            }
        }
        else if (direction == "down")
        {
            if (_inputIndex > 0)
            {
                this._inputIndex--;

                this._consoleInput.text = this._previousInputs[this._previousInputs.Count - 1 - _inputIndex];
                this._consoleInput.caretPosition = this._consoleInput.text.Length;
            }
            else if (_inputIndex == 0)
            {
                this._inputIndex--;

                this._consoleInput.text = "";
                this._consoleInput.caretPosition = this._consoleInput.text.Length;
            }
        }

        return;
    }

    /// <summary>
    /// Reads the input from the console input field and determands what command should be executed.
    /// It checks if the params are correct and if so it executes the command
    /// </summary>
    private void ReadInput()
    {
        string input = this._consoleInput.text.ToLower();
        string output = "";

        this._consoleInput.text = "";

        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        if (this._previousInputs.Count == 0 || this._previousInputs.Last() != input)
        {
            this._previousInputs.Add(input);
        }

        string[] inputArgs = input.Split(' ').ToArray();

        switch (inputArgs[0])
        {
            case "help":
                if (inputArgs.Length != 1)
                {
                    output = "Invalid number of arguments" + "\n";

                    break;
                }

                output = this.Help();

                break;
            case "tp":
                if (inputArgs.Length != 4)
                {
                    output = "Invalid number of arguments" +
                        "" + "\n";

                    break;
                }

                try
                {
                    float x = float.Parse(inputArgs[1]);
                    float y = float.Parse(inputArgs[2]);
                    float z = float.Parse(inputArgs[3]);

                    output = this.TeleportToPos(x, y, z);
                }
                catch (System.Exception)
                {
                    output = "Invalid argument value" + "\n";
                }

                break;
            case "clr":
                if (inputArgs.Length != 1)
                {
                    output = "Invalid number of arguments" + "\n";

                    break;
                }

                this.Clear();

                break;
            case "set":
                if (inputArgs.Length != 3)
                {
                    output = "Invalid number of arguments" + "\n";

                    break;
                }

                try
                {
                    string variable = inputArgs[1];
                    string newValue = inputArgs[2];

                    output = this.Set(variable, newValue);
                }
                catch (Exception)
                {
                    output = "Invalid argument value" + "\n";
                }

                break;
            case "set_help":
                if (inputArgs.Length != 1)
                {
                    output = "Invalid number of arguments" + "\n";

                    break;
                }

                output = this.SetHelp();

                break;
            case "show_fps":
                if (inputArgs.Length == 1)
                {
                    output += this.ShowFPS(true) + "\n";
                }
                else if (inputArgs.Length == 2)
                {
                    output += this.ShowFPS(inputArgs[1] == "0" ? true : false) + "\n";
                }
                else
                {
                    output = "Invalid number of argments\n";
                }

                break;
            case "previous":
                if (inputArgs.Length != 1)
                {
                    output = "Invalid number of arguments\n";

                    break;
                }

                this.Previous();

                break;
            case "next":
                if (inputArgs.Length != 1)
                {
                    output = "Invalid number of arguments\n";

                    break;
                }

                this.Next();

                break;
            default:
                if (!string.IsNullOrEmpty(input))
                {
                    output = "Invalid command" + "\n";
                }

                break;
        }

        this._consoleOutput.text += output;

        if (this._consoleOutput.textInfo.pageCount != this._pages)
        {
            this._pages = this._consoleOutput.textInfo.pageCount;

            this._page = this._pages;
        }

        this._consoleOutput.pageToDisplay = this._page;

        this.Focus();
    }

    /// <summary>
    /// The help command
    /// </summary>
    /// <returns>A list of commands</returns>
    private string Help()
    {
        string output = "";

        output += "Commands:" + "\n";

        foreach (string command in this._commands)
        {
            output += command + "\n";
        }

        return output;
    }

    /// <summary>
    /// This command teleports the player object to a specific position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>That the player was teleported to the position</returns>
    private string TeleportToPos(float x, float y, float z)
    {
        Vector3 position = new Vector3(x, y, z);

        this._player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this._player.transform.SetPositionAndRotation(position, this._player.transform.rotation);

        return $"Player teleported to {position}" + "\n";
    }

    /// <summary>
    /// This command clears the output text
    /// </summary>
    private void Clear()
    {
        this._consoleOutput.text = "";
    }

    /// <summary>
    /// This command sets a new value to a variable that can is setable
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    private string Set(string variable, string newValue)
    {
        string output = "";

        if (variable == "jump_force")
        {
            float val = float.Parse(newValue);

            this._playerMovement.JumpForce = val;

            output = $"Player's jump force was changed to {val}\n";
        }
        else if (variable == "dashing_force")
        {
            float val = float.Parse(newValue);

            this._playerMovement.DashingForce = val;

            output = $"Player's dashing force was changed to {val}\n";
        }
        else if (variable == "dashing_force_up")
        {
            float val = float.Parse(newValue);

            this._playerMovement.DashingForceUp = val;

            output = $"Player's dashing up force was changed to {val}\n";
        }
        else
        {
            output = "Invalid variable\n";
        }

        return output;
    }

    /// <summary>
    /// Shows a list of all setable variables
    /// </summary>
    /// <returns></returns>
    private string SetHelp()
    {
        string output = "";

        output += "Setable Variables:\n";

        foreach (string variable in this._setableVariables)
        {
            output += $"- {variable}\n";
        }

        return output;
    }

    /// <summary>
    /// Enables and disables the FPS object
    /// </summary>
    /// <param name="active"></param>
    /// <returns></returns>
    private string ShowFPS(bool active)
    {
        this._FPS.gameObject.SetActive(active);

        string returner = active ? "activated" : "deativated";

        return $"The FPS counter was {returner}";
    }

    /// <summary>
    /// Goes to the previous output page
    /// </summary>
    private void Previous()
    {
        if (this._page > 1)
        {
            this._page--;
        }
    }

    /// <summary>
    /// Goes to the next output page
    /// </summary>
    private void Next()
    {
        if (this._page < this._pages)
        {
            this._page++;
        }
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    #region Properties

    public Image ConsoleBackground;
    public TMP_InputField ConsoleInput;
    public TMP_Text ConsoleOutput;
    public TMP_Text FPS;

    public GameObject Player;

    #endregion

    #region Fields

    private Player player;
    private PlayerMovement playerMovement;

    private List<string> commands;
    private List<string> setableVariables;
    private List<string> previousInputs;

    private int page;
    private int pages;
    private int inputIndex;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.commands = new List<string>();
        this.setableVariables = new List<string>();
        this.previousInputs = new List<string>();

        this.commands.Add("help - Shows all comands");
        this.commands.Add("tp <x> <y> <z> - Teleports you to the given coordinates");
        this.commands.Add("clr - Clears the console");
        this.commands.Add("set <variable> <newValue> - Sets the given variable to the given value");
        this.commands.Add("set_help - Shows all variables that can be changed");
        this.commands.Add("show_fps <val> - Shows(val = 0) or hides(val = 1) the fps counter");
        this.commands.Add("previous - Goes to the previous console page");
        this.commands.Add("next - Goes to the previous console page");

        this.commands.Sort();

        this.setableVariables.Add("jump_force");
        this.setableVariables.Add("dashing_force");
        this.setableVariables.Add("dashing_force_up");
        // TODO: Add
        this.setableVariables.Add("gravity - TODO");

        this.setableVariables.Sort();

        this.player = this.Player.GetComponent<Player>();
        this.playerMovement = this.Player.GetComponent<PlayerMovement>();

        this.page = 1;
        this.pages = 1;
        this.inputIndex = -1;
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
            this.inputIndex = -1;
        }
    }

    private void OnEnable()
    {
        this.Focus();
    }

    #endregion

    #region Methods

    public void Focus()
    {
        this.ConsoleInput.Select();
        this.ConsoleInput.ActivateInputField();
    }

    public void OnValueChanged()
    {
        string input = this.ConsoleInput.text;

        if (input.Contains('`'))
        {
            this.ConsoleInput.text = this.ConsoleInput.text.Replace("`", "");
        }
    }

    public void OnSelect()
    {
        this.Focus();
    }

    private void FillInput(string direction)
    {
        if (direction == "up")
        {
            if (inputIndex < this.previousInputs.Count - 1)
            {
                this.inputIndex++;

                this.ConsoleInput.text = this.previousInputs[this.previousInputs.Count - 1 - inputIndex];
                this.ConsoleInput.caretPosition = this.ConsoleInput.text.Length;
            }
        }
        else if (direction == "down")
        {
            if (inputIndex > 0)
            {
                this.inputIndex--;

                this.ConsoleInput.text = this.previousInputs[this.previousInputs.Count - 1 - inputIndex];
                this.ConsoleInput.caretPosition = this.ConsoleInput.text.Length;
            }
            else if (inputIndex == 0)
            {
                this.inputIndex--;

                this.ConsoleInput.text = "";
                this.ConsoleInput.caretPosition = this.ConsoleInput.text.Length;
            }
        }

        return;
    }

    private void ReadInput()
    {
        string input = this.ConsoleInput.text.ToLower();
        string output = "";

        this.ConsoleInput.text = "";

        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        if (this.previousInputs.Count == 0 || this.previousInputs.Last() != input)
        {
            this.previousInputs.Add(input);
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

        this.ConsoleOutput.text += output;

        if (this.ConsoleOutput.textInfo.pageCount != this.pages)
        {
            this.pages = this.ConsoleOutput.textInfo.pageCount;

            this.page = this.pages;
        }

        this.ConsoleOutput.pageToDisplay = this.page;

        this.Focus();
    }

    private string Help()
    {
        string output = "";

        output += "Commands:" + "\n";

        foreach (string command in this.commands)
        {
            output += command + "\n";
        }

        return output;
    }

    private string TeleportToPos(float x, float y, float z)
    {
        Vector3 position = new Vector3(x, y, z);

        this.player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.player.transform.SetPositionAndRotation(position, this.player.transform.rotation);

        return $"Player teleported to {position}" + "\n";
    }

    private void Clear()
    {
        this.ConsoleOutput.text = "";
    }

    private string Set(string variable, string newValue)
    {
        string output = "";

        if (variable == "jump_force")
        {
            float val = float.Parse(newValue);

            this.playerMovement.JumpForce = val;

            output = $"Player's jump force was changed to {val}\n";
        }
        else if (variable == "dashing_force")
        {
            float val = float.Parse(newValue);

            this.playerMovement.DashingForce = val;

            output = $"Player's dashing force was changed to {val}\n";
        }
        else if (variable == "dashing_force_up")
        {
            float val = float.Parse(newValue);

            this.playerMovement.DashingForceUp = val;

            output = $"Player's dashing up force was changed to {val}\n";
        }
        else
        {
            output = "Invalid variable\n";
        }

        return output;
    }

    private string SetHelp()
    {
        string output = "";

        output += "Setable Variables:\n";

        foreach (string variable in this.setableVariables)
        {
            output += $"- {variable}\n";
        }

        return output;
    }

    private string ShowFPS(bool active)
    {
        this.FPS.gameObject.SetActive(active);

        string returner = active ? "activated" : "deativated";

        return $"The FPS counter was {returner}";
    }

    private void Previous()
    {
        if (this.page > 1)
        {
            this.page--;
        }
    }

    private void Next()
    {
        if (this.page < this.pages)
        {
            this.page++;
        }
    }

    #endregion
}

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

    public GameObject Player;

    #endregion

    #region Fields

    private Player player;
    private PlayerMovement playerMovement;

    private List<string> commands;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.commands = new List<string>();

        this.commands.Add("help - Shows all comands");
        this.commands.Add("tp <x> <y> <z> - Teleports you to the given coordinates");
        this.commands.Add("clr - Clears the console");

        this.commands.Sort();

        this.player = this.Player.GetComponent<Player>();
        this.playerMovement = this.Player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {

        if (Input.GetButtonDown("Enter"))
        {
            this.ReadInput();
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

        if (!input.StartsWith(">"))
        {
            this.ConsoleInput.text = this.ConsoleInput.text.Insert(0, ">");
        }

        if (input.Count(c => c == '>') > 1)
        {
            this.ConsoleInput.text = ">" + this.ConsoleInput.text.Replace(">", "");
        }

        if (input.Contains('`'))
        {
            this.ConsoleInput.text = this.ConsoleInput.text.Replace("`", "");
        }
    }

    public void OnSelect()
    {
        if (string.IsNullOrEmpty(this.ConsoleInput.text))
        {
            this.ConsoleInput.text += ">";
        }

        this.Focus();
    }

    private void ReadInput()
    {
        string input = this.ConsoleInput.text;
        string output = "";

        this.ConsoleInput.text = ">";

        if (input == ">" || string.IsNullOrEmpty(input))
        {
            return;
        }

        string[] inputArgs = input.Split(' ').ToArray();

        switch (inputArgs[0])
        {
            case ">help":
                if (inputArgs.Length != 1)
                {
                    output = "Invalid number of arguments" + "\n";

                    break;
                }

                output = this.Help();

                break;
            case ">tp":
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
            case ">clr":
                if (inputArgs.Length != 1)
                {
                    output = "Invalid number of arguments";

                    break;
                }

                this.Clear();

                break;
            default:
                if (input.StartsWith(">"))
                {
                    output = "Invalid command" + "\n";
                }
                else if (!string.IsNullOrEmpty(input))
                {
                    output = "Invalid input! Every command must begins with '>'" + "\n";
                }
                break;
        }

        this.ConsoleOutput.text += output;

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

    #endregion
}

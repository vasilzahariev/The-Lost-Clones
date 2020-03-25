using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Make it so the player can close the console, but can't stop the action
/// </summary>
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

        this.commands.Add(">help - Shows all commands");
        this.commands.Add(">tp <x> <y> <z> - Teleport to the given x, y and z coordinates");

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

    #endregion

    #region Methods

    private void ReadInput()
    {
        string input = this.ConsoleInput.text;
        string output = "";

        this.ConsoleInput.text = ">";

        List<string> inputArgs = input.Split(' ').ToList();

        switch (inputArgs[0])
        {
            case ">help":
                output = "Commands:" + "\n" + this.Help();
                break;
            case ">tp":
                if (string.IsNullOrEmpty(inputArgs[1]) || string.IsNullOrEmpty(inputArgs[2]) || string.IsNullOrEmpty(inputArgs[3]))
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
                    output = "Try again, but this time use the oposite one ('.' - ',')." + "\n";
                }

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
    }

    private string Help()
    {
        string output = "";

        foreach (string command in this.commands)
        {
            output += command + "\n";
        }

        return output;
    }

    private string TeleportToPos(float x, float y, float z)
    {
        Vector3 position = new Vector3(x, y, z);

        this.player.transform.SetPositionAndRotation(position, this.player.transform.rotation);

        return $"Player teleported to {position}" + "\n";
    }

    #endregion
}

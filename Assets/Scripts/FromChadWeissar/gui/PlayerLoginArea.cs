using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEditor.Build.Reporting;
using static ENUMS;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class PlayerLoginArea : MonoBehaviour
{
    private const string ChooseRoleTextContent = "Choose a role for this game.";
    public MainMenu MainMenu;
    public TMPro.TextMeshProUGUI ChooseRoleText;
    public TMPro.TextMeshProUGUI playerNameText;
    public int Position;
    public RoleCardDisplay [] roleCards;

    Player.Roles? _role;

    public Player.Roles? Role
    {
        get { return _role; }
        set
        {
            if(_role != null)
            {
                roleCards[(int)_role].background.GetComponent<Outline>().enabled = false;
            }
            if(value != null)
            {
                roleCards[(int)value].background.GetComponent<Outline>().enabled = true;
                ChooseRoleText.text = "You have chosen " + (value.GetDescription()) + " as your role for this game.";
            }
            _role = value;

        }
    }

    // Use this for initialization
    void Start()
    {
        resetPlayerLoginArea();
    }

    public void resetPlayerLoginArea()
    {
        Role = null;
        playerNameText.text = MainMenu.PlayerNames[Position] + "'s Play Area";
        ChooseRoleText.text = ChooseRoleTextContent;
    }



    public void OnRoleClicked(int role)
    {
        AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CLICK);
        Player.Roles roleToChangeTo = (Player.Roles)role;
        if (Role == roleToChangeTo)
        {
            Role = null;
            MainMenu.FreeRoles.Add(roleToChangeTo);
            ChooseRoleText.text = ChooseRoleTextContent;
            MainMenu.UpdateRoles();
        }
        else
        if (MainMenu.FreeRoles.Contains(roleToChangeTo))
        {
            MainMenu.FreeRoles.Remove(roleToChangeTo);
            if (Role != null)
            {
                MainMenu.FreeRoles.Add(Role.Value);
            }
            Role = (Player.Roles)role;
            MainMenu.UpdateRoles();
        }
    }

    public bool isPlaying()
    {
        return Role != null;
    }

    internal void UpdateRole(HashSet<Player.Roles> freeRoles)
    {
        foreach (Player.Roles role in Enum.GetValues(typeof(Player.Roles)))
        {
            if(role != Role)
            {
                if(freeRoles.Contains(role))
                    roleCards[(int)role].gameObject.SetActive(true);
                else
                    roleCards[(int)role].gameObject.SetActive(false);
            }    
        }
    }
}

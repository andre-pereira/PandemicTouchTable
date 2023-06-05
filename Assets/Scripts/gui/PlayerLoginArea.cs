﻿using UnityEngine;
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
    public GameObject prefabRolecard;
    public GameObject horizontalLayoutRoles;
    public int Position;

    private GameObject[] roleCards;
    private RoleCardDisplay[] roleCardsDisplay;

    private string playerName;

    public string PlayerName
    {
        get { return playerName; }
        set {
            playerName = value;
            playerNameText.text = playerName;
        }
    }


    Player.Roles? _role;

    public Player.Roles? Role
    {
        get { return _role; }
        set
        {
            if(_role != null)
            {
                roleCardsDisplay[(int)_role].background.GetComponent<Outline>().enabled = false;
            }
            if(value != null)
            {
                roleCardsDisplay[(int)value].background.GetComponent<Outline>().enabled = true;
                ChooseRoleText.text = "You have chosen " + (value.GetDescription()) + " as your role for this game.";
            }
            _role = value;

        }
    }

    // Use this for initialization
    void Start()
    {
        //resetPlayerLoginArea();
    }

    public void resetPlayerLoginArea()
    {
        Role = null;
        PlayerName = MainMenu.PlayerNames[Position];
        ChooseRoleText.text = ChooseRoleTextContent;
        changePlayerAreaColor(null, GameGUI.gui.playerUIOpacity);
        roleCards = new GameObject[Enum.GetValues(typeof(Player.Roles)).Length];
        roleCardsDisplay = new RoleCardDisplay[roleCards.Length];
        for (int i = 0; i < roleCards.Length; i++)
        {
            int currentValue = i;
            roleCards[i] = Instantiate(prefabRolecard, horizontalLayoutRoles.transform);
            roleCardsDisplay[i] = roleCards[i].GetComponent<RoleCardDisplay>();
            roleCardsDisplay[i].RoleCardData = GameGUI.gui.roleCards[i];
            roleCards[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                OnRoleClicked((Player.Roles)currentValue);
            });
        }
    }

    public void changePlayerAreaColor(Player.Roles? role, float alpha)
    {
        Color color = Color.gray;
        if (role != null)
        {
            foreach (var roleCard in roleCardsDisplay)
            {
                if (roleCard.RoleCardData.roleName == role.GetDescription())
                {
                    color = roleCard.RoleCardData.roleColor;
                }
            }
        }
        color.a = alpha;
        gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
    }


    public void OnRoleClicked(Player.Roles roleToChangeTo)
    {
        AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CLICK);
        if (Role == roleToChangeTo)
        {
            Role = null;
            MainMenu.FreeRoles.Add(roleToChangeTo);
            ChooseRoleText.text = ChooseRoleTextContent;
            MainMenu.UpdateRoles();
            changePlayerAreaColor(null, GameGUI.gui.playerUIOpacity);
        }
        else
        if (MainMenu.FreeRoles.Contains(roleToChangeTo))
        {
            MainMenu.FreeRoles.Remove(roleToChangeTo);
            if (Role != null)
            {
                MainMenu.FreeRoles.Add(Role.Value);
            }
            Role = roleToChangeTo;
            changePlayerAreaColor(roleToChangeTo, GameGUI.gui.playerUIOpacity);
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

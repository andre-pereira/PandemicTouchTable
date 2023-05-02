using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ENUMS;

public class RoleCardDisplay : MonoBehaviour
{
    public RoleCard roleCardData;

    public Image background;

    public TextMeshProUGUI roleName;
    public TextMeshProUGUI roleText;

    // Start is called before the first frame update
    void Start()
    {
        roleName.text = roleCardData.roleName;
        roleText.text = roleCardData.roleText;
        background.sprite = roleCardData.mainArtwork;
    }
}
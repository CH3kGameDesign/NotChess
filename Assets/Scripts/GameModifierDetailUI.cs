using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameModifierDetailUI : MonoBehaviour
{
    public TextMeshProUGUI TM_name;
    public TextMeshProUGUI TM_description;
    public Image I_sprite;
    [Space(10)]
    public float F_height = 250;

    public void OnCreate(VarClass.gameModifierClass _gameMod)
    {
        TM_name.text = _gameMod.name;
        TM_description.text = _gameMod.description;

        I_sprite.sprite = _gameMod.sprAddressable.spr;
    }
}

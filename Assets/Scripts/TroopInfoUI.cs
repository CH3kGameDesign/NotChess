using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroopInfoUI : MonoBehaviour
{
    public Image I_troopSprite;
    public Image I_modifier;

    public TextMeshProUGUI TM_name;
    public TextMeshProUGUI TM_description;
    public TextMeshProUGUI TM_modName;
    public TextMeshProUGUI TM_modDescription;
    public TextMeshProUGUI TM_player;
    public TextMeshProUGUI TM_points;

    public GameObject G_Modifier;
    public GameObject G_Canvas;

    public RectTransform T_Mover;
    private Vector2 V2_pos = new Vector2(2, 0);

    public void LoadTroop(VarClass.troopClass _troop)
    {
        if (_troop != null)
        {
            TM_name.text = _troop.name;
            TM_description.text = _troop.movementArray.ConvertToMoveString();
            I_troopSprite.sprite = _troop.sprAddressable.spr;

            if (_troop.isPlayer)
            {
                TM_player.text = "PLAYER";
                TM_points.text = "POINTS:  <color=#AAAAAA>" + _troop.getPoints().AbbreviatedString() + "</color>";
            }
            else
            {
                TM_player.text = "ENEMY";
                TM_points.text = "MULT:  <color=#AAAAAA>" + _troop.getMultiplier().AbbreviatedString() + "</color>";
            }

            if (_troop.mod.id != "")
            {
                I_modifier.sprite = _troop.mod.sprAddressable.spr;
                TM_modName.text = _troop.mod.name;
                TM_modDescription.text = _troop.mod.description;

                G_Modifier.SetActive(true);
                T_Mover.anchoredPosition = new Vector2(V2_pos.y, T_Mover.anchoredPosition.y);
            }
            else
            {
                G_Modifier.SetActive(false);
                T_Mover.anchoredPosition = new Vector2(V2_pos.x, T_Mover.anchoredPosition.y);
            }

            G_Canvas.SetActive(true);
        }
        else
            UnloadTroop();
    }
    public void UnloadTroop()
    {
        G_Canvas.SetActive(false);
    }
}

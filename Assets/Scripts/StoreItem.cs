using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreItem : MonoBehaviour
{
    public RectTransform RT_transform;
    public RectTransform RT_description;

    public GameObject G_descriptionObject;
    public GameObject G_modifierObject;
    public GameObject G_target;

    public TextMeshProUGUI TM_name;
    public TextMeshProUGUI TM_description;
    public TextMeshProUGUI TM_cost;
    public TextMeshProUGUI TM_targetArea;

    public TextMeshProUGUI TM_modDescription;

    public Image I_modifier;
    public Image I_sprite;
    public Image I_modifierDescIcon;
    public Image I_BG;

    public Vector2 V2_selHeights = new Vector2(0, 100);

    [Header("Colors")]
    public Color C_actionCard = new Color(0f, 0.15f, 0.3f, 1f);
    public Color C_modifierCard = new Color(0.3f, 0.15f, 0f, 1f);

    private itemTypeEnum itemType;
    public enum itemTypeEnum { pack, cardModifier, troopModifier, gameModifier, card, cardPack_item};

    [HideInInspector] public VarClass.storePackClass storePack;
    [HideInInspector] public VarClass.gameModifierClass gameMod;
    [HideInInspector] public VarClass.cardClass card;

    private Coroutine c_move;
    private bool b_sel = false;

    public void OnCreate(VarClass.storePackClass _storePack)
    {
        storePack = _storePack;
        itemType = itemTypeEnum.pack;

        TM_name.text = storePack.name;
        TM_description.text = storePack.description;
        TM_cost.text = storePack.cost.ToCashAmount(0);
        I_sprite.sprite = storePack.sprAddressable.spr;

        RT_description.anchoredPosition = new Vector2(0, 0);
        G_target.SetActive(false);
    }

    public void OnCreate(VarClass.gameModifierClass _gameMod)
    {
        gameMod = _gameMod;
        itemType = itemTypeEnum.gameModifier;

        TM_name.text = gameMod.name;
        TM_description.text = gameMod.description;
        TM_cost.text = gameMod.cost.ToCashAmount(0);
        I_sprite.sprite = gameMod.sprAddressable.spr;

        RT_description.anchoredPosition = new Vector2(0, 0);
        G_target.SetActive(false);
    }

    public void OnCreate(VarClass.cardClass _card, bool _cardPack)
    {
        card = _card;
        if (_cardPack)
        {
            itemType = itemTypeEnum.cardPack_item;
            TM_cost.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            itemType = itemTypeEnum.card;
            TM_cost.text = (card.cost + card.cardMod.cost).ToCashAmount(0);
        }

        TM_name.text = card.name;
        TM_description.text = card.description;
        
        I_sprite.sprite = card.sprAddressable.spr;

        if (_card.cardMod.id != "")
        {
            I_modifier.sprite = _card.cardMod.sprAddressable.spr;
            I_modifier.gameObject.SetActive(true);

            G_modifierObject.SetActive(true);
            I_modifierDescIcon.sprite = _card.cardMod.sprAddressable.spr;
            TM_modDescription.text = _card.cardMod.description;
        }

        G_target.SetActive(true);
        TM_targetArea.text = _card.targetArray.ConvertToEffectString(_card.targetTile.ToString());
        CardColor();
    }

    void CardColor()
    {
        switch (card.cardType)
        {
            case "cardAction":
                I_BG.color = C_actionCard;
                break;
            case "cardModifier":
                I_BG.color = C_modifierCard;
                break;
            default:
                break;
        }
    }

    public void OnClick()
    {
        switch (itemType)
        {
            case itemTypeEnum.pack:
                GameManager.Instance.CardPack_Open(storePack, this);
                break;
            case itemTypeEnum.cardModifier:
                break;
            case itemTypeEnum.troopModifier:
                break;
            case itemTypeEnum.gameModifier:
                GameManager.Instance.GameMod_Purchase(gameMod, this);
                break;
            case itemTypeEnum.card:
                GameManager.Instance.Card_Purchase(card, this);
                break;
            case itemTypeEnum.cardPack_item:
                GameManager.Instance.CardPack_SelectItem(this);
                break;
            default:
                break;
        }
    }

    public void OnHold()
    {
        G_descriptionObject.SetActive(true);
    }
    public void OnRelease()
    {
        if (!b_sel)
            G_descriptionObject.SetActive(false);
    }

    public void OnSelected(bool _sel)
    {
        b_sel = _sel;
        if (c_move != null)
            StopCoroutine(c_move);
        if (_sel)
        {
            c_move = StartCoroutine(Move(new Vector2(RT_transform.anchoredPosition.x, V2_selHeights.y), 0.25f));
            OnHold();
        }
        else
        {
            c_move = StartCoroutine(Move(new Vector2(RT_transform.anchoredPosition.x, V2_selHeights.x), 0.25f));
            OnRelease();
        }
    }

    IEnumerator Move(Vector2 tarLocalPos, float duration)
    {
        float timer = 0;
        Vector2 startPos = RT_transform.anchoredPosition;
        while (timer < 1)
        {
            timer += Time.deltaTime / duration;
            Vector3 tarPos = Vector3.Lerp(startPos, tarLocalPos, timer);
            transform.localPosition = tarPos;
            yield return new WaitForEndOfFrame();
        }
        Vector3 finalPos = tarLocalPos;
        transform.localPosition = finalPos;
    }
}

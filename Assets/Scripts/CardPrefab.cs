using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPrefab : MonoBehaviour
{
    [Header ("Object References")]
    public Canvas C_Canvas;
    [Space (10)]
    public Image I_CardBG;
    public Image I_CardPortraitBG;
    public Image I_CardPortrait;
    public Image I_Backing;
    public Image I_ModifierBG;
    public Image I_ModifierImage;

    public GameObject G_Description;
    public RectTransform RT_DescriptionMover;

    public Image I_cardMod;

    public TextMeshProUGUI TM_CardName;
    public TextMeshProUGUI TM_CardDescription;
    public TextMeshProUGUI TM_ModifierDescription;
    public TextMeshProUGUI TM_TargetArea;

    [Header("Colors")]
    public Color C_actionBG;
    public Color C_modifierBG;

    [HideInInspector] public VarClass.cardClass card;

    private Vector3 V3_basePos;
    private Coroutine moveCoroutine;

    [HideInInspector] public bool B_Used = false;

    public void Start()
    {
        C_Canvas.worldCamera = Camera.main;
    }

    public void OnCreate(VarClass.cardClass _card)
    {
        card = _card;

        I_CardPortrait.sprite = card.sprAddressable.spr;
        TM_CardName.text = card.name;
        TM_CardDescription.text = card.description;
        TM_TargetArea.text = card.targetArray.ConvertToEffectString(card.targetTile.ToString());

        UpdateCardColor();

        if (_card.cardMod.id != "")
        {
            I_cardMod.sprite = _card.cardMod.sprAddressable.spr;
            I_cardMod.gameObject.SetActive(true);

            I_ModifierBG.gameObject.SetActive(true);
            I_ModifierImage.sprite = _card.cardMod.sprAddressable.spr;
            TM_ModifierDescription.text = _card.cardMod.description;
        }    
    }

    void UpdateCardColor()
    {
        switch (card.cardType)
        {
            case "cardAction":
                I_CardBG.color = C_actionBG;
                break;
            case "cardModifier":
                I_CardBG.color = C_modifierBG;
                break;
            default:
                break;
        }
    }

    public void OnUsed()
    {
        GetComponent<Collider>().enabled = false;
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        StartCoroutine(RotateDown(0.25f));
        G_Description.SetActive(false);
        moveCoroutine = StartCoroutine(Move(new Vector3(0, 0, -1f), 0.25f, false));
        B_Used = true;
    }
    public void OnRemove()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(Move(new Vector3(0, 0, -5f), 0.25f, true));
        GameObject.Destroy(this.gameObject, 0.5f);
    }

    public void UpdateBasePos(Vector3 _basePos)
    {
        V3_basePos = _basePos;
        RT_DescriptionMover.anchoredPosition = new Vector2(-_basePos.x/3,0);
        ResetPos(0.25f);
    }

    public void SelectPos()
    {
        G_Description.SetActive(true);
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(Move(new Vector3(0,0,1), 0.1f, false));
    }
    public void ResetPos()
    {
        ResetPos(0.1f);
    }
    public void ResetPos(float _duration)
    {
        G_Description.SetActive(false);
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(Move(Vector3.zero, _duration, false));
    }

    IEnumerator Move (Vector3 tarLocalPos, float duration, bool absPos)
    {
        float timer = 0;
        Vector3 startPos = transform.localPosition;
        if (!absPos)
            startPos -= V3_basePos;
        while (timer < 1)
        {
            timer += Time.deltaTime / duration;
            Vector3 tarPos = Vector3.Lerp(startPos, tarLocalPos, timer);
            if (!absPos)
                tarPos += V3_basePos;
            transform.localPosition = tarPos;
            yield return new WaitForEndOfFrame();
        }
        Vector3 finalPos = tarLocalPos;
        if (!absPos)
            finalPos += V3_basePos;
        transform.localPosition = finalPos;
    }
    IEnumerator RotateDown(float duration)
    {
        float timer = 0;
        Vector3 startRot = Vector3.zero;
        Vector3 tarLocalRot = new Vector3(0, 180, 0);
        while (timer < 0.5f)
        {
            timer += Time.deltaTime / duration;
            C_Canvas.transform.localEulerAngles = Vector3.Lerp(startRot, tarLocalRot, timer);
            yield return new WaitForEndOfFrame();
        }
        I_Backing.gameObject.SetActive(true);
        while (timer < 1)
        {
            timer += Time.deltaTime / duration;
            C_Canvas.transform.localEulerAngles = Vector3.Lerp(startRot, tarLocalRot, timer);
            yield return new WaitForEndOfFrame();
        }
        C_Canvas.transform.localEulerAngles = tarLocalRot;
    }
}

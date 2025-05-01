using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundSelectObject : MonoBehaviour
{
    private bool b_available;
    private int i_roundNum;

    public RectTransform RT_transform;

    public GameObject G_darkenator;

    public TextMeshProUGUI TM_tarScore;
    public TextMeshProUGUI TM_reward;

    public TextMeshProUGUI TM_skipText;

    [Header ("Buttons")]
    public Button B_skip;
    public Button B_select;

    [Header("Skip Reward")]
    public TextMeshProUGUI TM_sRewardName;
    public TextMeshProUGUI TM_sRewardDescription;
    public Image I_sRewardImage;
    private VarClass.roundSkipReward R_roundSkip;

    public void OnCreate(int _roundNum, bool _available, bool _bossFight, double _tarScore, int _reward, VarClass.roundSkipReward _roundSkip)
    {
        i_roundNum = _roundNum;
        UpdateAvailability(_available, _bossFight);

        R_roundSkip = _roundSkip;
        TM_sRewardName.text = R_roundSkip.name;
        TM_sRewardDescription.text = R_roundSkip.description;
        I_sRewardImage.sprite = R_roundSkip.sprAddressable.spr;

        TM_tarScore.text = _tarScore.AbbreviatedString();
        TM_reward.text = "<color=#FFFFFF>Reward: </color>"+_reward.ToCashAmount(5);
    }

    public void UpdateAvailability(bool _available, bool _bossFight)
    {
        b_available = _available;

        G_darkenator.SetActive(!b_available);
        B_skip.interactable = b_available && !_bossFight;
        B_select.interactable = b_available;
    }

    public void OnSkip()
    {
        GameManager.Instance.SkipRound(R_roundSkip);
    }

    public void OnSelect()
    {
        GameManager.Instance.LoadNextRound();
    }
}

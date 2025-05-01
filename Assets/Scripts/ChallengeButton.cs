using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChallengeButton : MonoBehaviour
{
    public TextMeshProUGUI TM_Name;

    private VarClass.challengeClass c_Challenge;

    public float F_height = 200;

    public void OnCreate(VarClass.challengeClass _challenge)
    {
        c_Challenge = _challenge;
        TM_Name.text = c_Challenge.name;
    }

    public void OnClick()
    {
        MenuManager.Instance.SCENE_LoadChallenge(c_Challenge);
    }
}

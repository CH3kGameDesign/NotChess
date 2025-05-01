using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Not Chess/Info List/Challenges", fileName = "New Challenge List")]
public class ChallengeList : ScriptableObject
{
    public static ChallengeList Instance;
    public List<VarClass.challengeClass> list = new List<VarClass.challengeClass>();

    public VarClass.challengeClass FindChallengeByID(string id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i];

        Debug.LogError("Couldn't Find Troop: " + id);
        return null;
    }
    public VarClass.challengeClass GetRandom()
    {
        int _ran = Random.Range(0, list.Count);

        return list[_ran];
    }
}

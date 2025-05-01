using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Not Chess/Info List/Round Skip Reward", fileName = "New Round Skip List")]
public class RoundSkipList : ScriptableObject
{
    public static RoundSkipList Instance;
    public List<VarClass.roundSkipReward> list = new List<VarClass.roundSkipReward>();

    public VarClass.roundSkipReward FindModifierByID(string id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i];

        return null;
    }

    public VarClass.roundSkipReward GetRandom()
    {
        int _weightTotal = 1;
        foreach (var item in list)
            _weightTotal += item.weight;
        int _ran = Random.Range(0, _weightTotal);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].weight > 0)
            {
                _ran -= list[i].weight;
                if (_ran <= 0)
                    return list[i];
            }
        }
        return list[list.Count-1];
    }
}

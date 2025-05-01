using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Not Chess/Info List/Troop Modifiers", fileName = "New Troop Modifier List")]
public class TroopModList : ScriptableObject
{
    public static TroopModList Instance;
    public List<VarClass.troopModifierClass> list = new List<VarClass.troopModifierClass>();

    public VarClass.troopModifierClass FindModifierByID(string id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i];

        return null;
    }

    public VarClass.troopModifierClass GetRandom()
    {
        int _weightTotal = 1;
        foreach (var item in list)
            _weightTotal += item.weight;
        int _ran = Random.Range(0, _weightTotal);

        for (int i = 0; i < list.Count; i++)
        {
            _ran -= list[i].weight;
            if (_ran <= 0)
                return list[i];
                
        }
        return list[list.Count - 1];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Not Chess/Info List/Game Modifiers", fileName = "New Game Modifier List")]
public class GameModList : ScriptableObject
{
    public static GameModList Instance;
    public List<VarClass.gameModifierClass> list = new List<VarClass.gameModifierClass>();
    [HideInInspector] public List<VarClass.gameModifierClass> list_unowned = new List<VarClass.gameModifierClass>();

    public void OnAwake()
    {
        list_unowned = CopyList();
    }

    public List<VarClass.gameModifierClass> CopyList()
    {
        List<VarClass.gameModifierClass> _list = new List<VarClass.gameModifierClass>();
        for (int i = 0; i < list.Count; i++)
        {
            _list.Add(list[i]);
        }
        return _list;
    }

    public VarClass.gameModifierClass FindModifierByID(string id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i];

        return null;
    }

    public VarClass.gameModifierClass GetRandom()
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
        return list[list.Count-1];
    }

    public VarClass.gameModifierClass GetRandomUnowned()
    {
        if (list_unowned.Count > 0)
        {
            int _weightTotal = 1;
            foreach (var item in list_unowned)
                _weightTotal += item.weight;
            int _ran = Random.Range(0, _weightTotal);

            for (int i = 0; i < list_unowned.Count; i++)
            {
                _ran -= list_unowned[i].weight;
                if (_ran <= 0)
                    return list_unowned[i];
            }
            return list_unowned[list_unowned.Count - 1];
        }
        return null;
    }
}

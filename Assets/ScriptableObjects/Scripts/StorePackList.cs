using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Not Chess/Info List/Store Packs", fileName = "New Store Pack List")]
public class StorePackList : ScriptableObject
{
    public static StorePackList Instance;
    public List<VarClass.storePackClass> list = new List<VarClass.storePackClass>();

    public VarClass.storePackClass FindPackByID(string id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i];

        return null;
    }

    public VarClass.storePackClass GetRandom()
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
}

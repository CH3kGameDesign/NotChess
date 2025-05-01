using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Not Chess/Info List/Cards", fileName = "New Card List")]
public class CardList : ScriptableObject
{
    public static CardList Instance;
    public float F_modChance = 0.25f;
    public List<VarClass.cardClass> list = new List<VarClass.cardClass>();

    public VarClass.cardClass FindCardByID(string id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i].Clone();

        return null;
    }

    public VarClass.cardClass GetRandom()
    {
        int _weightTotal = 1;
        foreach (var item in list)
            _weightTotal += item.weight;
        int _ran = Random.Range(0, _weightTotal);

        VarClass.cardClass _temp = null;

        for (int i = 0; i < list.Count; i++)
        {
            _ran -= list[i].weight;
            if (_ran <= 0)
            {
                _temp = list[i].Clone();
                break;
            }
        }
        if (Random.Range(0f, 1f) < F_modChance)
            _temp.cardMod = CardModList.Instance.GetRandom();
        return _temp;
    }
}

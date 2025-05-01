using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Not Chess/Info List/Troops", fileName = "New Troop List")]
public class TroopList : ScriptableObject
{
    public static TroopList Instance;
    public List<VarClass.troopClass> list = new List<VarClass.troopClass>();

    public VarClass.troopClass FindTroopByID(string id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i];

        Debug.LogError("Couldn't Find Troop: " + id);
        return null;
    }
    public VarClass.troopClass GetRandom()
    {
        int _ran = Random.Range(0, list.Count);

        return list[_ran];
    }
}

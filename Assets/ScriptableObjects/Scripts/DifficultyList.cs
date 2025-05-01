using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Not Chess/Info List/Difficulties", fileName = "New Difficulty List")]
public class DifficultyList : ScriptableObject
{
    public static DifficultyList Instance;
    public List<VarClass.difficultyClass> list = new List<VarClass.difficultyClass>();

    public VarClass.difficultyClass FindDifficultyByID(string id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i];

        return null;
    }
}

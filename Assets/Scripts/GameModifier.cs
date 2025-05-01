using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameModifier
{
    #region Passive Modifiers
    public static float ApplyModifier(this float _value, string _type) { return ApplyModifier(_value, _type, ""); }
    public static float ApplyModifier(this float _value, string _type, string _type2)
    {
        float _result = _value;

        float _offset = GetOffset(_type, _type2);
        _result = _result + _offset;
        float _mult = GetMultiplier(_type, _type2);
        _result *= _mult;

        return _result;
    }

    public static double ApplyModifier(this double _value, string _type) { return ApplyModifier(_value, _type, ""); }
    public static double ApplyModifier(this double _value, string _type, string _type2)
    {
        double _result = _value;

        float _offset = GetOffset(_type, _type2);
        _result = _result + _offset;
        float _mult = GetMultiplier(_type, _type2);
        _result *= _mult;

        return _result;
    }

    public static int ApplyModifier(this int _value, string _type)  { return ApplyModifier(_value, _type, ""); }
    public static int ApplyModifier(this int _value, string _type, string _type2)
    {
        int _result = _value;

        float _offset = GetOffset(_type, _type2);
        _result = _result + (int)_offset;
        float _mult = GetMultiplier(_type, _type2);
        _result = Mathf.RoundToInt(_result * _mult);

        return _result;
    }

    public static VarClass.intArray[] ApplyModifier(this VarClass.intArray[] _array, string _id)
    {
        string[] _sArray;
        VarClass.intArray[] _output = (VarClass.intArray[])_array.Clone();
        foreach (var item in PlayerData.M_EquippedMods)
        {
            if (item.triggerID == "passive")
            {
                switch (item.effectID)
                {
                    case "movementChange":
                        if (_id == item.effectVar2)
                        {
                            _sArray = item.effectVar.Split(",");
                            for (int y = 0; y < 7; y++)
                            {
                                for (int x = 0; x < 7; x++)
                                {
                                    _output[y].array[x] = int.Parse(_sArray[x + (y * 7)]);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        return _output;
    }

    static float GetMultiplier(string _type, string _var2)
    {
        float _mult = 1;
        foreach (var item in PlayerData.M_EquippedMods)
        {
            if (item.triggerID == "passive")
            {
                switch (item.effectID)
                {
                    case "multiplierMultiply":
                        if (_type == "multiplier")
                            _mult *= float.Parse(item.effectVar);
                        break;
                    case "rerollCost":
                        if (_type == "rerollCost")
                            _mult *= float.Parse(item.effectVar);
                        break;
                    case "valueMultiply":
                        if (_type == "troopValue" && _var2 == item.effectVar2)
                            _mult *= float.Parse(item.effectVar);
                        break;
                    default:
                        break;
                }
            }
        }
        return _mult;
    }

    static float GetOffset(string _type, string _var2)
    {
        float _offset = 0;
        foreach (var item in PlayerData.M_EquippedMods)
        {
            if (item.triggerID == "passive")
            {
                switch (item.effectID)
                {
                    case "cardLimitAdd":
                        if (_type == "cardLimit")
                            _offset += float.Parse(item.effectVar);
                        break;
                    case "storeSizeAdd":
                        if (_type == "storeSize")
                            _offset += float.Parse(item.effectVar);
                        break;
                    case "turnAdd":
                        if (_type == "troopTurn" && _var2 == item.effectVar2)
                            _offset += float.Parse(item.effectVar);
                        break;
                    default:
                        break;
                }
            }
        }
        return _offset;
    }
    #endregion

    #region Triggered Effects
    public static void OnAllyDeath()
    {
        ApplyEffects("allyDeath");
    }
    public static void OnEnemyDeath()
    {
        ApplyEffects("enemyDeath");
    }
    public static void OnTurnEnd()
    {
        ApplyEffects("turnEnd");
    }

    static void ApplyEffects(string _trigger)
    {
        float f_var;
        int i_var;
        int i_var2;
        foreach (var item in PlayerData.M_EquippedMods)
        {
            if (item.triggerID == _trigger)
            {
                switch (item.effectID)
                {
                    case "pointAdd":
                        f_var = float.Parse(item.effectVar);
                        GameManager.Instance.AddRemovePoints(f_var);
                        break;
                    case "multiplierAddPerVar":
                        f_var = float.Parse(item.effectVar);
                        i_var2 = GetVar(item.effectVar2);
                        GameManager.Instance.AddRemoveMultiplier(f_var * i_var2);
                        break;
                    case "cashAdd":
                        i_var = int.Parse(item.effectVar);
                        GameManager.Instance.AddRemoveCash(i_var);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    static int GetVar(string _var)
    {
        switch (_var)
        {
            case "heldCards":
                return PlayerData.C_HandCards.Count;
            default:
                break;
        }
        Debug.LogWarning("Couldn't Find VarType: " + _var);
        return 0;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{ 
    [Header("Cards")]
    public static List<VarClass.cardClass> C_EquippedCards = new List<VarClass.cardClass>();    //All Cards Player Has On Current Run
    public static List<VarClass.cardClass> C_DeckCards = new List<VarClass.cardClass>();        //Cards still yet to be drawn
    public static List<VarClass.cardClass> C_HandCards = new List<VarClass.cardClass>();        //Cards in hand at the moment
    public static List<VarClass.cardClass> C_UsedCards = new List<VarClass.cardClass>();        //Cards that have been used or discarded

    [Header("Troops")]
    public static List<List<VarClass.troopClass>> T_EquippedTroops = new List<List<VarClass.troopClass>>();

    [Header("Game Modifiers")]
    public static List<VarClass.gameModifierClass> M_EquippedMods = new List<VarClass.gameModifierClass>();

    public static List<VarClass.cardClass> CloneEquippedCards()
    {
        List<VarClass.cardClass> _temp = new List<VarClass.cardClass>();
        for (int i = 0; i < C_EquippedCards.Count; i++)
            _temp.Add(C_EquippedCards[i].Clone());

        return _temp;
    }
    public static List<VarClass.cardClass> CopyEquippedCards()
    {
        List<VarClass.cardClass> _temp = new List<VarClass.cardClass>();
        for (int i = 0; i < C_EquippedCards.Count; i++)
            _temp.Add(C_EquippedCards[i]);

        return _temp;
    }

    public static int I_maxHandSize = 4;                                                        //Max Cards Players Can Have In Hand
    public static VarClass.challengeClass C_CurrentChallenge;                                   //The Current Challenge the player is attempting
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{
    public TroopList troopList;
    public CardList cardList;
    public DifficultyList difficultyList;

    public TroopModList troopModList;
    public CardModList cardModList;
    public GameModList gameModList;

    public RoundSkipList roundSkipList;

    public StorePackList storePackList;

    public ChallengeList challengeList;
    void Awake()
    {
        TroopList.Instance = troopList;
        CardList.Instance = cardList;
        DifficultyList.Instance = difficultyList;

        TroopModList.Instance = troopModList;
        CardModList.Instance = cardModList;
        GameModList.Instance = gameModList;

        RoundSkipList.Instance = roundSkipList;

        StorePackList.Instance = storePackList;

        ChallengeList.Instance = challengeList;

        gameModList.OnAwake();
    }
}

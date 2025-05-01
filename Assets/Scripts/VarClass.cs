using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarClass : MonoBehaviour
{
    [System.Serializable]
    public class troopClass
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public bool isPlayer = true;
        public bool hasMoved = false;

        public troopModifierClass mod = new troopModifierClass();
        [HideInInspector] public List<TileEffect> modEffects = new List<TileEffect>();

        public AddressableObjectClass objAddressable = new AddressableObjectClass();
        public AddressableSpriteClass sprAddressable = new AddressableSpriteClass();

        public intArray[] movementArray = new intArray[0];

        public int points = 1;
        public int getPoints() { return points.ApplyModifier("troopValue", id); }
        public int getUnmodifiedPoints() { return points; }

        public int multiplier = 1;
        public int getMultiplier() { return multiplier.ApplyModifier("troopValue", id); }
        public int getUnmodifiedMultiplier() { return multiplier; }

        public string switchOnReachEnd = "";
        public int extraTurnsOnEnemyKill = 0;
        public int getExtraTurnsOnKill() { return extraTurnsOnEnemyKill.ApplyModifier("troopTurn", id); }

        public troopClass Clone()
        {
            troopClass temp = new troopClass();
            temp.id = id;
            temp.name = name;
            temp.description = description;

            temp.mod = mod;

            temp.objAddressable = objAddressable;
            temp.sprAddressable = sprAddressable;

            temp.movementArray = movementArray;

            temp.points = points;
            temp.multiplier = multiplier;

            temp.switchOnReachEnd = switchOnReachEnd;
            temp.extraTurnsOnEnemyKill = extraTurnsOnEnemyKill;
            return temp;
        }

        int getMoveInfo(int x, int y)
        {
            if (isPlayer)
                y = 6 - y;
            else
                x = 6 - x;

            return movementArray[y].array[x];
        }

        public intArray[] getMoveArray()
        {
            intArray[] _temp1 = movementArray.ApplyModifier(id);
            intArray[] _temp2 = (intArray[])_temp1.Clone();
            if (isPlayer)
                for (int y = 0; y < 7; y++)
                    _temp2[y] = _temp1[6 - y];
            else
                for (int x = 0; x < 7; x++)
                    for (int y = 0; y < 7; y++)
                        _temp2[y].array[x] = _temp1[y].array[6 - x];

            return _temp2;
        }
    }

    [System.Serializable]
    public class cardClass
    {
        public string id = "";
        public string name = "";
        public string description = "";
        public string cardType = "";

        public AddressableSpriteClass sprAddressable = new AddressableSpriteClass();

        public cardModifierClass cardMod = new cardModifierClass();

        public int targetTile = 0;
        public intArray[] targetArray = new intArray[0];

        public string effectID = "";
        public string effectVar = "";

        public int cost = 1;
        public int weight = 1;

        public cardClass Clone()
        {
            cardClass temp = new cardClass();
            temp.id = id;
            temp.name = name;
            temp.description = description;
            temp.cardType = cardType;

            temp.sprAddressable = sprAddressable;

            temp.cardMod = cardMod;

            temp.targetTile = targetTile;
            temp.targetArray = targetArray;

            temp.effectID = effectID;
            temp.effectVar = effectVar;

            temp.cost = cost;
            temp.weight = weight;
            return temp;
        }


        public int getTargetInfo(int x, int y)
        {
            //isPlayer
                y = 6 - y;

            return targetArray[y].array[x];
        }
    }

    [System.Serializable]
    public class difficultyClass
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public AddressableSpriteClass sprAddressable = new AddressableSpriteClass();

        public int roundsPerStage = 0;
        public float multiplierPerRound = 0f;

        public numberClass tarScore = new numberClass();

        public double GetTarScore(int _round, int _stage)
        {
            double _temp = tarScore.GetNumAtLevel(_stage);
            _temp *= 1 + ((_round - 1) * multiplierPerRound);
            return _temp;
        }

        public difficultyClass Clone()
        {
            difficultyClass temp = new difficultyClass();
            temp.id = id;
            temp.name = name;
            temp.description = description;

            temp.sprAddressable = sprAddressable;

            temp.roundsPerStage = roundsPerStage;
            temp.multiplierPerRound = multiplierPerRound;

            temp.tarScore = tarScore.Clone();
            return temp;
        }
    }

    [System.Serializable]
    public class intArray
    {
        public int[] array = new int[0];
    }
    [System.Serializable]
    public class stringArray
    {
        public string[] array = new string[0];
    }
    [System.Serializable]
    public class boolArray
    {
        public bool[] array = new bool[0];
    }

    [System.Serializable]
    public class numberClass
    {
        public float Base = 0;
        public float Linear = 0;
        public float Coefficient2 = 0;
        public float Coefficient3 = 0;
        public float MilestoneRate = 0;
        public float MilestoneMultiplier = 0;

        public float GetNumAtLevel(int level)
        {
            float temp =
                Base
                + (Linear * (level - 1))
                + (Coefficient2 * Mathf.Pow(level - 1, 2))
                + (Coefficient3 * Mathf.Pow(level - 1, 3))
                ;
            if (MilestoneRate > 0)
                temp *= 1 + (MilestoneMultiplier * Mathf.Floor(level / MilestoneRate));

            return temp;
        }

        public numberClass Clone()
        {
            numberClass temp = new numberClass();
            temp.Base = Base;
            temp.Linear = Linear;
            temp.Coefficient2 = Coefficient2;
            temp.Coefficient3 = Coefficient3;
            temp.MilestoneRate = MilestoneRate;
            temp.MilestoneMultiplier = MilestoneMultiplier;
            return temp;
        }
    }

    [System.Serializable]
    public class troopModifierClass
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public Vector4 colorVector = Vector4.zero;

        public AddressableSpriteClass sprAddressable = new AddressableSpriteClass();
        public int weight = 0;

        public string triggerType = "";
        public intArray[] targetArray = new intArray[0];

        public string effectID = "";
        public string effectVar = "";

        public int getTargetInfo(int x, int y)
        {
            //isPlayer
            y = 6 - y;

            return targetArray[y].array[x];
        }
        public Color color()
        {
            return new Color(colorVector.x, colorVector.y, colorVector.z, colorVector.w);
        }

        public cardClass ConvertToCard()
        {
            cardClass _card = new cardClass();

            _card.effectID = effectID;
            _card.effectVar = effectVar;
            _card.targetArray = targetArray;
            return _card;
        }

        public List<TileEffect> ApplyEffectList(Vector2Int _pos, bool _isPlayer)
        {
            GameManager GM = GameManager.Instance;
            VarClass.cardClass _card = ConvertToCard();
            List<TileEffect> _TEList = new List<TileEffect>();

            Vector2Int _tar;
            Vector2Int _dir;
            Vector2Int _boardSize = GM.V2_boardSize;
            TileEffect _TE;

            for (int x = 1; x <= 5; x++)
            {
                for (int y = 1; y <= 5; y++)
                {
                    _dir = new Vector2Int(x - 3, y - 3);
                    _tar = _pos + _dir;
                    if (GM.CheckOnBoard(_tar))
                    {
                        int _moveType = _card.getTargetInfo(x, y);
                        if (_moveType != 0 && _moveType != 100)
                        {
                            _TE = TileEffect.Create(_tar, _isPlayer, effectID, effectVar, _moveType);
                            GM.GetBoardPanel(_tar).AddSpecificPassiveTileEffect(_TE);
                            _TEList.Add(_TE);
                        }
                    }
                }
            }
            for (int x = 0; x <= 6; x++)
            {
                int _moveType = _card.getTargetInfo(x, 0);
                if (_moveType > 0)
                {
                    for (int i = 0; i < Mathf.Max(_boardSize.x, _boardSize.y); i++)
                    {
                        _dir = new Vector2Int(x - 3 + Mathf.RoundToInt(((float)(x - 3) / 3) * i), -3 - i);
                        _tar = _pos + _dir;
                        if (GM.CheckOnBoard(_tar))
                        {
                            if (_moveType != 0 && _moveType != 100)
                            {
                                _TE = TileEffect.Create(_tar, _isPlayer, effectID, effectVar, _moveType);
                                GM.GetBoardPanel(_tar).AddSpecificPassiveTileEffect(_TE);
                                _TEList.Add(_TE);
                            }
                        }
                    }
                }
                _moveType = _card.getTargetInfo(x, 6);
                if (_moveType > 0)
                {
                    for (int i = 0; i < Mathf.Max(_boardSize.x, _boardSize.y); i++)
                    {
                        _dir = new Vector2Int(x - 3 + Mathf.RoundToInt(((float)(x - 3) / 3) * i), 3 + i);
                        _tar = _pos + _dir;
                        if (GM.CheckOnBoard(_tar))
                        {
                            if (_moveType != 0 && _moveType != 100)
                            {
                                _TE = TileEffect.Create(_tar, _isPlayer, effectID, effectVar, _moveType);
                                GM.GetBoardPanel(_tar).AddSpecificPassiveTileEffect(_TE);
                                _TEList.Add(_TE);
                            }
                        }
                    }
                }
            }
            for (int y = 1; y <= 5; y++)
            {
                int _moveType = _card.getTargetInfo(0, y);
                if (_moveType > 0)
                {
                    for (int i = 0; i < Mathf.Max(_boardSize.x, _boardSize.y); i++)
                    {
                        _dir = new Vector2Int(-3 - i, y - 3 + Mathf.RoundToInt(((float)(y - 3) / 3) * i));
                        _tar = _pos + _dir;
                        if (GM.CheckOnBoard(_tar))
                        {
                            if (_moveType != 0 && _moveType != 100)
                            {
                                _TE = TileEffect.Create(_tar, _isPlayer, effectID, effectVar, _moveType);
                                GM.GetBoardPanel(_tar).AddSpecificPassiveTileEffect(_TE);
                                _TEList.Add(_TE);
                            }
                        }
                    }
                }
                _moveType = _card.getTargetInfo(6, y);
                if (_moveType > 0)
                {
                    for (int i = 0; i < Mathf.Max(_boardSize.x, _boardSize.y); i++)
                    {
                        _dir = new Vector2Int(3 + i, y - 3 + Mathf.RoundToInt(((float)(y - 3) / 3) * i));
                        _tar = _pos + _dir;
                        if (GM.CheckOnBoard(_tar))
                        {
                            if (_moveType != 0 && _moveType != 100)
                            {
                                _TE = TileEffect.Create(_tar, _isPlayer, effectID, effectVar, _moveType);
                                GM.GetBoardPanel(_tar).AddSpecificPassiveTileEffect(_TE);
                                _TEList.Add(_TE);
                            }
                        }
                    }
                }
            }
            return _TEList;
        }
    }
    [System.Serializable]
    public class TileEffect
    {
        public Vector2Int pos = Vector2Int.zero;
        public bool isPlayer = true;
        public string effectID = "";
        public string effectVar = "";
        public int targetTile = 0;

        public static TileEffect Create(Vector2Int _pos, bool _isPlayer, string _effectID, string _effectVar, int _targetTile)
        {
            TileEffect _temp = new TileEffect();
            _temp.pos = _pos;
            _temp.isPlayer = _isPlayer;
            _temp.effectID = _effectID;
            _temp.effectVar = _effectVar;
            _temp.targetTile = _targetTile;
            return _temp;
        }
    }

    [System.Serializable]
    public class cardModifierClass
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public AddressableSpriteClass sprAddressable = new AddressableSpriteClass();
        public int cost = 0;
        public int weight = 0;

        public string triggerType = "";

        public string effectID = "";
        public string effectVar = "";
    }

    [System.Serializable]
    public class gameModifierClass
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public AddressableSpriteClass sprAddressable = new AddressableSpriteClass();

        public int cost = 0;
        public int weight = 0;

        public string effectID = "";
        public string effectVar = "";
        public string effectVar2 = "";
        public string triggerID = "";
    }

    [System.Serializable]
    public class roundSkipReward
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public AddressableSpriteClass sprAddressable = new AddressableSpriteClass();

        public int weight = 0;

        public string effectID = "";
        public string effectVar = "";
    }

    [System.Serializable]
    public class storePackClass
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public AddressableSpriteClass sprAddressable = new AddressableSpriteClass();

        public int cost = 0;
        public int weight = 0;
        public string packType = "";
        public int amt = 0;
        public int pool = 0;
    }

    [System.Serializable]
    public class challengeClass
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public stringArray[] troopIDs = new stringArray[0];
        public boolArray[] isEnemy = new boolArray[0];

        public int turnLimit = 0;

        public string[] cards = new string[0];
        public string[] gameModifiers = new string[0];

        public int sizeX = 0;
    }

    [System.Serializable]
    public class AddressableObjectClass
    {
        public string id = "";
        public GameObject obj = null;
    }

    [System.Serializable]
    public class AddressableSpriteClass
    {
        public string id = "";
        public Sprite spr = null;
    }
}

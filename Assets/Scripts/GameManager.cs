using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance;
    [Header("Camera References")]
    public Camera C_mainCam;

    [Header("UI References")]
    public TextMeshProUGUI TM_points;
    public TextMeshProUGUI TM_multiplier;
    public TextMeshProUGUI TM_totalScore;

    public TextMeshProUGUI TM_targetScore;
    public TextMeshProUGUI TM_reward;

    public TextMeshProUGUI TM_round;
    public TextMeshProUGUI TM_turns;
    public TextMeshProUGUI TM_cash;

    public TroopInfoUI TIUI_troopInfo;

    public Image I_OverallDarkenator;

    public GameObject G_ChallengeHeader;
    public GameObject G_RoundHolder;
    public GameObject G_CashHolder;
        
    [Header("Win Panel")]
    public GameObject G_winPanel;
    public TextMeshProUGUI TM_WP_totalScore;
    public TextMeshProUGUI TM_WP_targetScore;
    public TextMeshProUGUI TM_WP_scoreReward;
    public TextMeshProUGUI TM_WP_totalReward;
    public TextMeshProUGUI TM_WP_remainingTurns;
    public TextMeshProUGUI TM_WP_remainingTurnsReward;

    public Image I_WP_Darkenator;
    public RectTransform RT_WP_Header;
    public RectTransform RT_WP_Content;
    public RectTransform RT_WP_Claim;

    [Header("Lose Panel")]
    public GameObject G_losePanel;
    public TextMeshProUGUI TM_LP_totalScore;
    public TextMeshProUGUI TM_LP_targetScore;

    public Image I_LP_Darkenator;
    public RectTransform RT_LP_Header;
    public RectTransform RT_LP_Content;

    [Header("Store Panel")]
    public GameObject G_storePanel;
    public Transform T_storeFront;
    public Transform T_packs;
    public Transform T_stageMods;
    public float F_storeItemGap = 200;
    private List<StoreItem> si_storeFront = new List<StoreItem>();
    private List<StoreItem> si_packs = new List<StoreItem>();
    private List<StoreItem> si_stageMods = new List<StoreItem>();

    public TextMeshProUGUI TM_rerollCost;

    [Header("Card Select Panel")]
    public GameObject G_cardSelect;
    public Transform T_cardSelectHolder;
    public TextMeshProUGUI TM_cardSelectAmt;
    public float F_cardItemGap = 200;
    private VarClass.storePackClass sp_curPack;
    private List<StoreItem> si_cardSelectItems = new List<StoreItem>();
    private List<StoreItem> si_cardSelectedItems = new List<StoreItem>();
    [Space(10)]

    [Header("Round Select Panel")]
    public GameObject G_roundSelectPanel;
    public Transform T_roundSelectHolder;
    public float F_roundSelectGap = 350;
    private List<RoundSelectObject> RSO_roundSelects = new List<RoundSelectObject>();
    private List<VarClass.roundSkipReward> RSR_skipRewards = new List<VarClass.roundSkipReward>();

    [Header("Pause Panel")]
    public RectTransform RT_gameModHolder;
    private List<GameModifierDetailUI> GMD_gameModDetails = new List<GameModifierDetailUI>();

    [Header("Board References")]
    public Transform T_boardHolder;
    public Transform T_boardEnv;
    public Material[] M_boardPanelMaterials;
    private BoardPanel[,] BP_List = null;

    [Header("Board Values")]
    [HideInInspector] public Vector2Int V2_boardSize = new Vector2Int(4, 6);
    private Vector2Int[] v2_boardSizeByStage = { new Vector2Int(4, 6), new Vector2Int(6, 6), new Vector2Int(8, 8) };

    public LayerMask LM_boardRaycast;

    [Header("Card References")]
    public Transform T_cardHolder;

    private List<CardPrefab> CP_handCards = new List<CardPrefab>();

    [Header("Prefab References")]
    public RoundSelectObject PF_RoundSelectObject;
    public BoardPanel PF_boardPanel;
    public CardPrefab PF_cardPrefab;
    public StoreItem PF_storeItem;
    public GameModifierDetailUI PF_gameModDetail;

    [Header("FTUE Objects")]
    public List<GameObject> G_FTUEPanels;

    [Header("Color Values")]
    public Color C_selBoardColor;
    public Color C_potMoveBoardColor;
    public Color C_potAttackBoardColor;
    [Space(10)]
    public Color C_playerTroop;
    public Color C_enemyTroop;

    [Header("Animations")]
    public AnimCurve_Scriptable A_overShoot;

    private BoardPanel BP_selPanel = null;
    private CardPrefab CP_selCard = null;

    private List<BoardPanel> BP_potMovement = new List<BoardPanel>();

    VarClass.difficultyClass DC_curDifficulty;
    VarClass.challengeClass CC_curChallenge;

    private int I_turnNum = 0;
    private int I_maxTurns = 8;
    
    private int I_roundNum = 1;
    private int I_stageNum = 1;
    private int I_cash = 0;

    private int I_rerollCost = 5;

    private double D_points = 0;
    private double D_multSubTotal = 0;
    private double D_multTotal = 0;
    private double D_totalScore = 0;

    private double D_tarScore = 0;
    private int I_rewardCash = 0;

    private int i_extraTurnAmt = 0;

    private bool b_challengeMode = false;

    public gameStateEnum GameState = gameStateEnum.playerTurn;
    public enum gameStateEnum { playerTurn, playerPending, playerMultiTurn, enemyTurn, enemyPending, enemyMultiTurn, win, loss, store, roundSelect};
    #endregion

    #region Awake/Start
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(Panel_Darkenator(I_OverallDarkenator, 1, false));

        CC_curChallenge = PlayerData.C_CurrentChallenge;
        if (CC_curChallenge != null)
            b_challengeMode = true;
        G_ChallengeHeader.SetActive(b_challengeMode);
        G_CashHolder.SetActive(!b_challengeMode);
        G_RoundHolder.SetActive(!b_challengeMode);

        DC_curDifficulty = DifficultyList.Instance.list[0];

        FTUE_ShowPanel(0);
        RestartGame();
    }
    #endregion

    #region Update
    void Update()
    {
        switch (GameState)
        {
            case gameStateEnum.playerTurn:
                Update_PlayerTurn();
                break;
            case gameStateEnum.playerMultiTurn:
                Update_PlayerMultiTurn();
                break;
            case gameStateEnum.enemyTurn:
                    GameState = gameStateEnum.enemyPending;
                    StartCoroutine(Update_EnemyTurn());
                break;
            case gameStateEnum.enemyMultiTurn:
                    GameState = gameStateEnum.enemyPending;
                    StartCoroutine(Update_EnemyMultiTurn());
                break;
            default:
                break;
        }
    }

    void Update_PlayerTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(C_mainCam.ScreenPointToRay(Input.mousePosition), out hit, 100, LM_boardRaycast))
            {
                BoardPanel _BP;
                if (hit.transform.TryGetComponent<BoardPanel>(out _BP))
                {
                    if (BP_potMovement.Contains(_BP))
                    {
                        if (BP_selPanel != null)
                        {
                            GameState = gameStateEnum.playerPending;
                            _BP.MoveFrom(BP_selPanel);
                            if(_BP.troop.mod.triggerType == "onMove")
                                ActivateTroopModEffect(_BP.troop, _BP.pos);
                            ClearSelPanel();
                        }
                        if (CP_selCard != null)
                        {
                            List<BoardPanel> _targets = GetCardTargets(CP_selCard.card, _BP.pos, true);
                            if (_targets.Count > 0)
                            {
                                foreach (var item in _targets)
                                {
                                    ApplyCardEffect(CP_selCard.card, item);
                                }
                            }
                            CP_selCard.OnUsed();

                            switch (CP_selCard.card.cardType)
                            {
                                case "cardAction":
                                    break;
                                case "cardModifier":
                                    PlayerData.C_EquippedCards.Remove(CP_selCard.card);
                                    break;
                                default:
                                    break;
                            }

                            PlayerData.C_HandCards.Remove(CP_selCard.card);
                            PlayerData.C_UsedCards.Add(CP_selCard.card);
                            CP_selCard = null;
                            ClearSelPanel();
                        }
                    }
                    else
                    {
                        BoardPanel _temp = BP_selPanel;
                        ClearSelPanel();
                        if (_BP != _temp)
                        {
                            BP_selPanel = _BP;
                            _BP.SetColor(C_selBoardColor);
                            TIUI_troopInfo.LoadTroop(_BP.troop);

                            if (_BP.IsAvailable())
                                if (_BP.IsPlayerEnemy(true, true))
                                    BP_potMovement = GetPotMovements(BP_selPanel, true, false);
                        }
                    }
                    return;
                }
                CardPrefab _CP;
                if (hit.transform.TryGetComponent<CardPrefab>(out _CP))
                {
                    if (CP_selCard != _CP)
                    {
                        ClearSelPanel();
                        CP_selCard = _CP;
                        CP_selCard.SelectPos();
                        BP_potMovement = GetPotCardPlacement(true);
                    }
                    else
                        ClearSelPanel();
                    return;
                }
            }
        }
    }
    void Update_PlayerMultiTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(C_mainCam.ScreenPointToRay(Input.mousePosition), out hit, 100, LM_boardRaycast))
            {
                BoardPanel _BP;
                if (hit.transform.TryGetComponent<BoardPanel>(out _BP))
                {
                    if (BP_potMovement.Contains(_BP))
                    {
                        GameState = gameStateEnum.playerPending;
                        _BP.MoveFrom(BP_selPanel);
                        if (_BP.troop.mod.triggerType == "onMove")
                            ActivateTroopModEffect(_BP.troop, _BP.pos);
                        ClearSelPanel();
                    }
                    else
                    {
                        ClearSelPanel();
                        EndTurn(true);
                    }
                    return;
                }
                CardPrefab _CP;
                if (hit.transform.TryGetComponent<CardPrefab>(out _CP))
                {
                    ClearSelPanel();
                    EndTurn(true);
                    return;
                }
            }
        }
    }
    public void MultiTurn(BoardPanel _BP)
    {
        if (_BP.troop.getExtraTurnsOnKill() > i_extraTurnAmt)
        {
            i_extraTurnAmt++;
            BP_selPanel = _BP;
            if (_BP.troop.isPlayer)
            {
                GameState = gameStateEnum.playerMultiTurn;
                
                BP_potMovement = GetPotMovements(BP_selPanel, true, true);
                if (BP_potMovement.Count == 0)
                    EndTurn(_BP.troop.isPlayer);
            }
            else
            {
                GameState = gameStateEnum.enemyMultiTurn;
            }
        }
        else
            EndTurn(_BP.troop.isPlayer);
    }
    class MoveInfo
    {
        public BoardPanel from;
        public BoardPanel to;
        public float weighting = -1;
    }

    IEnumerator Update_EnemyTurn()
    {
        List<BoardPanel> _enemies = GetAllTroops(false);
        MoveInfo _move = new MoveInfo();
        yield return new WaitForSeconds(1f);
        foreach (var item in _enemies)
        {
            List<BoardPanel> _movements = GetPotMovements(item, false, false);
            foreach (var item2 in _movements)
            {
                float _weight = 0;
                if (item2.IsAvailable())
                {
                    if (item2.IsPlayerEnemy(true, false))
                        _weight = item2.troop.getPoints();
                    if (_weight > _move.weighting)
                    {
                        _move.weighting = _weight;
                        _move.to = item2;
                        _move.from = item;
                    }
                    else if (_weight == _move.weighting)
                    {
                        if (Random.Range(0f, 1f) > 0.5f)
                        {
                            _move.weighting = _weight;
                            _move.to = item2;
                            _move.from = item;
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        if (_move.to != null)
            _move.to.MoveFrom(_move.from);
        else
            EndTurn(false);
    }

    IEnumerator Update_EnemyMultiTurn()
    {
        List<BoardPanel> _enemies = new List<BoardPanel>();
        _enemies.Add(BP_selPanel);
        MoveInfo _move = new MoveInfo();
        yield return new WaitForSeconds(0.1f);
        foreach (var item in _enemies)
        {
            List<BoardPanel> _movements = GetPotMovements(item, false, true);
            foreach (var item2 in _movements)
            {
                float _weight = 0;
                if (item2.IsAvailable())
                {
                    if (item2.IsPlayerEnemy(true, false))
                        _weight = item2.troop.getPoints();
                    if (_weight > _move.weighting)
                    {
                        _move.weighting = _weight;
                        _move.to = item2;
                        _move.from = item;
                    }
                    else if (_weight == _move.weighting)
                    {
                        if (Random.Range(0f, 1f) > 0.5f)
                        {
                            _move.weighting = _weight;
                            _move.to = item2;
                            _move.from = item;
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        if (_move.to != null)
            _move.to.MoveFrom(_move.from);
        else
            EndTurn(false);
    }
    #endregion

    #region Effects
    public void ActivateTroopModEffect (VarClass.troopClass _troop, Vector2Int _pos)
    {
        VarClass.cardClass _card = _troop.mod.ConvertToCard();
        List<BoardPanel> _targets = GetCardTargets(_card, _pos, _troop.isPlayer);
        if (_targets.Count > 0)
        {
            foreach (var item in _targets)
            {
                ApplyCardEffect(_card, item);
            }
        }
    }

    void ApplyCardEffect(VarClass.cardClass _card, BoardPanel _BP)
    {
        VarClass.troopClass _troop;
        VarClass.troopModifierClass _troopMod;
        switch (_card.effectID)
        {
            case "troopSpawn":
                _troop = TroopList.Instance.FindTroopByID(_card.effectVar).Clone();
                if (_troop != null && _BP.troop == null)
                    _BP.CreateTroop(_troop,true,this);
                    break;
            case "troopDefeat":
                _BP.DestroyChild();
                break;
            case "troopPointMultiply":
                float _var = float.Parse(_card.effectVar);
                int _prevScore = _BP.troop.getPoints();
                _BP.troop.points = Mathf.RoundToInt(_BP.troop.getUnmodifiedPoints() * _var);
                AddRemovePoints(_BP.troop.getPoints() - _prevScore);
                break;
            case "troopControl":
                _BP.SwitchTeam(true, this);
                break;
            case "troopChange":
                _troop = TroopList.Instance.FindTroopByID(_card.effectVar).Clone();
                if (_troop != null)
                    _BP.SwitchTroop(_troop, true, this);
                break;
            case "troopModifier":
                _troopMod = TroopModList.Instance.FindModifierByID(_card.effectVar);
                if (_troopMod != null)
                    _BP.ApplyTroopModifier(_troopMod);
                break;
            default:
                Debug.LogError("Can't Find Effect: " + _card.effectID);
                break;
        }

        if (_card.cardMod.triggerType == "onPlay")
            ApplyCardModEffect(_card);
    }

    void ApplyCardModEffect(VarClass.cardClass _card)
    {
        switch (_card.cardMod.effectID)
        {
            case "pointAdd":
                AddRemovePoints(int.Parse(_card.cardMod.effectVar));
                break;
            case "multiplierAdd":
                AddRemoveMultiplier(int.Parse(_card.cardMod.effectVar));
                break;
            default:
                break;
        }
    }

    #endregion

    #region Object Creation
    void CreateStartTroops()
    {
        int troopMax = TroopList.Instance.list.Count;
        int[] enemyRow1 = { 3, 1, 2, 4, 5, 2, 1, 3 };
        int[] enemyRow2 = { 0, 0, 0, 0, 0, 0, 0, 0 };


        List<List<VarClass.troopClass>> _playerLineup = new List<List<VarClass.troopClass>>();
        for (int i = 0; i < V2_boardSize.x; i++)
            _playerLineup.Add(PlayerData.T_EquippedTroops[i]);

        _playerLineup.Shuffle();

        for (int x = 0; x < V2_boardSize.x; x++)
        {
            //_temp.mod = TroopModList.Instance.GetRandom();
            if (x < _playerLineup.Count)
            {
                for (int y = 0; y < _playerLineup[x].Count; y++)
                {
                    if (_playerLineup[x][y] != null)
                        BP_List[x, y].CreateTroop(_playerLineup[x][y], true, this);
                }
            }
            BP_List[x, V2_boardSize.y - 1].CreateTroop(TroopList.Instance.list[enemyRow1[x]].Clone(), false, this);
            BP_List[x, V2_boardSize.y - 2].CreateTroop(TroopList.Instance.list[enemyRow2[x]].Clone(), false, this);
        }
    }

    void CHALLENGE_CreateStartTroops()
    {
        for (int x = 0; x < V2_boardSize.x; x++)
        {
            for (int y = 0; y < V2_boardSize.y; y++)
            {
                string _id = CC_curChallenge.troopIDs[y].array[x];
                if (_id !="Empty")
                {
                    if (_id == "Null")
                    {
                        BP_List[x, V2_boardSize.y - y - 1].Remove();
                    }
                    else
                    {
                        VarClass.troopClass _troop = TroopList.Instance.FindTroopByID(_id).Clone();
                        bool _enemy = CC_curChallenge.isEnemy[y].array[x];
                        BP_List[x, V2_boardSize.y - y - 1].CreateTroop(_troop, !_enemy, this);
                    }
                }
            }
        }
    }

    void DrawCards(int amt)
    {
        amt = Mathf.Min(amt, PlayerData.C_DeckCards.Count, PlayerData.I_maxHandSize.ApplyModifier("cardLimit") - PlayerData.C_HandCards.Count);
        for (int i = 0; i < amt; i++)
        {
            int _ran = Random.Range(0, PlayerData.C_DeckCards.Count);
            CardPrefab CP = Instantiate(PF_cardPrefab, T_cardHolder);
            CP.transform.localPosition = new Vector3(0, 0, -5);
            CP.OnCreate(PlayerData.C_DeckCards[_ran]);
            CP_handCards.Add(CP);
            PlayerData.C_HandCards.Add(PlayerData.C_DeckCards[_ran]);
            PlayerData.C_DeckCards.RemoveAt(_ran);
        }
        UpdateCardPositions();
    }

    void ClearCards()
    {
        for (int i = CP_handCards.Count - 1; i >= 0; i--)
        {
            Destroy(CP_handCards[i].gameObject);
        }
        CP_handCards.Clear();
        PlayerData.C_HandCards.Clear();
        PlayerData.C_UsedCards.Clear();
        UpdateCardPositions();
    }

    void UpdateCardPositions()
    {
        for (int i = CP_handCards.Count -1; i >= 0; i--)
        {
            if (CP_handCards[i].B_Used)
            {
                CP_handCards[i].OnRemove();
                CP_handCards.RemoveAt(i);
            }
        }
        for (int i = CP_handCards.Count - 1; i >= 0; i--)
        {
            float _xPos = i * 2 - (((float)CP_handCards.Count - 1) / 2) * 2;
            CP_handCards[i].UpdateBasePos(new Vector3(_xPos, 0, 0));
        }
    }
    #endregion

    #region Board Selection
    public BoardPanel GetBoardPanel(Vector2Int _pos) { return GetBoardPanel(_pos.x, _pos.y); }
    public BoardPanel GetBoardPanel(int x, int y) { return BP_List[x, y]; }

    List<BoardPanel> GetPotMovements(BoardPanel _selPanel, bool _applyColor, bool _needsKill)
    {
        List<BoardPanel> _tempBP = new List<BoardPanel>();
        VarClass.troopClass _troop = _selPanel.troop;
        if (_troop == null)
            return _tempBP;
        Vector2Int _pos = _selPanel.pos;
        Vector2Int _tar;
        Vector2Int _dir;
        List<Vector3Int> _posList = new List<Vector3Int>();

        VarClass.intArray[] _moveArray = _troop.getMoveArray();

        for (int x = 1; x <= 5; x++)
        {
            for (int y = 1; y <= 5; y++)
            {
                if (x == 3 && y == 3)
                    continue;
                _dir = new Vector2Int(x - 3, y - 3);
                _tar = _pos + _dir;
                int _moveType = _moveArray[y].array[x];
                if (CheckMovePosition(_tar, _moveType, _pos, _troop, _needsKill))
                    _posList.Add(new Vector3Int(_tar.x, _tar.y, _moveType));

            }
        }
        for (int x = 0; x <= 6; x++)
        {
            int _moveType = _moveArray[0].array[x];
            if (_moveType > 0)
            {
                for (int i = 0; i < Mathf.Max(V2_boardSize.x, V2_boardSize.y); i++)
                {
                    _dir = new Vector2Int(x - 3 + Mathf.RoundToInt(((float)(x - 3) / 3) * i), -3 - i);
                    _tar = _pos + _dir;
                    if (CheckMovePosition(_tar, _moveType, _pos, _troop, _needsKill))
                        _posList.Add(new Vector3Int(_tar.x, _tar.y, _moveType));
                }
            }
            _moveType = _moveArray[6].array[x];
            if (_moveType > 0)
            {
                for (int i = 0; i < Mathf.Max(V2_boardSize.x, V2_boardSize.y); i++)
                {
                    _dir = new Vector2Int(x - 3 + Mathf.RoundToInt(((float)(x - 3) / 3) * i), 3 + i);
                    _tar = _pos + _dir;
                    if (CheckMovePosition(_tar, _moveType, _pos, _troop, _needsKill))
                        _posList.Add(new Vector3Int(_tar.x, _tar.y, _moveType));
                }
            }
        }
        for (int y = 1; y <= 5; y++)
        {
            int _moveType = _moveArray[y].array[0];
            if (_moveType > 0)
            {
                for (int i = 0; i < Mathf.Max(V2_boardSize.x, V2_boardSize.y); i++)
                {
                    _dir = new Vector2Int(-3 - i, y - 3 + Mathf.RoundToInt(((float)(y - 3) / 3) * i));
                    _tar = _pos + _dir;
                    if (CheckMovePosition(_tar, _moveType, _pos, _troop, _needsKill))
                        _posList.Add(new Vector3Int(_tar.x, _tar.y, _moveType));
                }
            }
            _moveType = _moveArray[y].array[6];
            if (_moveType > 0)
            {
                for (int i = 0; i < Mathf.Max(V2_boardSize.x, V2_boardSize.y); i++)
                {
                    _dir = new Vector2Int(3 + i, y - 3 + Mathf.RoundToInt(((float)(y - 3) / 3) * i));
                    _tar = _pos + _dir;
                    if (CheckMovePosition(_tar, _moveType, _pos, _troop, _needsKill))
                        _posList.Add(new Vector3Int(_tar.x, _tar.y, _moveType));
                }
            }
        }
        foreach (var item in _posList)
        {
            _tempBP.Add(BP_List[item.x, item.y]);
            if (_applyColor)
            {
                if (BP_List[item.x, item.y].troop == null)
                    BP_List[item.x, item.y].SetColor(C_potMoveBoardColor);
                else
                    BP_List[item.x, item.y].SetColor(C_potAttackBoardColor);
            }
        }
        return _tempBP;
    }

    List<BoardPanel> GetPotCardPlacement(bool _isPlayer)
    {
        List<BoardPanel> _tempBP = new List<BoardPanel>();
        VarClass.cardClass _card = CP_selCard.card;
        if (_card == null)
            return _tempBP;
        List<Vector3Int> _posList = new List<Vector3Int>();

        for (int x = 0; x < V2_boardSize.x; x++)
        {
            for (int y = 0; y < V2_boardSize.y; y++)
            {
                if (CheckEffectPosition(new Vector2Int(x,y), _card.targetTile, _isPlayer))
                    _posList.Add(new Vector3Int(x, y, _card.targetTile));
            }
        }

        foreach (var item in _posList)
        {
            _tempBP.Add(BP_List[item.x, item.y]);
            if (BP_List[item.x, item.y].troop == null)
                BP_List[item.x, item.y].SetColor(C_potMoveBoardColor);
            else
                BP_List[item.x, item.y].SetColor(C_potAttackBoardColor);
        }
        return _tempBP;
    }

    List<BoardPanel> GetAllTroops(bool player)
    {
        List<BoardPanel> _temp = new List<BoardPanel>();
        foreach (var item in BP_List)
        {
            if (item.IsAvailable())
                if (item.IsPlayerEnemy(player, false))
                    _temp.Add(item);
        }
        return _temp;
    }

    public List<BoardPanel> GetCardTargets(VarClass.cardClass _card, Vector2Int _pos, bool _isPlayer)
    {
        Vector2Int _tar;
        Vector2Int _dir;
        List<BoardPanel> _panelList = new List<BoardPanel>();

        for (int x = 1; x <= 5; x++)
        {
            for (int y = 1; y <= 5; y++)
            {
                _dir = new Vector2Int(x - 3, y - 3);
                _tar = _pos + _dir;
                int _moveType = _card.getTargetInfo(x, y);
                if (CheckEffectPosition(_tar, _moveType, _isPlayer))
                    _panelList.Add(BP_List[_tar.x, _tar.y]);

            }
        }
        for (int x = 0; x <= 6; x++)
        {
            int _moveType = _card.getTargetInfo(x, 0);
            if (_moveType > 0)
            {
                for (int i = 0; i < Mathf.Max(V2_boardSize.x, V2_boardSize.y); i++)
                {
                    _dir = new Vector2Int(x - 3 + Mathf.RoundToInt(((float)(x - 3) / 3) * i), -3 - i);
                    _tar = _pos + _dir;
                    if (CheckEffectPosition(_tar, _moveType, _isPlayer))
                        _panelList.Add(BP_List[_tar.x, _tar.y]);
                }
            }
            _moveType = _card.getTargetInfo(x, 6);
            if (_moveType > 0)
            {
                for (int i = 0; i < Mathf.Max(V2_boardSize.x, V2_boardSize.y); i++)
                {
                    _dir = new Vector2Int(x - 3 + Mathf.RoundToInt(((float)(x - 3) / 3) * i), 3 + i);
                    _tar = _pos + _dir;
                    if (CheckEffectPosition(_tar, _moveType, _isPlayer))
                        _panelList.Add(BP_List[_tar.x, _tar.y]);
                }
            }
        }
        for (int y = 1; y <= 5; y++)
        {
            int _moveType = _card.getTargetInfo(0, y);
            if (_moveType > 0)
            {
                for (int i = 0; i < Mathf.Max(V2_boardSize.x, V2_boardSize.y); i++)
                {
                    _dir = new Vector2Int(-3 - i, y - 3 + Mathf.RoundToInt(((float)(y - 3) / 3) * i));
                    _tar = _pos + _dir;
                    if (CheckEffectPosition(_tar, _moveType, _isPlayer))
                        _panelList.Add(BP_List[_tar.x, _tar.y]);
                }
            }
            _moveType = _card.getTargetInfo(6, y);
            if (_moveType > 0)
            {
                for (int i = 0; i < Mathf.Max(V2_boardSize.x, V2_boardSize.y); i++)
                {
                    _dir = new Vector2Int(3 + i, y - 3 + Mathf.RoundToInt(((float)(y - 3) / 3) * i));
                    _tar = _pos + _dir;
                    if (CheckEffectPosition(_tar, _moveType, _isPlayer))
                        _panelList.Add(BP_List[_tar.x, _tar.y]);
                }
            }
        }
        return _panelList;
    }
    bool CheckMovePosition(Vector2Int _tar, int _moveType, Vector2Int _pos, VarClass.troopClass _troop, bool _needsKill)
    {
        bool _doesKill = false;
        if (!CheckOnBoard(_tar))
            return false;
        if (!BP_List[_tar.x, _tar.y].IsAvailable())
            return false;
        switch (_moveType)
        {
            case 0:
                return false;
            case 1:
                break;
            case 2:
                if (CheckObstructed(_tar, _pos))
                    return false;
                break;
            case 3:
                if (CheckAlly(_tar, !_troop.isPlayer))
                    return false;
                break;
            case 4:
                if (!CheckAlly(_tar, !_troop.isPlayer))
                    return false;
                break;
            case 5:
                if (_troop.hasMoved || CheckObstructed(_tar, _pos) || CheckAlly(_tar, !_troop.isPlayer))
                    return false;
                break;
            case 6:
                if (CheckAlly(_tar, !_troop.isPlayer))
                    return false;
                List<BoardPanel> _enemies = CheckObstructedByEnemy(_tar, _pos, _troop.isPlayer);
                if (_enemies.Contains(null))
                    return false;
                _doesKill = true;
                break;
            default:
                Debug.LogError("Movement Num \"" + _moveType + "\" is not defined.");
                return false;
        }
        if (CheckAlly(_tar, _troop.isPlayer))
            return false;
        if (CheckAlly(_tar, !_troop.isPlayer))
            _doesKill = true;
        if (_needsKill && !_doesKill)
            return false;
        return true;
    }

    public bool CheckEffectPosition(Vector2Int _tar, int _effectType, bool _isPlayer)
    {
        if (!CheckOnBoard(_tar))
            return false;
        if (!BP_List[_tar.x, _tar.y].IsAvailable())
            return false;
        switch (_effectType)
        {
            case 0:
                return false;
            case 101:
                break;
            case 102:
                if (BP_List[_tar.x, _tar.y].troop != null)
                    break;
                return false;
            case 103:
                if (BP_List[_tar.x, _tar.y].troop != null)
                    if (BP_List[_tar.x, _tar.y].troop.isPlayer == _isPlayer)
                        break;
                return false;
            case 104:
                if (BP_List[_tar.x, _tar.y].troop != null)
                    if (!BP_List[_tar.x, _tar.y].troop.isPlayer == _isPlayer)
                        break;
                return false;
            case 105:
                if (BP_List[_tar.x, _tar.y].troop == null)
                    break;
                return false;
            default:
                Debug.LogError("Effect Num \"" + _effectType + "\" is not defined.");
                return false;
        }
        return true;
    }

    public bool CheckOnBoard(Vector2Int _pos)
    {
        if (_pos.x >= 0 &&
            _pos.x < V2_boardSize.x &&
            _pos.y >= 0 &&
            _pos.y < V2_boardSize.y)
            return true;
        else
        return false;
    }
    bool CheckTroop(Vector2Int _pos)
    {
        if (BP_List[_pos.x, _pos.y].troop == null)
            return false;

        return true;
    }

    bool CheckAlly(Vector2Int _pos, bool _isPlayer)
    {
        if (BP_List[_pos.x, _pos.y].troop == null)
            return false;
        
        return BP_List[_pos.x, _pos.y].troop.isPlayer == _isPlayer;
    }

    bool CheckObstructed(Vector2Int _tar, Vector2Int _pos)
    {
        Vector2Int _dir = _tar - _pos;
        int _steps = Mathf.Max(Mathf.Abs(_dir.x), Mathf.Abs(_dir.y));
        Vector2 _stepDist = (Vector2)_dir / _steps;
        for (int i = 1; i < _steps; i++)
        {
            Vector2Int _tarPos = new Vector2Int(_pos.x + Mathf.RoundToInt(_stepDist.x * (float)i), _pos.y + Mathf.RoundToInt(_stepDist.y * (float)i));
            
            if (CheckTroop(_tarPos))
                return true;
        }
        return false;
    }
    public List<BoardPanel> CheckObstructedByEnemy(Vector2Int _tar, Vector2Int _pos, bool _isPlayer)
    {
        List<BoardPanel> _BP = new List<BoardPanel>();
        Vector2Int _dir = _tar - _pos;
        int _steps = Mathf.Max(Mathf.Abs(_dir.x), Mathf.Abs(_dir.y));
        Vector2 _stepDist = (Vector2)_dir / _steps;
        for (int i = 1; i < _steps; i++)
        {
            Vector2Int _tarPos = new Vector2Int(_pos.x + Mathf.RoundToInt(_stepDist.x * (float)i), _pos.y + Mathf.RoundToInt(_stepDist.y * (float)i));

            if (CheckAlly(_tarPos, !_isPlayer))
                _BP.Add(BP_List[_tarPos.x, _tarPos.y]);
            else
                _BP.Add(null);
        }
        return _BP;
    }

    void ClearSelPanel()
    {
        if (BP_selPanel != null)
            BP_selPanel.ResetColor();
        if (CP_selCard != null)
            CP_selCard.ResetPos();
        foreach (var item in BP_potMovement)
            item.ResetColor();

        CP_selCard = null;
        BP_selPanel = null;
        BP_potMovement.Clear();
        TIUI_troopInfo.UnloadTroop();
    }
    #endregion

    #region Board Manipulation
    void RemoveBoard()
    {
        for (int i = T_boardHolder.childCount - 1; i >= 0; i--)
            Destroy(T_boardHolder.GetChild(i).gameObject);
        BP_List = null;
    }

    void CreateBoard(int x, int y) { CreateBoard(new Vector2Int(x, y)); }

    void UpdateCameraSize(float _size) { C_mainCam.orthographicSize = _size; }

    void CreateBoard(Vector2Int boardSize)
    {
        RemoveBoard();
        //UpdateCameraSize(boardSize.x * 1.4f);
        BP_List = new BoardPanel[boardSize.x, boardSize.y];
        T_boardHolder.position = new Vector3(-(float)boardSize.x / 2 + 0.5f, 0, -(float)boardSize.y / 2 + 0.5f);
        T_boardEnv.localScale = new Vector3((float)boardSize.x + 0.5f, 1, (float)boardSize.y + 0.5f);
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                BoardPanel BP = Instantiate(PF_boardPanel, T_boardHolder);
                BP_List[x, y] = BP;

                int _matNum = (x + y) % M_boardPanelMaterials.Length;
                BP.OnCreate(new Vector2Int(x, y), M_boardPanelMaterials[_matNum]);

                BP.transform.localPosition = new Vector3(x, 0, y);
            }
        }
    }

    void ResetBoard()
    {
        foreach (var item in BP_List)
        {
            item.DestroyChild();
        }
    }
    #endregion

    #region Add Remove Voids
    public void AddRemovePoints(float _points)
    {
        D_points += _points;
        TM_points.text = D_points.AbbreviatedString();
        UpdateScore();
    }

    public void AddRemoveMultiplier(float _mult)
    {
        D_multSubTotal += _mult;
        D_multTotal = D_multSubTotal.ApplyModifier("multiplier");
        TM_multiplier.text = D_multTotal.AbbreviatedString();
        UpdateScore();
    }

    public void UpdateScore()
    {
        D_totalScore = D_points * D_multTotal;
        TM_totalScore.text = D_totalScore.AbbreviatedString();
    }

    public void AddRemoveCash(int _cash)
    {
        I_cash += _cash;
        TM_cash.text = "$"+I_cash.AbbreviatedString();
    }

    public void AddRemoveRound(int _round)
    {
        I_roundNum += _round;
        TM_round.text = I_roundNum.AbbreviatedString();
    }

    public void EndTurn(bool playerTurn)
    {
        i_extraTurnAmt = 0;
        if (playerTurn)
        {
            AddRemoveTurn(1);
            foreach (var item in GetAllTroops(true))
            {
                if (item.troop.mod.triggerType == "turnEnd")
                {
                    VarClass.cardClass _card = item.troop.mod.ConvertToCard();
                    List<BoardPanel> _targets = GetCardTargets(_card, item.pos, true);
                    if (_targets.Count > 0)
                    {
                        foreach (var item2 in _targets)
                        {
                            ApplyCardEffect(_card, item2);
                        }
                    }
                }
            }

            foreach (var item in CP_handCards)
            {
                if (item.card.cardMod.triggerType == "onHold")
                    ApplyCardModEffect(item.card);
            }

            GameModifier.OnTurnEnd();

            if (b_challengeMode)
            {
                if (GetAllTroops(false).Count == 0)
                {
                    Win();
                    return;
                }
            }
            else if (D_totalScore >= D_tarScore)
            {
                Win();
                return;
            }
            GameState = gameStateEnum.enemyTurn;
        }
        else
        {
            if (b_challengeMode)
            {
                if (GetAllTroops(false).Count == 0)
                {
                    Win();
                    return;
                }
            }
            else if (D_totalScore >= D_tarScore)
            {
                Win();
                return;
            }
            if (I_turnNum >= I_maxTurns)
            {
                Lose();
                return;
            }
            DrawCards(1);
            GameState = gameStateEnum.playerTurn;
        }
        List<BoardPanel> _list = GetAllTroops(!playerTurn);
        if (!playerTurn && _list.Count == 0)
        {
            Lose();
            return;
        }
        foreach (var item in _list)
        {
            item.StartCoroutine(item.Bounce());
        }
    }

    public void Win()
    {
        GameState = gameStateEnum.win;

        if (b_challengeMode)
        {
            TM_WP_totalScore.transform.parent.gameObject.SetActive(false);

            TM_WP_targetScore.transform.parent.gameObject.SetActive(false);
            TM_WP_remainingTurns.transform.parent.gameObject.SetActive(false);

            TM_WP_totalReward.text = "Return";
        }
        else
        {
            TM_WP_totalScore.text = D_totalScore.AbbreviatedString();

            TM_WP_targetScore.text = D_tarScore.AbbreviatedString();
            TM_WP_scoreReward.text = I_rewardCash.ToCashAmount(5);

            int _turnsLeft = I_maxTurns - I_turnNum;
            TM_WP_remainingTurns.text = _turnsLeft.AbbreviatedString();
            TM_WP_remainingTurnsReward.text = _turnsLeft.ToCashAmount(5);

            TM_WP_totalReward.text = "Claim: " + (I_rewardCash + _turnsLeft).ToCashAmount(0);
        }        

        G_winPanel.SetActive(true);
        StartCoroutine(WinPanel_Animation());
    }

    public void Lose()
    {
        GameState = gameStateEnum.loss;

        if (b_challengeMode)
        {
            TM_LP_totalScore.transform.parent.gameObject.SetActive(false);
            TM_LP_targetScore.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            TM_LP_totalScore.text = D_totalScore.AbbreviatedString();
            TM_LP_targetScore.text = D_tarScore.AbbreviatedString();
        }

        G_losePanel.SetActive(true);
        StartCoroutine(LosePanel_Animation());
    }

    public void LoadStore()
    {
        if (b_challengeMode)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            G_winPanel.SetActive(false);
            G_losePanel.SetActive(false);
            G_storePanel.SetActive(true);
            CreateStore(I_roundNum > DC_curDifficulty.roundsPerStage);
            AddRemoveCash(
                I_rewardCash +
                I_maxTurns - I_turnNum);

            I_roundNum++;
            if (I_roundNum > DC_curDifficulty.roundsPerStage)
            {
                I_roundNum = 1;
                I_stageNum++;
                RoundSkip_RollRewards();
            }
        }
    }
    public void LoadRoundSelect()
    {
        G_storePanel.SetActive(false);
        G_roundSelectPanel.SetActive(true);

        for (int i = RSO_roundSelects.Count - 1; i >= 0; i--)
            Destroy(RSO_roundSelects[i]);
        RSO_roundSelects.Clear();

        int _buttonAmt = DC_curDifficulty.roundsPerStage;
        for (int i = 0; i < _buttonAmt; i++)
        {
            RoundSelectObject _RSO = Instantiate(PF_RoundSelectObject, T_roundSelectHolder);
            bool _active = i+1 == I_roundNum;
            _RSO.RT_transform.anchoredPosition = new Vector2(0, F_roundSelectGap * i - (F_roundSelectGap * (_buttonAmt - 1) / 2));
            double tarScore = DC_curDifficulty.GetTarScore(i+1, I_stageNum);

            if (i < RSR_skipRewards.Count)
                _RSO.OnCreate(i, _active, i + 1 == _buttonAmt, tarScore, 5, RSR_skipRewards[i]);
            else
                _RSO.OnCreate(i, _active, i + 1 == _buttonAmt, tarScore, 5, RoundSkipList.Instance.list[0]);
            RSO_roundSelects.Add(_RSO);
        }
    }
    public void SkipRound(VarClass.roundSkipReward _reward)
    {
        I_roundNum = Mathf.Clamp(I_roundNum+1,0, DC_curDifficulty.roundsPerStage);
        for (int i = 0; i < RSO_roundSelects.Count; i++)
        {
            RSO_roundSelects[i].UpdateAvailability(i+1 == I_roundNum, i + 1 == RSO_roundSelects.Count);
        }

        switch (_reward.effectID)
        {
            case "cashMultiply":
                int _amt = Mathf.RoundToInt(I_cash * float.Parse(_reward.effectVar));
                _amt -= I_cash;
                AddRemoveCash(_amt);
                break;
            case "cardGet":
                VarClass.cardClass _card = CardList.Instance.FindCardByID(_reward.effectVar);
                if (_card != null)
                    PlayerData.C_EquippedCards.Add(_card);
                break;
            default:
                break;
        }
    }

    public void LoadNextRound()
    {
        G_roundSelectPanel.SetActive(false);
        

        LoadGame();
    }

    public void RestartGame()
    {
        if (b_challengeMode)
        {
            CHALLENGE_LoadCards();
            CHALLENGE_LoadModifiers();
        }
        else
        {
            DEBUG_CreateRandomTroops(new Vector2Int(8, 1));
            DEBUG_CreateRandomDeck(3);
        }
        I_cash = 0;

        RoundSkip_RollRewards();
        LoadGame();
    }

    void CHALLENGE_LoadCards()
    {
        PlayerData.C_EquippedCards.Clear();
        for (int i = 0; i < CC_curChallenge.cards.Length; i++)
        {
            if (CC_curChallenge.cards[i] != "N/A")
                PlayerData.C_EquippedCards.Add(CardList.Instance.FindCardByID(CC_curChallenge.cards[i]).Clone());
        }
        PlayerData.C_DeckCards = PlayerData.CopyEquippedCards();
        ClearCards();
        DrawCards(PlayerData.C_EquippedCards.Count);
    }    
    void CHALLENGE_LoadModifiers()
    {
        PlayerData.M_EquippedMods.Clear();
        for (int i = 0; i < CC_curChallenge.gameModifiers.Length; i++)
        {
            if (CC_curChallenge.gameModifiers[i] != "N/A")
                PlayerData.M_EquippedMods.Add(GameModList.Instance.FindModifierByID(CC_curChallenge.gameModifiers[i]));
        }
        PAUSE_UpdateGameModifiers();
    }


    public void LoadGame()
    {
        G_losePanel.SetActive(false);
        RemoveBoard();

        I_turnNum = 0;

        D_points = 0;
        D_multSubTotal = 0;
        D_multTotal = 0;
        D_totalScore = 0;

        if (b_challengeMode)
        {
            I_maxTurns = CC_curChallenge.turnLimit;
            V2_boardSize = new Vector2Int(CC_curChallenge.sizeX, CC_curChallenge.troopIDs.Length);
            CreateBoard(V2_boardSize);
            CHALLENGE_CreateStartTroops();
        }
        else
        {

            I_maxTurns = 8;

            V2_boardSize = v2_boardSizeByStage[Mathf.Min(I_stageNum - 1, v2_boardSizeByStage.Length - 1)];
            CreateBoard(V2_boardSize);
            CreateStartTroops();

            PlayerData.C_DeckCards = PlayerData.CopyEquippedCards();
            ClearCards();
            DrawCards(3);
        }

        AddRemovePoints(0);
        AddRemoveMultiplier(0);
        AddRemoveRound(0);
        AddRemoveTurn(0);
        AddRemoveCash(0);
        SetRewardAmt(5);
        SetTargetScore(DC_curDifficulty.GetTarScore(I_roundNum, I_stageNum));
        GameState = gameStateEnum.playerTurn;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    void RoundSkip_RollRewards()
    {
        RSR_skipRewards.Clear();
        for (int i = 0; i < DC_curDifficulty.roundsPerStage-1; i++)
            RSR_skipRewards.Add(RoundSkipList.Instance.GetRandom());

        RSR_skipRewards.Add(RoundSkipList.Instance.list[0]);
    }

    public void AddRemoveTurn(int _turn)
    {
        I_turnNum += _turn;
        TM_turns.text = I_turnNum + "/" + I_maxTurns;
    }

    public void SetTargetScore(double _tarScore)
    {
        D_tarScore = _tarScore;
        TM_targetScore.text = D_tarScore.AbbreviatedString();
    }

    public void SetRewardAmt(int _rewardCash)
    {
        I_rewardCash = _rewardCash;
        TM_reward.text = "Reward: " + I_rewardCash.ToCashAmount(8);
    }
    #endregion

    #region Store Management
    public void CreateStore(bool _newStage)
    {
        ClearStore(_newStage);

        Store_UpdateRerollCost();

        AddStoreItems();
        AddPackItems();
        //if (_newStage)
            AddStageItems();
    }

    void Store_UpdateRerollCost()
    {
        TM_rerollCost.text = "$" + I_rerollCost.ApplyModifier("rerollCost").ToString();
    }

    void AddStoreItems()
    {
        int _storeItems = Random.Range(1, 4);
        _storeItems = _storeItems.ApplyModifier("storeSize");
        for (int i = 0; i < _storeItems; i++)
        {
            VarClass.cardClass _card = CardList.Instance.GetRandom();
            if (_card != null)
            {
                StoreItem SI = Instantiate(PF_storeItem, T_storeFront);
                SI.RT_transform.anchoredPosition = new Vector2(i * F_storeItemGap - ((_storeItems - 1) * F_storeItemGap / 2), 0);
                SI.OnCreate(_card, false);
                si_storeFront.Add(SI);
            }

        }
    }
    void AddPackItems()
    {
        int _packItems = 2;

        for (int i = 0; i < _packItems; i++)
        {
            VarClass.storePackClass _pack = StorePackList.Instance.GetRandom();
            if (_pack != null)
            {
                StoreItem SI = Instantiate(PF_storeItem, T_packs);
                SI.RT_transform.anchoredPosition = new Vector2(i * F_storeItemGap - ((_packItems - 1) * F_storeItemGap / 2), 0);
                SI.OnCreate(_pack);
                si_packs.Add(SI);
            }
        }
    }
    void AddStageItems()
    {
        int _stageItems = 1;

        for (int i = 0; i < _stageItems; i++)
        {
            VarClass.gameModifierClass _pack = GameModList.Instance.GetRandomUnowned();
            if (_pack != null)
            {
                StoreItem SI = Instantiate(PF_storeItem, T_stageMods);
                SI.RT_transform.anchoredPosition = new Vector2(i * F_storeItemGap - ((_stageItems - 1) * F_storeItemGap / 2), 0);
                SI.OnCreate(_pack);
                si_stageMods.Add(SI);
            }
        }
    }

    public void SortStore()
    {
        int _size = si_storeFront.Count;
        for (int i = 0; i < _size; i++)
            si_storeFront[i].RT_transform.anchoredPosition = new Vector2(i * F_storeItemGap - ((_size - 1) * F_storeItemGap / 2), 0);

        _size = si_packs.Count;
        for (int i = 0; i < _size; i++)
            si_packs[i].RT_transform.anchoredPosition = new Vector2(i * F_storeItemGap - ((_size - 1) * F_storeItemGap / 2), 0);

        _size = si_stageMods.Count;
        for (int i = 0; i < _size; i++)
            si_stageMods[i].RT_transform.anchoredPosition = new Vector2(i * F_storeItemGap - ((_size - 1) * F_storeItemGap / 2), 0);
    }

    public void RerollStore()
    {
        int _cost = I_rerollCost.ApplyModifier("rerollCost");
        if (I_cash >= _cost)
        {
            AddRemoveCash(-_cost);
            for (int i = si_storeFront.Count - 1; i >= 0; i--)
                Destroy(si_storeFront[i].gameObject);
            si_storeFront.Clear();
            AddStoreItems();
        }
    }

    public void ClearStore(bool _clearStageMod)
    {
        for (int i = si_packs.Count - 1; i >= 0; i--)
            Destroy(si_packs[i].gameObject);
        for (int i = si_storeFront.Count - 1; i >= 0; i--)
            Destroy(si_storeFront[i].gameObject);
        si_packs.Clear();
        si_storeFront.Clear();
        //if (_clearStageMod)
        //{
            for (int i = si_stageMods.Count - 1; i >= 0; i--)
                Destroy(si_stageMods[i].gameObject);
            si_stageMods.Clear();
        //}
    }

    public void GameMod_Purchase(VarClass.gameModifierClass _mod, StoreItem _item)
    {
        if (_mod.cost <= I_cash)
        {
            PlayerData.M_EquippedMods.Add(_mod);
            GameModList.Instance.list_unowned.Remove(_mod);

            AddRemoveCash(-_mod.cost);

            CardPack_RemovePack(_item);
            Store_UpdateRerollCost();
            PAUSE_UpdateGameModifiers();
        }
    }

    public void Card_Purchase(VarClass.cardClass _card, StoreItem _item)
    {
        if (_card.cost + _card.cardMod.cost <= I_cash)
        {
            PlayerData.C_EquippedCards.Add(_card);

            AddRemoveCash(-(_card.cost + _card.cardMod.cost));

            CardPack_RemovePack(_item);
        }
    }

    #endregion

    #region Card Pack Management

    public void CardPack_Open(VarClass.storePackClass _pack, StoreItem _item)
    {
        if (_pack.cost <= I_cash)
        {
            TM_cardSelectAmt.text = "SELECT UP TO " + _pack.amt + " CARDS";
            CardPack_RemovePack(_item);
            AddRemoveCash(-_pack.cost);
            CardPack_Clear();
            sp_curPack = _pack;
            for (int i = 0; i < _pack.pool; i++)
            {
                StoreItem _si = Instantiate(PF_storeItem, T_cardSelectHolder);
                _si.RT_transform.anchoredPosition = new Vector2(i * F_cardItemGap - ((_pack.pool - 1) * F_cardItemGap / 2), 0);
                _si.OnCreate(CardList.Instance.GetRandom(), true);
                si_cardSelectItems.Add(_si);
            }

            G_cardSelect.SetActive(true);
        }
    }

    void CardPack_RemovePack(StoreItem _item)
    {
        if (si_packs.Contains(_item))
            si_packs.Remove(_item);
        if (si_stageMods.Contains(_item))
            si_stageMods.Remove(_item);
        if (si_storeFront.Contains(_item))
            si_storeFront.Remove(_item);

        Destroy(_item.gameObject);

        SortStore();
    }

    public void CardPack_SelectItem(StoreItem _item)
    {
        if (si_cardSelectedItems.Contains(_item))
        {
            si_cardSelectedItems.Remove(_item);
            _item.OnSelected(false);
        }
        else
        {
            if (si_cardSelectedItems.Count < sp_curPack.amt)
            {
                si_cardSelectedItems.Add(_item);
                _item.OnSelected(true);
            }
        }
    }

    public void CardPack_Confirm()
    {
        foreach (var item in si_cardSelectedItems)
            PlayerData.C_EquippedCards.Add(item.card);
        
        G_cardSelect.SetActive(false);
        CardPack_Clear();
    }

    void CardPack_Clear()
    {
        for (int i = si_cardSelectItems.Count - 1; i >= 0; i--)
            Destroy(si_cardSelectItems[i].gameObject);
        si_cardSelectItems.Clear();
        si_cardSelectedItems.Clear();
        sp_curPack = null;
    }
    #endregion

    #region DEBUG
    void DEBUG_CreateRandomDeck(int deckSize)
    {
        PlayerData.C_EquippedCards.Clear();
        for (int i = 0; i < deckSize; i++)
        {
            int RandomNum = Random.Range(0, CardList.Instance.list.Count);
            PlayerData.C_EquippedCards.Add(CardList.Instance.list[RandomNum].Clone());
        }
        PlayerData.C_DeckCards = PlayerData.CopyEquippedCards();
    }

    void DEBUG_CreateRandomTroops(Vector2Int _size)
    {
        List<List<VarClass.troopClass>> _list = new List<List<VarClass.troopClass>>();
        for (int x = 0; x < _size.x; x++)
        {
            _list.Add(new List<VarClass.troopClass>());
            for (int y = 0; y < _size.y; y++)
            {
                _list[x].Add(TroopList.Instance.GetRandom().Clone());
            }
        }
        PlayerData.T_EquippedTroops = _list;
    }
    #endregion

    #region FTUE
    public void FTUE_ShowPanel(int num)
    {
        G_FTUEPanels[num].SetActive(true);
    }
    public void FTUE_HidePanel(int num)
    {
        G_FTUEPanels[num].SetActive(false);
    }
    public void FTUE_HideAll()
    {
        foreach (var item in G_FTUEPanels)
            item.SetActive(false);
    }
    #endregion

    #region PAUSE
    void PAUSE_UpdateGameModifiers()
    {
        for (int i = GMD_gameModDetails.Count-1; i >= 0; i--)
            Destroy(GMD_gameModDetails[i].gameObject);
        GMD_gameModDetails.Clear();
        float _yPos = -200;
        for (int i = 0; i < PlayerData.M_EquippedMods.Count; i++)
        {
            GameModifierDetailUI _temp = Instantiate(PF_gameModDetail, RT_gameModHolder);
            _temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, _yPos);
            _yPos -= _temp.F_height;
            _temp.OnCreate(PlayerData.M_EquippedMods[i]);
            GMD_gameModDetails.Add(_temp);
        }
        RT_gameModHolder.sizeDelta = new Vector2(RT_gameModHolder.sizeDelta.x, -_yPos);
    }
    #endregion

    #region Animation
    IEnumerator WinPanel_Animation()
    {
        RT_WP_Header.anchoredPosition = new Vector2(0, 2000);
        RT_WP_Content.anchoredPosition = new Vector2(0, -2000);
        TM_WP_totalScore.rectTransform.localScale = new Vector2(0, 0);
        RT_WP_Claim.localScale = new Vector2(0, 0);

        StartCoroutine(Panel_Darkenator(I_WP_Darkenator, 0.4f, A_overShoot));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Panel_Move(RT_WP_Header, 0.4f, new Vector2(0,2000), new Vector2(0, 350), A_overShoot));
        StartCoroutine(Panel_Move(RT_WP_Content, 0.4f, new Vector2(0, -2000), new Vector2(0, -250), A_overShoot));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(Panel_Scale(TM_WP_totalScore.rectTransform, 0.4f, Vector2.zero, Vector2.one, A_overShoot));
        StartCoroutine(Panel_Scale(RT_WP_Claim, 0.4f, Vector2.zero, Vector2.one, A_overShoot));
    }
    IEnumerator LosePanel_Animation()
    {
        RT_LP_Header.anchoredPosition = new Vector2(0, 2000);
        RT_LP_Content.anchoredPosition = new Vector2(0, -2000);
        TM_LP_totalScore.rectTransform.localScale = new Vector2(0, 0);

        StartCoroutine(Panel_Darkenator(I_LP_Darkenator, 0.4f, A_overShoot));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Panel_Move(RT_LP_Header, 0.4f, new Vector2(0, 2000), new Vector2(0, 350), A_overShoot));
        StartCoroutine(Panel_Move(RT_LP_Content, 0.4f, new Vector2(0, -2000), new Vector2(0, -400), A_overShoot));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(Panel_Scale(TM_LP_totalScore.rectTransform, 0.4f, Vector2.zero, Vector2.one, A_overShoot));
    }

    IEnumerator Panel_Darkenator(Image _darkenator, float _duration, bool on)
    {
        _darkenator.gameObject.SetActive(true);
        float start = _darkenator.color.a;
        float end = 0;
        Color _color = _darkenator.color;
        if (on)
            end = 0.75f;
        float _progress = 0;
        while (_progress < 1)
        {
            _color.a = Mathf.Lerp(start, end, A_overShoot.Evaluate(_progress));
            _darkenator.color = _color;
            _progress += Time.deltaTime / _duration;
            yield return new WaitForEndOfFrame();
        }
        _color.a = end;
        _darkenator.color = _color;
        if (!on)
            _darkenator.gameObject.SetActive(false);
    }
    void Panel_Move(RectTransform _transform, float _duration, Vector2 _tar, AnimCurve_Scriptable _anim)
    {
        StartCoroutine(Panel_Move(_transform, _duration, _transform.anchoredPosition, _tar, _anim));
    }

    IEnumerator Panel_Move(RectTransform _transform, float _duration, Vector2 _start, Vector2 _tar, AnimCurve_Scriptable _anim)
    {
        float _progress = 0;
        while (_progress < 1)
        {
            _transform.anchoredPosition = Vector2.Lerp(_start, _tar, _anim.Evaluate(_progress));
            _progress += Time.deltaTime / _duration;
            yield return new WaitForEndOfFrame();
        }
        _transform.anchoredPosition = Vector2.Lerp(_start, _tar, _anim.Evaluate(1));
    }
    void Panel_Scale(RectTransform _transform, float _duration, Vector2 _tar, AnimCurve_Scriptable _anim)
    {
        StartCoroutine(Panel_Scale(_transform, _duration, _transform.localScale, _tar, _anim));
    }
    IEnumerator Panel_Scale(RectTransform _transform, float _duration, Vector2 _start, Vector2 _tar, AnimCurve_Scriptable _anim)
    {
        float _progress = 0;
        while (_progress < 1)
        {
            _transform.localScale = Vector2.Lerp(_start, _tar, _anim.Evaluate(_progress));
            _progress += Time.deltaTime / _duration;
            yield return new WaitForEndOfFrame();
        }
        _transform.localScale = Vector2.Lerp(_start, _tar, _anim.Evaluate(1));
    }
    #endregion
}

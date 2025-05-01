using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPanel : MonoBehaviour
{
    public Renderer R_Renderer;
    private float F_moveDuration = 0.25f;
    [HideInInspector] public Color col = Color.white;
    [HideInInspector] public Vector2Int pos;
    [HideInInspector] public VarClass.troopClass troop = null;

    [HideInInspector] public TroopPrefab TP_troop;

    public ParticleHandler PF_selParticles;
    private ParticleHandler PH_activeParticles;

    private List<VarClass.TileEffect> TE_Effects = new List<VarClass.TileEffect>();

    private bool b_killedTroopOnMove = false;

    public AnimCurve_Scriptable A_Bounce;
    public AnimCurve_Scriptable A_Move;
    public AnimCurve_Scriptable A_Lerp;

    private int i_points = 0;

    private bool b_unavailable = false;

    public void OnCreate(Vector2Int _pos, Material _mat)
    {
        gameObject.SetActive(true);
        b_unavailable = false;

        col = _mat.color;
        pos = _pos;
        troop = null;

        R_Renderer.material = _mat;
    }

    public void Remove()
    {
        gameObject.SetActive(false);
        b_unavailable = true;
    }

    public void CreateTroop(VarClass.troopClass _troop, bool _isPlayer, GameManager _GM)
    {
        troop = _troop;
        troop.isPlayer = _isPlayer;
        troop.hasMoved = false;
        TP_troop = Instantiate(troop.objAddressable.obj, transform).GetComponent<TroopPrefab>();
        TP_troop.transform.localPosition = Vector3.zero;
        if (troop.isPlayer)
        {
            SetPoints();
            TP_troop.SetBaseColor(_GM.C_playerTroop);
        }
        else
        {
            TP_troop.SetBaseColor(_GM.C_enemyTroop);
        }
        if (_troop.mod.id != "")
        {
            TP_troop.SetBandColor(_troop.mod.color());
            ApplyPassiveTileEffects();
        }
    }

    public void SwitchTeam(bool _isPlayer, GameManager _GM)
    {
        troop.isPlayer = _isPlayer;
        if (troop.isPlayer)
        {
            SetPoints();
            TP_troop.SetBaseColor(_GM.C_playerTroop);
        }
        else
        {
            TP_troop.SetBaseColor(_GM.C_enemyTroop);
        }
        RemovePassiveTileEffects();
        ApplyPassiveTileEffects();
    }

    public void SwitchTroop(VarClass.troopClass _troop, bool _isPlayer, GameManager _GM)
    {
        DestroyChild();
        troop = _troop.Clone();
        troop.isPlayer = _isPlayer;
        TP_troop = Instantiate(troop.objAddressable.obj, transform).GetComponent<TroopPrefab>();
        TP_troop.transform.localPosition = Vector3.zero;
        if (troop.isPlayer)
        {
            SetPoints();
            TP_troop.SetBaseColor(_GM.C_playerTroop);
        }
        else
        {
            TP_troop.SetBaseColor(_GM.C_enemyTroop);
        }
        if (_troop.mod.id != "")
            TP_troop.SetBandColor(_troop.mod.color());
    }

    public void ApplyTroopModifier(VarClass.troopModifierClass _troopMod)
    {
        if (troop != null)
        {
            troop.mod = _troopMod;
            TP_troop.SetBandColor(troop.mod.color());
            ApplyPassiveTileEffects();
        }
    }

    public void DestroyChild()
    {
        if (TP_troop != null)
        {
            RemovePassiveTileEffects();
            if (troop.mod.triggerType == "onDeath")
                StartCoroutine(OnDeathEffect(troop));
            b_killedTroopOnMove = true;
            TP_troop.Destroy(A_Lerp);
            if (troop.isPlayer)
            {
                GameModifier.OnAllyDeath();
                SetPoints();
            }
            else
            {
                GameModifier.OnEnemyDeath();
                GameManager.Instance.AddRemoveMultiplier(troop.getMultiplier());
            }
        }
        TP_troop = null;
        troop = null;
    }
    void RemovePassiveTileEffects()
    {
        if (troop != null)
        {
            foreach (var item in troop.modEffects)
            {
                GameManager.Instance.GetBoardPanel(item.pos).RemoveSpecificPassiveTileEffect(item);
            }
            troop.modEffects.Clear();
        }
    }

    void ApplyPassiveTileEffects()
    {
        if (troop != null)
        {
            if (troop.mod.triggerType == "passive")
            {
                troop.modEffects = troop.mod.ApplyEffectList(pos, troop.isPlayer);
            }
        }
    }
    public void RemoveSpecificPassiveTileEffect(VarClass.TileEffect _TE)
    {
        TE_Effects.Remove(_TE);
        SetPoints();
    }
    public void AddSpecificPassiveTileEffect(VarClass.TileEffect _TE)
    {
        TE_Effects.Add(_TE);
        SetPoints();
    }
    IEnumerator OnDeathEffect(VarClass.troopClass _troop)
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.ActivateTroopModEffect(_troop, pos);
    }
    public int RemoveChild()
    {
        int _points = i_points;
        i_points = 0;
        RemovePassiveTileEffects();
        TP_troop = null;
        troop = null;
        return _points;
    }

    public void SetColor(Color _col)
    {
        R_Renderer.material.color = _col;

        if (PH_activeParticles != null)
            Destroy(PH_activeParticles.gameObject);
        PH_activeParticles = Instantiate(PF_selParticles, transform.position, PF_selParticles.transform.rotation);
        PH_activeParticles.SetColor(_col);
    }
    public void ResetColor()
    {
        R_Renderer.material.color = col;

        if (PH_activeParticles != null)
            Destroy(PH_activeParticles.gameObject);
    }
    public void MoveFrom(BoardPanel _from)
    {
        b_killedTroopOnMove = false;
        DestroyChild();
        TP_troop = _from.TP_troop;
        TP_troop.transform.parent = transform;
        troop = _from.troop;
        troop.hasMoved = true;
        i_points = _from.RemoveChild();
        SetPoints();
        StartCoroutine(Move());
        OnReachEnd();
        ApplyPassiveTileEffects();
        MoveFrom_MoveTypeMechanic(_from);
    }

    void MoveFrom_MoveTypeMechanic(BoardPanel _from)
    {
        Vector2Int _arrayPos = pos - _from.pos;
        _arrayPos.x = Mathf.Clamp(_arrayPos.x + 3, 0, 6);
        _arrayPos.y = Mathf.Clamp(_arrayPos.y + 3, 0, 6);
        int _moveType = troop.getMoveArray()[_arrayPos.y].array[_arrayPos.x];
        switch (_moveType)
        {
            case 6:
                foreach (var item in GameManager.Instance.CheckObstructedByEnemy(pos, _from.pos, troop.isPlayer))
                    item.DestroyChild();
                b_killedTroopOnMove = true;
                break;
            default:
                break;
        }
    }

    void SetPoints()
    {
        int _points = 0;
        if (troop != null)
        {
            if (troop.isPlayer)
            {
                _points = troop.getPoints();
                foreach (var item in TE_Effects)
                {
                    if (GameManager.Instance.CheckEffectPosition(pos, item.targetTile, item.isPlayer))
                    {
                        if (item.effectID == "troopPointMultiply")
                        {
                            _points = Mathf.RoundToInt(_points * float.Parse(item.effectVar));
                        }
                    }
                }
            }
        }
        if (_points != i_points)
            GameManager.Instance.AddRemovePoints(_points - i_points);
        i_points = _points;
    }
    public void OnReachEnd()
    {
        int end = 0;
        if (troop.isPlayer)
            end = GameManager.Instance.V2_boardSize.y - 1;

        if (pos.y == end)
        {
            if (troop.switchOnReachEnd != "N/A")
                SwitchTroop(TroopList.Instance.FindTroopByID(troop.switchOnReachEnd), troop.isPlayer, GameManager.Instance);
        }
    }

    IEnumerator Move()
    {
        float _progress = 0;
        Vector3 _startPos = TP_troop.transform.localPosition;
        Vector3 _pos;
        while (_progress < 1)
        {
            _pos = Vector3.Lerp(_startPos, Vector3.zero, _progress);
            _pos.y = Mathf.Lerp(0, 1f, A_Move.Evaluate(_progress));
            TP_troop.transform.localPosition = _pos;
            _progress += Time.deltaTime/F_moveDuration;
            yield return new WaitForEndOfFrame();
        }
        TP_troop.transform.localPosition = Vector3.zero;

        if (b_killedTroopOnMove)
            GameManager.Instance.MultiTurn(this);
        else
            GameManager.Instance.EndTurn(troop.isPlayer);
    }

    public IEnumerator Bounce()
    {
        float _progress = 0;
        Vector3 _startPos = TP_troop.transform.localPosition;
        Vector3 _endPos = _startPos + new Vector3(0,0.5f,0);
        while (_progress < 1)
        {
            TP_troop.transform.localPosition = Vector3.Lerp(_startPos, _endPos, A_Bounce.Evaluate(_progress));
            _progress += Time.deltaTime / 0.25f;
            yield return new WaitForEndOfFrame();
        }
        TP_troop.transform.localPosition = Vector3.zero;
    }

    public bool IsPlayerEnemy(bool _isPlayer, bool _includeEmpty)
    {
        if (troop != null)
            return troop.isPlayer == _isPlayer;
        return _includeEmpty;
    }

    public bool IsAvailable()
    {
        return !b_unavailable;
    }
}

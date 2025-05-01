using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Animations")]
    public AnimCurve_Scriptable A_smooth;
    public AnimCurve_Scriptable A_accelerate;
    public AnimCurve_Scriptable A_overshoot;
    public RectTransform RT_handObject;
    public Image I_Darkenator;

    [Header("Challenges")]
    public ChallengeButton PF_challengeButton;
    public RectTransform RT_challengeHolder;
    private List<ChallengeButton> CB_challenges = new List<ChallengeButton>();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Panel_Darkenator(I_Darkenator, 1, 1, 0, false));
        CHALLENGE_CreateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CHALLENGE_CreateButtons()
    {
        for (int i = CB_challenges.Count - 1; i >= 0; i--)
            Destroy(CB_challenges[i].gameObject);
        CB_challenges.Clear();
        float _yPos = 200;
        for (int i = 0; i < ChallengeList.Instance.list.Count; i++)
        {
            ChallengeButton CB = Instantiate(PF_challengeButton, RT_challengeHolder);
            CB.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -_yPos);
            _yPos += CB.F_height;

            CB.OnCreate(ChallengeList.Instance.list[i]);
        }
        RT_challengeHolder.sizeDelta = new Vector2(RT_challengeHolder.sizeDelta.x, _yPos);
    }

    public void SCENE_LoadGame()
    {
        PlayerData.C_CurrentChallenge = null;
        StartCoroutine(LoadScene(1));
    }

    public void SCENE_LoadChallenge(VarClass.challengeClass _challenge)
    {
        PlayerData.C_CurrentChallenge = _challenge;
        StartCoroutine(LoadScene(1));
    }

    public IEnumerator LoadScene(int _sceneNum)
    {
        RT_handObject.gameObject.SetActive(true);
        Panel_Move(RT_handObject, 0.7f, Vector2.zero, A_accelerate);
        StartCoroutine(Panel_Rot(RT_handObject, 1.3f, -15f, 0, A_overshoot));
        yield return new WaitForSeconds(1f);
        Panel_Scale(RT_handObject, 1f, Vector3.one*25, A_smooth);
        StartCoroutine(Panel_Darkenator(I_Darkenator, 1f, 0, 1, false));
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_sceneNum);
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
    void Panel_Rot(RectTransform _transform, float _duration, float _tar, AnimCurve_Scriptable _anim)
    {
        StartCoroutine(Panel_Rot(_transform, _duration, _transform.localEulerAngles.z, _tar, _anim));
    }
    IEnumerator Panel_Rot(RectTransform _transform, float _duration, float _start, float _tar, AnimCurve_Scriptable _anim)
    {
        float _progress = 0;
        float _curRot = _start;
        while (_progress < 1)
        {
            _curRot = Mathf.Lerp(_start, _tar, _anim.Evaluate(_progress));
            _transform.localEulerAngles = new Vector3(_transform.localRotation.x, _transform.localRotation.y, _curRot);
            _progress += Time.deltaTime / _duration;
            yield return new WaitForEndOfFrame();
        }
        _curRot = Mathf.Lerp(_start, _tar, _anim.Evaluate(1));
        _transform.localEulerAngles = new Vector3(_transform.localRotation.x, _transform.localRotation.y, _curRot);
    }
    IEnumerator Panel_Darkenator(Image _darkenator, float _duration, float _start, float _end, bool on)
    {
        _darkenator.gameObject.SetActive(true);
        Color _color = _darkenator.color;
        float _progress = 0;
        while (_progress < 1)
        {
            _color.a = Mathf.Lerp(_start, _end, A_smooth.Evaluate(_progress));
            _darkenator.color = _color;
            _progress += Time.deltaTime / _duration;
            yield return new WaitForEndOfFrame();
        }
        _color.a = _end;
        _darkenator.color = _color;
        if (!on)
            _darkenator.gameObject.SetActive(false);
    }
}

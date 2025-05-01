using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPrefab : MonoBehaviour
{
    public Renderer R_Renderer;

    public void SetBaseColor(Color _color)
    {
        R_Renderer.materials[0].color = _color;
    }
    public void SetBandColor(Color _color)
    {
        R_Renderer.materials[1].color = _color;
    }

    public void Destroy(AnimCurve_Scriptable _deathAnim)
    {
        transform.parent = null;
        StartCoroutine(DestroyCoroutine(_deathAnim));
    }

    private IEnumerator DestroyCoroutine(AnimCurve_Scriptable _deathAnim)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime / 0.2f;
            transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1, 0.05f, 1), _deathAnim.Evaluate(progress));
            yield return new WaitForEndOfFrame();
        }
        GameObject.Destroy(gameObject);
    }
}

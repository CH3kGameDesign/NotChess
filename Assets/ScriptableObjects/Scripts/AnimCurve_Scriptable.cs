using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animal Warriors/Visual/Anim Curve Object", fileName = "New Anim Curve")]
public class AnimCurve_Scriptable : ScriptableObject
{
    public AnimationCurve curve = new AnimationCurve();

    public float Evaluate(float point)
    {
        return curve.Evaluate(point);
    }
}

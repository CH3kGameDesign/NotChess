using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    public ParticleSystem[] P_Particles = new ParticleSystem[0];
    
    public void SetColor(Color _color)
    {
        foreach (var item in P_Particles)
        {
            ParticleSystem.MainModule _main = item.main;
            _main.startColor = _color;
        }
    }
}

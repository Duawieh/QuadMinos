using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoColors
{
    public readonly static Color[] minoColor = new Color[8] {
        new Color(0.2901961f, 1.0000000f, 0.6784314f),  // mino_I
        new Color(0.3960785f, 0.2196079f, 0.9686275f),  // mino_J
        new Color(1.0000000f, 0.6352941f, 0.0000000f),  // mino_L
        new Color(1.0000000f, 0.9215687f, 0.0000000f),  // mino_O
        new Color(0.5372549f, 1.0000000f, 0.2588235f),  // mino_S
        new Color(0.8274511f, 0.2196079f, 1.0000000f),  // mino_T
        new Color(1.0000000f, 0.2941177f, 0.5176471f),  // mino_Z
        new Color(0.6901961f, 0.6901961f, 0.6901961f)   // mino_Garbage
    };
}

public class S_ParticleStar : MonoBehaviour
{
    public void Init(int _type)
    {
        ParticleSystem.MainModule m = GetComponent<ParticleSystem>().main;
        m.startColor = MinoColors.minoColor[_type];
        return;
    }
}

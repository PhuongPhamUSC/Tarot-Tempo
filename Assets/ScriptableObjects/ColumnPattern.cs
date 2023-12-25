using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ColumnPattern : ScriptableObject
{
    public NotePattern[] m_patterns;
    public float[] m_xPlacement;
    public float[] m_distanceBetween;
    public float m_xPos;
    public bool m_isRandom = false;
}

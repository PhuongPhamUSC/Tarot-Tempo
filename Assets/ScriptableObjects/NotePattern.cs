using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NotePattern : ScriptableObject
{
    public int m_noteQuantity = 1;
    public string m_axis = "x";
    public float m_distance = 0.25f;
    public GameObject m_notePrefab;
}

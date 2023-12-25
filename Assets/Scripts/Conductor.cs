using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    // user provided variable
    public float m_bpm;
    public float m_offset;
    public int m_beatInterval = 1;
    // calculated variables
    public float m_crochet;
    public float m_songPosition;
    AudioSource m_audioSource;
    double m_startTime;
    // column pattern
    int m_currentColumnPatternIdx = 0;
    public ColumnPattern m_currentColumnPattern;
    public ColumnPattern[] m_columnPatternList;


    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_crochet = 60 / m_bpm;
        m_currentColumnPattern = m_columnPatternList[m_currentColumnPatternIdx];
    }

    private void Update()
    {
        if (m_audioSource.isPlaying)
        {
            m_songPosition = (float)(AudioSettings.dspTime - m_startTime) * m_audioSource.pitch - m_offset;
        }
    }

    public void UpdateColPattern()
    {
        if (m_currentColumnPatternIdx < m_columnPatternList.Length - 1)
        {
            m_currentColumnPatternIdx++;
        }
        else
        {
            m_currentColumnPatternIdx = 0;
        }
        m_currentColumnPattern = m_columnPatternList[m_currentColumnPatternIdx];
    }

    public void PlaySong()
    {
        m_audioSource.Play();
        m_startTime = AudioSettings.dspTime;
    }

    public void StopSong()
    {
        m_audioSource.Stop();
    }

    public bool IsSongPlaying()
    {
        return m_audioSource.isPlaying;
    }

    public void PauseSong()
    {
        m_audioSource.Pause();
    }

    public float AudioLength()
    {
        return m_audioSource.clip.length;
    }
}

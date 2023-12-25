using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RhythmGame : MonoBehaviour
{
    public GameManager m_gameManager;
    public GameObject m_buttonNote;
    public GameObject m_popFX;
    public TextMeshProUGUI m_scoreText;
    public TextMeshProUGUI m_resultText;
    public AudioSource m_audio;
    RectTransform m_rectTransform;
    // coord x
    float m_currX;
    float m_minX;
    float m_maxX;
    // coord y
    float m_stepY;
    float m_currY;
    float m_minY;
    float m_maxY;
    // score
    int m_totalScore = 0;
    int m_score = 0;
    bool m_revealResult = false;
    int m_buttonCount = 0;
    // variable for songs
    Conductor m_conductor;
    List<string> m_narrations;
    float m_lastBeat;


    // Start is called before the first frame update
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();

        m_minX = -m_rectTransform.rect.width * 0.4f;
        m_maxX = m_rectTransform.rect.width * 0.4f;

        m_minY = -m_rectTransform.rect.height * 0.35f;
        m_maxY = m_rectTransform.rect.height * 0.35f;

        m_stepY = m_rectTransform.rect.height * 0.1f;
        m_currY = m_minY;

        m_resultText.text = "";
        m_scoreText.text = "Score: " + m_score; // update score
    }

    // Update is called once per frame
    void Update()
    {
        if (m_conductor != null)
        {
            if (m_conductor.IsSongPlaying() && m_lastBeat + m_conductor.m_crochet * m_conductor.m_beatInterval < m_conductor.m_songPosition) {
                SpawnNote();
                m_lastBeat += m_conductor.m_crochet;
                m_scoreText.text = "Score: " + m_score; // update score
            } 
            else if (!m_conductor.IsSongPlaying() && !m_revealResult)
            {
                // if no more children has "notes" tag, end the game
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).CompareTag("Notes"))
                    {
                        return;
                    }
                }
                // result score
                ShowResult();

                // advance story
                m_gameManager.AdvanceSongLevel();
                foreach (string narration in m_narrations) {
                    m_gameManager.AdvanceDialogue(narration);
                }
            }
        }
    }

    void SpawnNote()
    {
        ColumnPattern colPattern = m_conductor.m_currentColumnPattern;
        int noteIdx = colPattern.m_isRandom ? Random.Range(0, colPattern.m_patterns.Length) : (Mathf.RoundToInt(m_conductor.m_songPosition) % colPattern.m_patterns.Length);
        NotePattern pattern = colPattern.m_patterns[noteIdx];
        // decide x pattern
        float x_displacement = 0f;
        if (!colPattern.m_isRandom && colPattern.m_xPlacement.Length > 0)
        {
            x_displacement = colPattern.m_xPlacement[Mathf.RoundToInt(m_conductor.m_songPosition) % colPattern.m_xPlacement.Length];
            m_currX = MapNumber(colPattern.m_xPos);
        }
        else
        {
            m_currX = Random.Range(m_minX, m_maxX);
        }
        Vector3 pos = new(m_currX + m_maxX * x_displacement, m_currY, 0f);
        // update y position
        if (m_currY + m_stepY > m_maxY || m_currY + m_stepY < m_minY)
        {
            m_stepY *= -1;
            m_conductor.UpdateColPattern();
        }
        m_currY += m_stepY;
        // spawn notes
        for (int i = 0; i < pattern.m_noteQuantity; i++)
        {
            if (pattern.m_axis == "x")
            {
                pos.x += m_maxX * pattern.m_distance;
            } 
            else if (pattern.m_axis == "y")
            {
                pos.y += m_maxY * pattern.m_distance;
            }
            if (pos.x <= m_maxX && pos.x >= m_minX && pos.y <= m_maxY && pos.y >= m_minY)
            {
                GameObject note = Instantiate(pattern.m_notePrefab, Vector3.zero, Quaternion.identity, transform);
                note.transform.localPosition = pos - Vector3.forward * i;
                m_totalScore += 100;
                m_buttonCount++;
            }
        }

    }

    public void Reset() {
         if (m_conductor != null) {
            m_conductor.StopSong();
        }
        m_resultText.text = "";
        m_revealResult = false;
        m_conductor = null;
        m_lastBeat = 0.0f;
        m_score = 0;
        m_totalScore = 0;
    }

    public void UpdateScore(int score, Vector2 pos) {
        m_score += score;
        GameObject fx = Instantiate(m_popFX, new Vector3(pos.x, pos.y, -1f), Quaternion.identity, transform);
        Destroy(fx, 3f);
    }

    public void SetConductor(Conductor c)
    {
        m_conductor = c;
        m_conductor.PlaySong();
    }

    public void SetNarration(SongCard songCard) {
        m_narrations = songCard.m_narrations;
    }

    void ShowResult()
    {
        float percentage = m_score * 100 / m_totalScore;
        m_resultText.text = "You scored " + m_score.ToString() + " out of " + m_totalScore.ToString() + " points! \n " + percentage +"%";
        m_revealResult = true;
    }

    float MapNumber(float value)
    {
        // Ensure the input value is within the range (0, 1)
        value = Mathf.Clamp01(value);

        // Map the value to the target range (-0.5, 0.5)
        float mappedValue = (value * (m_maxX - m_minX)) - m_maxX;

        return mappedValue;
    }
}

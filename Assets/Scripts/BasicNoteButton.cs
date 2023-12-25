using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNoteButton : MonoBehaviour
{
    
    public float m_popTime = 2.0f;   
    public GameObject m_outerRing;
    public NoteResult m_noteResult;
    public AudioClip m_hitSound;
    float m_currentTime = 0.0f;
    float m_minSize = 0.4f;
    float m_step;
    List<float> m_timeBoundary = new List<float>();
    bool m_isPopped = false;
    RhythmGame m_rhythmGame;
    RectTransform m_rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
        // get rhythm game component in parent
        m_rhythmGame = GetComponentInParent<RhythmGame>();
        m_step = m_popTime / 5.0f;
        for (int i = 0; i < 5; i++)
        {
            m_timeBoundary.Add(m_popTime - m_step * i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if click on texture?
        if (Application.isEditor)
        {
            if (IsMouseWithinUIElement(m_rectTransform) && !m_isPopped)
            {
                Pop();
            }
        } else
        {
            foreach (Touch t in Input.touches)
            {
                if (IsTouchWithinUIElement(t, m_rectTransform) && !m_isPopped)
                {
                    Pop();
                }
            }
        }
        // scale down the outer ring from 1f to 0.2f over m_popTime
        m_currentTime += Time.deltaTime;
        float scale = Mathf.Lerp(1f, m_minSize, m_currentTime / m_popTime);
        m_outerRing.transform.localScale = new Vector3(scale, scale, scale);

        if (m_currentTime > m_popTime && !m_isPopped) {
            m_isPopped = true;
            m_noteResult.TellResult("Miss");
        }

        if (m_isPopped) {
            gameObject.SetActive(false);
            Destroy(gameObject, 5f);
        }
    }

    public void Pop() {
        m_isPopped = true;
        AudioSource.PlayClipAtPoint(m_hitSound, Camera.main.transform.position);
        string result;
        int score;
        if (m_currentTime <= m_timeBoundary[0] && m_currentTime > m_timeBoundary[1]) {
            result = "Perfect";
            score = 100;
        } else if (m_currentTime <= m_timeBoundary[1] && m_currentTime > m_timeBoundary[2]) {
            result = "Great";
            score = 80;
        } else if (m_currentTime <= m_timeBoundary[2] && m_currentTime > m_timeBoundary[3]) {
            result = "Good";
            score = 50;
        } else if (m_currentTime <= m_timeBoundary[3] && m_currentTime > m_timeBoundary[4]) {
            result = "Bad";
            score = 10;
        } else {
            result = "Miss";
            score = 0;
        }
        m_noteResult.TellResult(result);
        m_rhythmGame.UpdateScore(score, transform.position);
    }

    bool IsTouchWithinUIElement(Touch touch, RectTransform uiElement)
    {
        Vector2 touchPosition = touch.position;

        // Convert touch position to UI space
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiElement, touchPosition, null, out localPoint);

        // Check if the local point is within the UI element's rect
        return uiElement.rect.Contains(localPoint);
    }

    bool IsMouseWithinUIElement(RectTransform uiElement)
    {
        Vector2 touchPosition = Input.mousePosition;

        // Convert touch position to UI space
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiElement, touchPosition, null, out localPoint);

        // Check if the local point is within the UI element's rect
        return Input.GetMouseButtonDown(0) && uiElement.rect.Contains(localPoint);
    }
}

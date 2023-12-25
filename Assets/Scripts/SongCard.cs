using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongCard : MonoBehaviour
{
    public List<string> m_narrations;
    public bool m_isLocked = true;
    public Sprite m_lockedSprite;
    public Sprite m_unlockedSprite;
    public TextMeshProUGUI m_songName;
    public TextMeshProUGUI m_songMetadata;
    public Conductor m_conductor;
    Image m_image;
    Button m_button;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
        m_button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isLocked) {
            m_image.sprite = m_lockedSprite;
            m_button.interactable = false;
            m_songName.text = "Locked";
        } else {
            m_image.sprite = m_unlockedSprite;
            m_button.interactable = true;
            m_songName.text = transform.name;
            m_songMetadata.text = "BPM: " + m_conductor.m_bpm.ToString() + "\n" + "Minutes: " + RoundToOneDecimalPlace((m_conductor.AudioLength() / 60f)).ToString();
        }
    }

    float RoundToOneDecimalPlace(float value)
    {
        // Round to one decimal place
        return Mathf.Round(value * 10.0f) / 10.0f;
    }

    public void Unlock()
    {
        m_isLocked = false;
    }

    public void Lock() {
        m_isLocked = true;
    }
}

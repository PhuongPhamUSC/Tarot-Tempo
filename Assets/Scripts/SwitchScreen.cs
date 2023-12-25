using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SwitchScreen : MonoBehaviour
{
    public GameObject m_playerScreen;
    public GameObject m_deckScreen;
    public GameObject m_rhythmScreen;
    public GameObject m_pauseScreen;
    public GameObject m_environment;
    public TextMeshProUGUI m_dialogue;
    public AudioSource m_globalMusic;

    private static SwitchScreen _instance;
    public static SwitchScreen Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SwitchScreen>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        SwitchToPlayer();
    }

    public void SwitchToPlayer() {
        DestroyNotes();
        m_environment.SetActive(true);
        m_playerScreen.SetActive(true);
        m_deckScreen.SetActive(false);
        m_rhythmScreen.SetActive(false);
        m_pauseScreen.SetActive(false);
        if (!m_globalMusic.isPlaying)
        {
            m_globalMusic.UnPause();
        }
    }

    public void SwitchToDeck() {
        DestroyNotes();
        m_environment.SetActive(false);
        m_playerScreen.SetActive(false);
        m_deckScreen.SetActive(true);
        m_rhythmScreen.SetActive(false);
        m_globalMusic.UnPause();
        m_dialogue.text = "";
    }

    public void SwitchToRhythm() {
        m_environment.SetActive(false);
        m_playerScreen.SetActive(false);
        m_deckScreen.SetActive(false);
        m_rhythmScreen.SetActive(true);
        // pause 
        m_globalMusic.Pause();
        m_dialogue.text = "";
    }

    void DestroyNotes()
    {
        foreach (Transform child in m_rhythmScreen.transform)
        {
            if (child.gameObject.tag == "Notes" || child.gameObject.tag == "VFX")
            {
                Destroy(child.gameObject);
            }
        }
    }
}

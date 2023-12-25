using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool m_isGamePaused = false;
    public SongCard[] m_songCards;
    public ConvoTrigger[] m_convoTriggers;
    //map name of xml file to convo trigger
    public Dictionary<string, ConvoTrigger> m_convoTriggerDict = new Dictionary<string, ConvoTrigger>();
    int m_currentSongIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        // set dictionary
        for (int i = 0; i < m_convoTriggers.Length; i++) {
            m_convoTriggerDict.Add(m_convoTriggers[i].m_xmlFile, m_convoTriggers[i]);
        }
        // print dictionary
        foreach (var currConvoTrigger in m_convoTriggerDict) {
            Debug.Log(currConvoTrigger.Key + " " + currConvoTrigger.Value);
        }
        ResetGame();
        m_songCards[m_currentSongIndex].GetComponent<SongCard>().Unlock();
    }

    public void AdvanceSongLevel() {
        if (m_currentSongIndex < m_songCards.Length - 1) {
            m_currentSongIndex++;
        }
        m_songCards[m_currentSongIndex].GetComponent<SongCard>().Unlock();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        m_isGamePaused = true;
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        // In the Unity Editor, stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // In a standalone build or other platform, quit the application
        Application.Quit();
    #endif
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        m_isGamePaused = false;
    }

    public void ResetGame() {
        m_currentSongIndex = 0;
        // iterate over dictionary m_convoTriggerDict and reset each convo trigger
        foreach (var currConvoTrigger in m_convoTriggerDict) {
            currConvoTrigger.Value.Reset();
        }
        for (int i = 0; i < m_songCards.Length; i++) {
            if (i > 0) m_songCards[i].GetComponent<SongCard>().Lock();
        }
        ResumeGame();
    }

    public void AdvanceDialogue(string xmlFile) {
        m_convoTriggerDict[xmlFile].AdvanceDialogue();
    }
}

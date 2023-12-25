using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class ConvoTrigger : MonoBehaviour
{
    public float m_maxDist = 10.0f;
    public float m_delayTime = 2.0f;
    public string m_xmlFile;
    public bool m_isRandom = false;
    DialogueBox dialogueBox;
    int m_convoStage = 0;
    DialogueScript m_dialogueScript;

    public class DialogueScript
    {
        public List<List<string>> dialogue;
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogueBox = DialogueBox.Instance;
        {
            var textFile = Resources.Load<TextAsset>("Dialogue/" + m_xmlFile);
            if (textFile == null) {
                Debug.LogError("Could not load " + m_xmlFile);
            } else {
                StringReader stringReader = new StringReader(textFile.text);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DialogueScript));
                m_dialogueScript = xmlSerializer.Deserialize(stringReader) as DialogueScript;
            }
        }
    }

    void Update() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, m_maxDist))
        {
            if (hit.transform.gameObject == gameObject)
            {
                PlayDialogue(m_delayTime);
            }
        }
    }

    public void PlayDialogue(float time = 1.5f)
    {
        if (m_isRandom)
        {
            int randomIdx = Random.Range(0, m_dialogueScript.dialogue.Count);
            dialogueBox.SetText(m_dialogueScript.dialogue[randomIdx]);
        }
        else
        {
            dialogueBox.SetText(m_dialogueScript.dialogue[m_convoStage]);
        }
        dialogueBox.StartDialogue(time);
    }

    public void AdvanceDialogue() {
        if (m_convoStage < m_dialogueScript.dialogue.Count - 1) {
            m_convoStage++;
        }
    }

    public void Reset() {
        if (!m_isRandom) {
            m_convoStage = 0;
        }
    }
}

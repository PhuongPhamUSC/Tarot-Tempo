using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteResult : MonoBehaviour
{
    public float m_delayTime = 0.5f;
    public Color m_missColor;
    public Color m_badColor;

    TextMeshProUGUI m_resultText;

    void Start() {
        m_resultText = GetComponent<TextMeshProUGUI>();
        m_resultText.text = "";
    }

    public void TellResult(string res) {
        m_resultText.text = res;
        if (res == "Miss")
            m_resultText.color = m_missColor;
        else if (res == "Bad")
            m_resultText.color = m_badColor;
        StartCoroutine(DestroyParent());
    }

    IEnumerator DestroyParent() {
        yield return new WaitForSeconds(m_delayTime);
        Destroy(transform.parent.gameObject);
    }
}

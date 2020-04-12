using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.SpeechBubble
{
    public class SpeechBubbleText : MonoBehaviour
    {
        [SerializeField] bool m_RandomDialog = false;
        [SerializeField] float m_ShowTime = 10f;
        [SerializeField] float m_ChangeTime = 5f;

        [SerializeField] Canvas m_canvas;
        [SerializeField] Text m_BubbleText = null;

        [SerializeField] string[] m_SpeechText;

        bool m_Visible = false;
        int m_NextText = 0;
        int m_Length;
        float m_TimePassed;

        private void Start()
        {
            m_Length = m_SpeechText.Length;
            float m_TimePassed = 0;
            SetSpeechBubbleText(m_SpeechText[0]);
        }

        private void Update()
        {
            if (m_TimePassed >= m_ShowTime && m_Length>0 && m_Visible)
            {
                SetSpeechBubbleOnOff(false);
                if (m_TimePassed >= (m_ShowTime + m_ChangeTime))
                {
                    m_TimePassed = 0.0f;
                    ChangeDialog();
                    SetSpeechBubbleOnOff(true);
                }
            }
            m_TimePassed += Time.deltaTime;
        }

        private void ChangeDialog()
        {
            if (m_RandomDialog)
            {
                m_NextText = UnityEngine.Random.Range(0, m_Length);
            }
            else if (m_NextText<m_Length-1)
            {
                m_NextText += 1;
            }
            else
            {
                m_NextText = 0;
            }
            SetSpeechBubbleText(m_SpeechText[m_NextText]);
        }

        private void SetSpeechBubbleText(string newtext)
        {
            m_BubbleText.text = newtext;
        }

        private void SetSpeechBubbleOnOff(bool val)
        {
            m_canvas.enabled = val;
        }

        public void MakeSpeechBubbleVisible(bool val)
        {
            m_Visible = val;
            SetSpeechBubbleOnOff(val);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.SceneManagement
{
    //A Simple class for fading an image in and out.
    public class Fader : MonoBehaviour
    {
        CanvasGroup m_CanvasGroup;
        Coroutine m_ActiveFade = null;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1,time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (m_ActiveFade != null)
            {
                StopCoroutine(m_ActiveFade);
            }
            m_ActiveFade = StartCoroutine(FadeRoutine(target, time));
            return  m_ActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(m_CanvasGroup.alpha, target))
            {
                m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

        public void FadeOutImmediate()
        {
            m_CanvasGroup.alpha = 1;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class XPDisplay : MonoBehaviour
    {
        Experience m_XP;

        public void Awake()
        {
            m_XP = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", m_XP.GetExperiencePoints());
        }
    }
}

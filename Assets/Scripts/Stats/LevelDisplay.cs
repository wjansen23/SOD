using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats m_BaseStatsComp;

        private void Awake()
        {
            m_BaseStatsComp = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }
        private void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", m_BaseStatsComp.GetLevel());
        }
    }
}

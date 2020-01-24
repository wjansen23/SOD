using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats m_BaseStatsComp;
        //IBaseStatProvider m_BaseStatsComp;

        private void Awake()
        {
            m_BaseStatsComp = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            //m_BaseStatsComp = GameObject.FindWithTag("Player").GetComponent<IBaseStatProvider>();
        }
        private void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", m_BaseStatsComp.GetLevel());
        }
    }
}

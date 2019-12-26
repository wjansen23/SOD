using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        CharacterCombat m_CharCombat;

        public void Awake()
        {
            m_CharCombat = GameObject.FindWithTag("Player").GetComponent<CharacterCombat>();
        }

        private void Update()
        {
            Health targetHealth = m_CharCombat.GetTarget();

            if (targetHealth != null && !targetHealth.IsDead())
            {
                GetComponent<Text>().text = String.Format("{0:0}/{1:0}", targetHealth.GetHealthPoints(), targetHealth.GetMaxHealthPoints());
            }
            else
            {
                GetComponent<Text>().text = "NO TARGET";
            }

        }
    }
}

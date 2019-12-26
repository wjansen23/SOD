using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health m_Health;
        

        public void Awake()
        {
            m_Health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}",m_Health.GetHealthPoints(),m_Health.GetMaxHealthPoints());  
        }
    }
}

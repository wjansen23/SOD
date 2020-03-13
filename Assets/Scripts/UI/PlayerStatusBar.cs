using RPG.Stats;
using RPG.Attributes;
using UnityEngine;

namespace RPG.UI
{
    public class PlayerStatusBar : MonoBehaviour
    {
        [SerializeField] RectTransform m_DamageBar = null;
        [SerializeField] RectTransform m_ExhaustBar = null;
        [SerializeField] RectTransform m_XPBar= null;

        Health m_Health = null;
        BaseStats m_BaseComp = null;
        Experience m_XPComp = null;

        private void Start()
        {
           m_Health=GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
           m_BaseComp = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
           m_XPComp = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
            UpdateExhaustBar();
            UpdateXPBar();
        }

        private void UpdateXPBar()
        {
            int currentLvl = m_BaseComp.GetLevel();
            float currentXP = m_XPComp.GetExperiencePoints();
            float neededXP = m_BaseComp.GetXPForNextLevel(currentLvl);
            float previousXP = m_BaseComp.GetXPForNextLevel(currentLvl-1);

            float xppercent = (neededXP - currentXP) / (neededXP - previousXP);
            m_XPBar.localScale = new Vector3(1-xppercent, 1, 1); 
        }

        private void UpdateExhaustBar()
        {
            m_ExhaustBar.localScale = new Vector3(0, 1, 1);   //TODO: ADD Exhaustion
        }

        void UpdateHealthBar()
        {
            float healthpercent = 0f;
            if (m_Health != null)
            {
                healthpercent = m_Health.GetFractionalHealth();
            }

            m_DamageBar.localScale = new Vector3(1 - healthpercent, 1, 1);
        }
    }
}


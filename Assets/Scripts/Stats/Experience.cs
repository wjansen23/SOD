using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float m_ExperiencePoints=0;

        //public delegate void ExperienceGainedDelegate();
        public event Action onExperienceGained;

        public void GainExperiece(float xpGained)
        {
            m_ExperiencePoints += xpGained;
            onExperienceGained();
        }

        public float GetExperiencePoints()
        {
            return m_ExperiencePoints;
        }

        public object CaptureState()
        {
            return m_ExperiencePoints;
        }

        public void RestoreState(object state)
        {
            //Get Health Points
            m_ExperiencePoints = (float)state;
        }
    }
}

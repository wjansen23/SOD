using UnityEngine;
using System;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int m_StartLevel = 1;
        [SerializeField] CharacterClass m_CharacterClass;
        [SerializeField] Progression m_Progression = null;
        [SerializeField] GameObject m_LevelUpFX = null;
        [SerializeField] bool m_ShouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> m_CurrentLevel;
        Experience xpComp;

        //Grab component references here
        private void Awake()
        {
            xpComp = GetComponent<Experience>();
            m_CurrentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            m_CurrentLevel.ForceInit();
        }

        //Register for events here
        private void OnEnable()
        {
            if (xpComp != null)
            {
                xpComp.onExperienceGained += UpdateLevel;
            }
        }

        //Unregister for events here
        private void OnDisable()
        {
            if (xpComp != null)
            {
                xpComp.onExperienceGained -= UpdateLevel;
            }
        }

        //check to see if the characters level has changed.
        private void UpdateLevel()
        {
            int newlevel = CalculateLevel();
            if (newlevel > m_CurrentLevel.value)
            {
                m_CurrentLevel.value = newlevel;
                PlayLevelUpEffect();
                onLevelUp();
            }
        }

        private void PlayLevelUpEffect()
        {
            Instantiate(m_LevelUpFX, transform);
        }

        //Get the sent stat value
        public float GetStatValue(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat))*(1+ GetPercentModifier(stat));
        }

        private float GetBaseStat(Stat stat)
        {
            return m_Progression.GetStat(stat, m_CharacterClass, GetLevel());
        }
        
        //Get additive multiples for stat
        private float GetAdditiveModifier(Stat stat)
        {
            if (!m_ShouldUseModifiers) return 0;
            float totalmodifiers = 0;

            foreach (IModifierProvider modprovider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in modprovider.GetAdditiveModifiers(stat))
                {
                    totalmodifiers += modifier;
                }                
            }
            return totalmodifiers;
        }

        //Get percentage modifiers for stat
        private float GetPercentModifier(Stat stat)
        {
            if (!m_ShouldUseModifiers) return 0;
            float totalmodifiers = 0;

            foreach (IModifierProvider modprovider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in modprovider.GetPercentageModifiers(stat))
                {
                    totalmodifiers += modifier;
                }
            }
            return totalmodifiers;
        }

        public int GetLevel()
        {
            return m_CurrentLevel.value;
        }

        //Compute the current level of a character
        public int CalculateLevel()
        {
            if (xpComp == null) return m_StartLevel;

            float currentXP = xpComp.GetExperiencePoints();
            int maxlevel = m_Progression.GetMaxLevels(Stat.LevelXP,m_CharacterClass);

            for (int level = 1;level <= maxlevel; level++)
            {
                if (m_Progression.GetStat(Stat.LevelXP, m_CharacterClass, level) > currentXP) return level;
            }
            return maxlevel+1;
        }

        //Return XP for Next Level
        public float GetXPForNextLevel(int level)
        {
            int maxlevel = m_Progression.GetMaxLevels(Stat.LevelXP, m_CharacterClass);

            if (level >= maxlevel){return 10000000000.0f;}
            if (level <= 0) { return 0; }
            return m_Progression.GetStat(Stat.LevelXP, m_CharacterClass, level);
        }
    }
}

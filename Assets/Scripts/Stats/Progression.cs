using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {

        [SerializeField] ProgressionCharacterClass[] m_characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> m_StatLookupTable;


        public float GetStat(Stat stat, CharacterClass classtype, int level)
        {
            BuildLookup();
            float[] levels = m_StatLookupTable[classtype][stat];

            if (levels.Length < level) return 0;
            return levels[level-1];
        }

        public int GetMaxLevels(Stat stat,CharacterClass classtype)
        {
            BuildLookup();

            float[] levels = m_StatLookupTable[classtype][stat];
            return levels.Length;
        }

        //Build the stat lookup table if it currently does not exist
        private void BuildLookup()
        {
            //Check to see if lookup table exists.  If so, don't build it again
            if (m_StatLookupTable != null) return;

            m_StatLookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass pclass in m_characterClasses)
            {
                Dictionary<Stat, float[]> statlookup = new Dictionary<Stat, float[]>();
                foreach (ProgressionStats pstat in pclass.stats)
                {
                    statlookup[pstat.m_Stat] = pstat.levels;
                }
                m_StatLookupTable[pclass.m_CharacterClass] = statlookup;                   
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass m_CharacterClass;
            public ProgressionStats[] stats;
        }

        [System.Serializable]
        class ProgressionStats
        {
            public Stat m_Stat;
            public float[] levels;
        }
    }
}


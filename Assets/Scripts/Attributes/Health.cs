using UnityEngine;
using UnityEngine.Events;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using GameDevTV.Utils;
using System;

namespace RPG.Attributes
{
    //This class handles all behavoir associated with a character having health
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float m_LevelUpHealthPercent = 0.7f;
        [SerializeField] TakeDamageEvent takeDamageEvent;
        [SerializeField] UnityEvent onDieEvent;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        static string DEATH_TRIGGER = "die";

        LazyValue<float> m_CurrentHealth;

        float m_MaxHealth = 100f;
        bool m_isDead = false;

        BaseStats BStatComp;

        private void Awake()
        {
            BStatComp = GetComponent<BaseStats>();
            m_CurrentHealth = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return BStatComp.GetStatValue(Stat.LevelHealth);
        }

        private void Start()
        {
            m_CurrentHealth.ForceInit();
        }

        //Register for events here
        private void OnEnable()
        {
            if (BStatComp != null)
            {
                BStatComp.onLevelUp += LevelUpHealth;
            }
        }

        //Unregister for events here
        private void OnDisable()
        {
            if (BStatComp != null)
            {
                BStatComp.onLevelUp -= LevelUpHealth;
            }
        }

        //Adjusts current health based upon leveling up
        private void LevelUpHealth()
        {
            float leveluphealth = GetMaxHealthPoints() * m_LevelUpHealthPercent;
            m_CurrentHealth.value = Mathf.Max(m_CurrentHealth.value,leveluphealth);
        }

        //Return whether the character is dead
        public bool IsDead()
        {
            return m_isDead;
        }

        //Reduce character health by the damage amount.
        public void TakeDamage(float damage, GameObject instigator)   //TODO: RANDOMIZE HIT SOUNDS
        {
            //Ensure health does not go to zero or greater than Max
            m_CurrentHealth.value = Mathf.Clamp(m_CurrentHealth.value - damage, 0, GetMaxHealthPoints());
            takeDamageEvent.Invoke(damage);

            //Check to see if character is dead
            if (m_CurrentHealth.value <= 0 )
            {
                onDieEvent.Invoke();
                Die();
                AwardExperience(instigator);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience xpComp = instigator.GetComponent<Experience>();

            if (xpComp == null) return;
            xpComp.GainExperiece(BStatComp.GetStatValue(Stat.XPReward));
        }

        public float GetHealthPoints()
        {
            return m_CurrentHealth.value;
        }

        public float GetMaxHealthPoints()
        {
            return BStatComp.GetStatValue(Stat.LevelHealth);
        }

        public float GetHealthPercentage()
        {
            return 100*GetFractionalHealth(); 
        }

        public float GetFractionalHealth()
        {
            return (m_CurrentHealth.value / BStatComp.GetStatValue(Stat.LevelHealth));
        }

        //Executate death behavoir for the character
        private void Die()
        {
            //Don't repeat death behavoirs if already dead.
            if (m_isDead) return;

            //Set Flag
            m_isDead = true;

            //Set Death Animation
            GetComponent<Animator>().SetTrigger(DEATH_TRIGGER);

            //Stop current action
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return m_CurrentHealth.value;
        }

        public void RestoreState(object state)
        {
            //Get Health Points
            m_CurrentHealth.value = (float)state;

            //Check if dead and if so call die routine
            if (m_CurrentHealth.value <= 0) Die();
        }

        public void RestoreHealth(float amount)
        {
            m_CurrentHealth.value = Mathf.Clamp(m_CurrentHealth.value + amount, 0, GetMaxHealthPoints());
        }
    }
}

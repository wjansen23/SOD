using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    //This class handles all character combat actions.
    public class CharacterCombat : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float m_TimeBetweenAttacks=1f;
        [SerializeField] float m_AttackSpeedModifier = 1.2f;
        [SerializeField] Transform m_RTHandTransform = null;
        [SerializeField] Transform m_LFHandTransform = null;
        [SerializeField] WeaponConfig m_DefaultWeapon = null;

        static string ATTACK_TRIGGER = "attack";
        static string STOP_ATTACK_TRIGGER = "stopAttack";

        float m_TimeSinceLastAttack = Mathf.Infinity;

        Health m_Target;
        WeaponConfig m_CurrentWeaponConfig;
        LazyValue<Weapon> m_CurrentWeapon;

        private void Awake()
        {
            m_CurrentWeaponConfig = m_DefaultWeapon;
            m_CurrentWeapon = new LazyValue<Weapon>(SetUpDefaultWeapon);
        }

        private Weapon SetUpDefaultWeapon()
        {
            return AttachWeapon(m_DefaultWeapon);
        }

        // Start is called before the first frame update
        void Start()
        {
            m_CurrentWeapon.ForceInit();
        }

        // Update is called once per frame
        void Update()
        {
            //Increment time since last attack. Used to delay how often a
            //character can execute an attack.
            m_TimeSinceLastAttack += Time.deltaTime;

            //If no target is selected for attack then return
            if (m_Target == null) return;

            //If the selected Target is dead then return
            if (m_Target.IsDead()) return;

            //If a target is select, check to see if in range of current weapon
            if (!InWeaponRange(m_Target.transform))
            {
                //Move closer to the target
                GetComponent<Mover>().MoveTo(m_Target.transform.position,m_AttackSpeedModifier);
            }
            else
            {
                //In weapon range. Attack target.
                GetComponent<Mover>().Cancel();
                AttackBehavoir();
            }
        }

        //Spawn the current weapon for the character
        public void EquipWeapon(WeaponConfig weapon)
        {
            m_CurrentWeaponConfig = weapon;
            m_CurrentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator m_ANIMATOR = GetComponent<Animator>();
            return weapon.Spawn(m_RTHandTransform, m_LFHandTransform, m_ANIMATOR);
        }

        public Health GetTarget()
        {
            return m_Target;
        }

        //Executes the attack behavoir for a character
        void AttackBehavoir()
        {
            //Look at enemy
            transform.LookAt(m_Target.transform);

            //Make sure enough time has passed since last attack
            if (m_TimeSinceLastAttack >= m_TimeBetweenAttacks)
            {
                //Trigger the attack event
                TriggerAttack();

                //Reset time since last attack
                m_TimeSinceLastAttack = 0;
            }
        }

        //Executes the attack animation for the character
        void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger(STOP_ATTACK_TRIGGER);
            GetComponent<Animator>().SetTrigger(ATTACK_TRIGGER);
        }

        //Called from "melee" animation.  We will deal damage here.
        void Hit()
        {
            //Make sure character still has a target
            if (m_Target == null) { return; }

            //Check current weapon instance
            if (m_CurrentWeapon.value != null)
            {
                m_CurrentWeapon.value.OnHit();
            }

            //Invoke Damage
            m_Target.TakeDamage(GetLevelBasedDamage(), gameObject);
        }

        //Grabs the damage done based on level
        private float GetLevelBasedDamage()
        {
            return GetComponent<BaseStats>().GetStatValue(Stat.LevelDamage);
        }

        //Called from projectile attack animation.
        void Shoot()
        {
            //Make sure character still has a target
            if (m_Target == null) { return; }

            //Make sure our current weapon is a projectile weapon
            if (m_CurrentWeaponConfig.IsProjectile())
            {
                // We will start the projectile moving here
                m_CurrentWeaponConfig.LaunchProjectile(m_Target.GetComponent<Health>(), m_RTHandTransform, m_LFHandTransform, gameObject, GetLevelBasedDamage());
            }
            //Hit();
        }

        //Cancel attacking animation
        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger(ATTACK_TRIGGER);
            GetComponent<Animator>().SetTrigger(STOP_ATTACK_TRIGGER);
        }

        //Stop the current attack behavoir
        public void Cancel()
        {
            //Stop attack
            StopAttack();

            //Stop Movement
            GetComponent<Mover>().Cancel();

            //Reset target
            m_Target = null;
        }

        //Is the current target select something that can be attacked
        public bool CanAttack(GameObject target)
        {
            //Check if target is null
            if (target == null) { return false; }
            if (!GetComponent<Mover>().CanMoveTo(target.transform.position) && !InWeaponRange(target.transform)) { return false; }

            //Check if target is alive
            Health targetHealth = target.GetComponent<Health>();
            return targetHealth!=null && !targetHealth.IsDead();
        }

        //Sets the target.  Damage will occur at the hit() point in the animation
        public void Attack(GameObject target)
        {
            //Add to action scheduler
            GetComponent<ActionScheduler>().StartAction(this);

            //Set target to Health Component
            m_Target = target.GetComponent<Health>();        
        }

        //Check if within range of current weapon
        bool InWeaponRange(Transform targetTransform)
        {
            return Vector3.Distance(this.transform.position, targetTransform.position)<=m_CurrentWeaponConfig.GetRange();
        }

        public object CaptureState()
        {
            return m_CurrentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponname = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponname);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.LevelDamage)
            {
                yield return m_CurrentWeaponConfig.GetDamage();
            }          
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.LevelDamage)
            {
                yield return m_CurrentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}

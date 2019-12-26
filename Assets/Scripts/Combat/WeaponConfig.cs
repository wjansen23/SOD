using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName ="Weapon",menuName = "Weapons/Make New Weapon",order =0)]

    public class WeaponConfig: ScriptableObject
    {
        [SerializeField] AnimatorOverrideController m_WeaponAOC = null;
        [SerializeField] Weapon m_EquippedWeaponPreFab = null;
        [SerializeField] float m_WeaponRange = 1.5f;
        [SerializeField] float m_WeaponDamage = 12f;
        [SerializeField] float m_PercentageBonus = 0;
        [SerializeField] bool m_isRTHanded = true;
        [SerializeField] Projectile m_Projectile = null;

        const string WEAPON_NAME = "Weapon";

        //Spawns the weapon into the characters hand
        public Weapon Spawn(Transform rtHand, Transform lfHand, Animator animator)
        {
            DestroyOldWeapon(rtHand, lfHand);
            Weapon weapon = null;

            if (m_EquippedWeaponPreFab != null)
            {
                weapon = Instantiate(m_EquippedWeaponPreFab, GetWeaponHandTransform(rtHand, lfHand));
                weapon.gameObject.name = WEAPON_NAME;
            }

            //Check to see if the weapon has an override controller if not check to see if one has been used before
            //an revert to default controller to avoid playing wrong animation
            if (m_WeaponAOC != null)
            {
                animator.runtimeAnimatorController = m_WeaponAOC;
            }
            else
            {
                var overridecontroller = animator.runtimeAnimatorController as AnimatorOverrideController;
                if (overridecontroller != null)
                {
                    animator.runtimeAnimatorController = overridecontroller.runtimeAnimatorController;
                }
            }

            return weapon;
        }

        //Destroys whatever weapon the player has in their hand
        private void DestroyOldWeapon(Transform rtHand, Transform lfHand)
        {
            Transform oldweapon = rtHand.Find(WEAPON_NAME);
            if (oldweapon == null)
            {
                oldweapon = lfHand.Find(WEAPON_NAME);
            }
            if (oldweapon == null) return;

            //To avoid destroying the wrong weapon
            oldweapon.name = "DESTROYING";
            Destroy(oldweapon.gameObject);
        }

        private Transform GetWeaponHandTransform(Transform rtHand, Transform lfHand)
        {
            Transform hand;
            if (m_isRTHanded)
            {
                hand = rtHand;
            }
            else
            {
                hand = lfHand;
            }
            return hand;
        }

        //Returns weapon damage
        public float GetDamage()
        {
            return m_WeaponDamage;
        }

        //Returns weapon damage percentage bonus
        public float GetPercentageBonus()
        {
            return m_PercentageBonus;
        }

        //Returns range of the weapon
        public float GetRange()
        {
            return m_WeaponRange;
        }

        //Returns whether or not the weapon has a projectile
        public bool IsProjectile()
        {
            return m_Projectile != null;
        }

        //Spawns the projectile and fires it
        public void LaunchProjectile(Health target, Transform rtHand, Transform lfHand, GameObject instigator, float basedamage)
        {
            if (m_Projectile != null)
            {
                Projectile projectile = Instantiate(m_Projectile, GetWeaponHandTransform(rtHand, lfHand).position,Quaternion.identity);
                projectile.SetTarget(target, basedamage, instigator);
            }
        }
    }
}
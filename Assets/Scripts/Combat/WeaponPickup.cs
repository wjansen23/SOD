using System.Collections;
using UnityEngine;
using RPG.Control;
using RPG.Attributes;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig m_Weapon = null;
        [SerializeField] float m_HealthToHeal = 0f;
        [SerializeField] float m_RespawnTime = 5.0f;

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player") return;
            Pickup(other.gameObject);
        }

        private void Pickup(GameObject subject)
        {
            if (m_Weapon != null)
            {
                subject.GetComponent<CharacterCombat>().EquipWeapon(m_Weapon);
            }

            if (m_HealthToHeal > 0)
            {
                subject.GetComponent<Health>().RestoreHealth(m_HealthToHeal);
            }

            StartCoroutine(HideForSeconds(m_RespawnTime));
        }

        private IEnumerator HideForSeconds(float time)
        {
            ShowHidePickup(false);
            yield return new WaitForSeconds(time);
            ShowHidePickup(true);
        }

        private void ShowHidePickup(bool visible)
        {
            GetComponent<SphereCollider>().enabled = visible;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(visible);   
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            // if (Input.GetMouseButtonDown(0))
            //{
            //    Pickup(callingController.gameObject.GetComponent<CharacterCombat>());
            //}
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent m_OnHitEvent;

        public void OnHit()
        {
            m_OnHitEvent.Invoke();
        }
    }
}
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] float m_Speed = 1;
        [SerializeField] float m_HitHeightPercent = 0.65f;
        [SerializeField] float m_BonusDamage = 0;
        [SerializeField] float m_MaxLifeTime = 10.0f;
        [SerializeField] float m_LifeAfterImpact = 0.5f;
        [SerializeField] bool m_Homing = false;
        [SerializeField] GameObject m_HitEffect = null;
        [SerializeField] GameObject[] m_DestroyOnHit = null;

        [SerializeField] UnityEvent onProjectileHitEvent;


        Health m_Target = null;
        GameObject m_Instigator;
        float m_BaseDamage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLoation());
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Target == null) return;

            if (m_Homing && !m_Target.IsDead())
            {
                transform.LookAt(GetAimLoation());
            }
            transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
        }

        //find the mid point of the targets capsule collider assuming initial transform position is at the feet.
        private Vector3 GetAimLoation()
        {
            CapsuleCollider targetCapsule = m_Target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) return m_Target.transform.position;
            return (m_Target.transform.position + Vector3.up * targetCapsule.height * m_HitHeightPercent);
        }

        //Set the target for the projectile
        public void SetTarget(Health target, float baseDamage, GameObject instigator)
        {
            m_Target = target;
            m_BaseDamage = baseDamage;
            m_Instigator = instigator;

            //Destroy the game object after a certain amount of time
            Destroy(gameObject, m_MaxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != m_Target) return;
            if (other.GetComponent<Health>().IsDead()) return;

            onProjectileHitEvent.Invoke();

            if (m_HitEffect != null)
            {
                Instantiate(m_HitEffect, transform.position, transform.rotation);
            }
            m_Target.TakeDamage(m_BaseDamage + m_BonusDamage,m_Instigator);

            foreach(GameObject todestroy in m_DestroyOnHit)
            {
                Destroy(todestroy);
            }
            Destroy(gameObject, m_LifeAfterImpact);
        }
    }
}

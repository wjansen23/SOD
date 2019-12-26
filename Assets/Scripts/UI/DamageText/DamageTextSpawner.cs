using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText m_damageTextPrefab = null;

        public void Spawn(float damageAMT)
        {
            DamageText instance = Instantiate<DamageText>(m_damageTextPrefab, transform);
            instance.SetDamageText(damageAMT);
        }

    }
}

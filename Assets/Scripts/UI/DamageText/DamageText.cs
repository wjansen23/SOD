using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {

        [SerializeField] Text m_DamageText = null;

        public void SetDamageText(float damageAMT)
        {
            m_DamageText.text = String.Format("{0:0}",damageAMT);
        }
    }

}
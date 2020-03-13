using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health m_Health=null;
        [SerializeField] RectTransform m_BarForeground=null;
        [SerializeField] Canvas m_CanvasComp = null;

        // Update is called once per frame
        void Update()
        {
            float healthpercent = 0f;
            if (m_Health != null)
            {
                 healthpercent = m_Health.GetFractionalHealth();
            }
            
            if (Mathf.Approximately(healthpercent,0) || Mathf.Approximately(healthpercent, 1))
            {
                m_CanvasComp.enabled = false;
                return;
            }
            m_CanvasComp.enabled = true;
            m_BarForeground.localScale = new Vector3(healthpercent, 1, 1);
        }
    }
}

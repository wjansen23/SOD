using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RPG.Core
{
    //A very simple camera that will follow the player
    public class FollowCamera : MonoBehaviour
    {

        //[SerializeField] Transform m_target;
        [SerializeField] float m_MinZoomLevel=15f;
        [SerializeField] float m_MaxZoomLevel=30f;
        [SerializeField] float m_ZoomScale = 1f;

        float m_StandardZoomLevel;

        CinemachineVirtualCamera m_Camera;

        private void Start()
        {
            m_Camera = GetComponent<CinemachineVirtualCamera>();
            m_StandardZoomLevel = m_Camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (Input.mouseScrollDelta.y!=0) ChangeZoomLevel(Input.mouseScrollDelta.y);
        }

        void ChangeZoomLevel(float delta)
        {
            float currentzoom = m_Camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
            float zoomchange = delta * m_ZoomScale;

            if (zoomchange < 0)
            {
                m_Camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = Mathf.Min(m_MaxZoomLevel, currentzoom - zoomchange);
            }

            if (zoomchange > 0)
            {
                m_Camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = Mathf.Max(m_MinZoomLevel, currentzoom - zoomchange);
            }

        }
    }
}

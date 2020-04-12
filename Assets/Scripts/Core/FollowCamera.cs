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
        [SerializeField] float m_RotateScale = 50f;

        bool m_bMouse;
        float m_StandardZoomLevel;

        CinemachineVirtualCamera m_Camera;
        Transform m_Transform;

        private void Start()
        {
            m_Camera = GetComponent<CinemachineVirtualCamera>();
            m_Transform = GetComponent<Transform>();

            m_StandardZoomLevel = m_Camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (Input.mouseScrollDelta.y!=0) ChangeZoomLevel(Input.mouseScrollDelta.y);

            if (Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.E))
            {
                RotateCameraYAxis(m_RotateScale);
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
            {
                RotateCameraYAxis(-m_RotateScale);
            }

        }

        //Allows for limited zooming in or out based upon the max and min zoom level
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

        //Rotates the camera around its Y axis based upon the delta and scaled by time.deltatime.
        void RotateCameraYAxis(float delta)
        {
            m_Transform.Rotate(0, delta*Time.deltaTime , 0, Space.World);
        }
    }
}

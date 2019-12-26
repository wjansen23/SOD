using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    //A very simple camera that will follow the player
    public class FollowCamera : MonoBehaviour
    {

        [SerializeField] Transform m_target;


        // Update is called once per frame
        void LateUpdate()
        {
            //Move to player position
            transform.position = m_target.position;
        }
    }
}

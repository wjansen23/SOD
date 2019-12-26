using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    //This class is used to set triggers within a scene to enable cinematics
    // or cutscenes to be played.
    public class CinematicTrigger : MonoBehaviour
    {

        bool m_AlreadyTrigger = false;

        private void OnTriggerEnter(Collider other)
        {
            //Only let the player fire off the trigger to play the
            //cinematic or cutscene.
            if(!m_AlreadyTrigger && other.gameObject.tag == "Player")
            {
                m_AlreadyTrigger = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
    }
}

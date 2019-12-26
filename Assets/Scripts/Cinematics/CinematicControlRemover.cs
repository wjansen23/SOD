using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    //This class contains code to control player interaction during a cinematic
    //moment or cutscene.

    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject m_Player;

        private void Awake()
        {
            //Find the player in the scene
            m_Player = GameObject.FindWithTag("Player");
        }

        //Register for events here
        private void OnEnable()
        {
            //Register functions to be called when specific PlayableDirector functions
            //are executed.
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        //Unregister for events here
        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector pd)
        {
            //Cancel current player actions and diable the player controller to 
            //prevent movement
            m_Player.GetComponent<ActionScheduler>().CancelCurrentAction();
            m_Player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector pd)
        {
            //Re-enable player movement
            m_Player.GetComponent<PlayerController>().enabled = false;
        }
    }
}

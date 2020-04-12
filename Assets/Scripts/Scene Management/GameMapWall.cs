using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class GameMapWall : MonoBehaviour
    {

        ParticleSystem m_ParticleSystem;

        static string PLAYER_TAG = "Player";

        // Start is called before the first frame update
        void Start()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            m_ParticleSystem.Stop();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == PLAYER_TAG)
            {
                m_ParticleSystem.Play();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == PLAYER_TAG)
            {
                m_ParticleSystem.Stop();
            }
        }

    }
}

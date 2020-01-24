using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using RPG.Control;
using RPG.Core;

namespace RPG.SceneManagement
{
    //A class for creating portals between different scenes
    public class Portal : MonoBehaviour
    {
        //Allows up to five different portals to exist
        enum DestinationID
        {
            A, B, C, D, E
        }

        [SerializeField] int m_SceneIndexToLoad=-1;
        [SerializeField] Transform m_SpawnPoint;
        [SerializeField] DestinationID m_DestinationID;
        [SerializeField] float m_FadeOutTime = 2f;
        [SerializeField] float m_FadeInTime = 1f;
        [SerializeField] float m_FadeWaitTime = .25f;

        //A portal is a volume location within the scene.  Call whenever something enters.
        private void OnTriggerEnter(Collider other)
        {
            //Only let the player cause a transition
            if (other.tag=="Player")
            {
                //Make sure there is a scene to load
                if (m_SceneIndexToLoad != -1)
                {
                    StartCoroutine(Transition());
                }
            }
        }


        //Transition to new scene
        private IEnumerator Transition()
        {           
            if (m_SceneIndexToLoad < 0)
            {
                Debug.LogError("Scene to load not set");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            //Make sure the portals isn't destoryed yet when loading a new scene.
            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savewrapper = FindObjectOfType<SavingWrapper>();

            //Disable the player controller tovprevent movement
            PlayerController playercontrolold = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playercontrolold.enabled = false;

            //Fade out and wait for new scene to load
            yield return fader.FadeOut(m_FadeOutTime);

            //Save current Scene
            savewrapper.Save();

            //change to new Scene
            yield return SceneManager.LoadSceneAsync(m_SceneIndexToLoad);

            //Disable the player controller tovprevent movement
            PlayerController playercontrolnew = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playercontrolnew.enabled = false;

            //Load new Scene save
            savewrapper.Load();

            //Get Portal location and set player position to it
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            savewrapper.Save();

            //Wait for playe reposition to settle and then fade into the game.
            yield return new WaitForSeconds(m_FadeWaitTime);
            fader.FadeIn(m_FadeInTime);

            // Enable the player controller
            playercontrolnew.enabled = true;

            //Now allow the object to be destroyed.
            Destroy(gameObject);
        }

        //Finds the portal with the destinationID that matches the current portals
        private Portal GetOtherPortal()
        {
           foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.m_DestinationID != m_DestinationID) continue;
              
                return portal;                
            }
            return null;
        }

        //Moves player to the spawn point for the portal.
        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.m_SpawnPoint.transform.position;
            player.transform.rotation = otherPortal.m_SpawnPoint.transform.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}

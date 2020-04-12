using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using System;
using GameDevTV.Utils;
using RPG.Saving;

namespace RPG.SceneManagement {
    public class SpawnPoint : MonoBehaviour, ISaveable
    {
        //Used for representing where a spawnpoint center is
        [SerializeField] bool m_RespawnFlag = false;
        [SerializeField] float m_RespawnTime = 1500.0f;

        [SerializeField] float m_WaypointDrawRadius = .2f;
        [SerializeField] float m_SpawnRadius = 2f;
        [SerializeField] int m_NumberToSpawn = 3;
        [SerializeField] AIController m_AIEnemy;

        LazyValue<bool> m_bHasSpawned;

        //Require so that the spawnflag's state can be saved to prevent all spawns from having their flags reset on load
        private void Awake()
        {
            m_bHasSpawned = new LazyValue<bool>(GetInitialSpawnFlag);
        }

        private bool GetInitialSpawnFlag()
        {
            return false;
        }

        //Check to see if the player has entered the collider and that it has not already spawned.
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player" || m_bHasSpawned.value) return;
            SpawnEnemies();
            StartCoroutine(DisableForSeconds(m_RespawnTime));
        }

        //Spawn Enemies
        private void SpawnEnemies()
        {
            for (int i = 0; i< m_NumberToSpawn; i++)
            {
                Vector3 placeposition = SpawnLocation();
                AIController enemy = Instantiate(m_AIEnemy, placeposition, Quaternion.identity);
            }
        }

        //Creates a random spawn location for the enemy within the spawn radius (Not the collider radius)
        private Vector3 SpawnLocation()
        {
            //Compute random x location and then compute what the z is.
            float xoffset = UnityEngine.Random.Range(-m_SpawnRadius, m_SpawnRadius);
            float zoffset = Mathf.Sqrt((Mathf.Pow(m_SpawnRadius, 2) - Mathf.Pow(xoffset, 2)));

            //determine if z will positive or negative
            if (UnityEngine.Random.Range(-1, 1) < 0)
            {
                zoffset *= -1;
            }

            Vector3 spawnPos = GetComponent<Transform>().position;
            Vector3 placeposition = new Vector3(spawnPos.x + xoffset, spawnPos.y, spawnPos.z + zoffset);
            return placeposition;
        }

        //Prevent the spawner from spawning new enemies for a period of time
        private IEnumerator DisableForSeconds(float time)
        {
            m_bHasSpawned.value = true;
            yield return new WaitForSeconds(time);
            if (m_RespawnFlag) { m_bHasSpawned.value = false;}
            DespawnEnemies();
        }

        private void DespawnEnemies()
        {
            throw new NotImplementedException();
        }

        public object CaptureState()
        {
            return m_bHasSpawned.value;
        }

        public void RestoreState(object state)
        {
            //Get spawn flag
            m_bHasSpawned.value = (bool)state;
        }

        //Called by unity. Draws our the patrol path.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, m_WaypointDrawRadius);
        }
    }
}

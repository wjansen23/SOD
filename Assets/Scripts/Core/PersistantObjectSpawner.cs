using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    //Class which handles making sure object that are desired to be persisted across scenes are.
    public class PersistantObjectSpawner : MonoBehaviour
    {
        //Stores what should be persited.  Should be a Prefab that includes all objects that should
        //persist.
        [SerializeField] GameObject m_PersistentObjectPrefab;

        //Used to track whether the GameObject has spawned so we don't spawn it mulitiple times.
        static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned) return;

            SpawnPersistentObject();
            hasSpawned = true;
        }

        private void SpawnPersistentObject()
        {
            //Instantiate pre-fab and don't destroy
            GameObject persistentObject = Instantiate(m_PersistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}

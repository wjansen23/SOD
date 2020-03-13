using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSpawnPoint : MonoBehaviour
{
    //Used for representing where a spawn point
    [SerializeField] float m_SpawnPointDrawRadius = .2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called by Unity.  For displaying an spawn point.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_SpawnPointDrawRadius);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    //This class is used for creating patrol paths for AI's to walk.
    public class PatrolPath : MonoBehaviour
    {
        //Used for representing where a waypoint is on a patrol path
        [SerializeField] float m_WaypointDrawRadius = .2f;

        //Called by unity. Draws our the patrol path.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int i=0; i<transform.childCount; i++)
            {
                Gizmos.DrawSphere(GetWaypoint(i), m_WaypointDrawRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextWaypointIndex(i)));              
            }
        }

        //Get the next waypoint for a patrol path
        public int GetNextWaypointIndex(int i)
        {
            if (i +1 >= transform.childCount)
            {
                return 0;
            }else
            {
                return i+1;
            }
            
        }

        //Return position of specified waypoint index.
        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).transform.position;
        }
    }
}

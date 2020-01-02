using UnityEngine;
using RPG.Combat;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    //This class is responsible for controll AI behavoir in the game.
    //TODO: See if there is a better way to deal with "Endless Aggroing" within mobs

    public class AIController : MonoBehaviour
    {
        //These fields are for PATROL, CHASE and LOOK AROUND behavoirs
        [SerializeField] float m_chaseDistance = 5f;
        [SerializeField] float m_SuspicionTime = 3f;  //MAYBE RENAME
        [SerializeField] float m_AggroCooldownTime = 5f;
        [SerializeField] float m_WaypointTolerance = 1f;
        [SerializeField] float m_WaypointWaitTime = 2f;
        [SerializeField] float m_PatrolSpeedModifier = .5f;
        [SerializeField] float m_AlertDistance = 5f;
        [SerializeField] PatrolPath m_PatrolPath=null;

        static string PLAYER_TAG = "Player";

        //Used to control PATROL, CHASE and LOOK AROUND behavoirs
        bool m_AlreadyAggro = false;
        float m_TimeSinceLastSawPlayer = Mathf.Infinity;
        float m_TimeSinceWaypointReached = Mathf.Infinity;
        float m_TimeSinceAggrevated = Mathf.Infinity;
        float m_BaseSpeed;
        int m_CurrentWaypointIndex=0;

        CharacterCombat m_characterCombat;
        GameObject m_TargetPlayer;
        Health m_Health;
        Mover m_Mover;

        LazyValue<Vector3> m_guardPosition;

        //Grab component references here
        private void Awake()
        {
            m_characterCombat = GetComponent<CharacterCombat>();
            m_Health = GetComponent<Health>();
            m_Mover = GetComponent<Mover>();
            m_TargetPlayer = GameObject.FindWithTag(PLAYER_TAG);
            m_guardPosition = new LazyValue<Vector3>(SetInitialGuardPosition);
        }

        //Set base position for AI within the scene
        private Vector3 SetInitialGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            m_guardPosition.ForceInit();
        }

        private void Update()
        {
            //Check to see if dead
            if (m_Health.IsDead()) return;

            //AI Logic
            //Check if AI should chase player and attack
            //Check is AI should stop chasing and enter suspicion state
            //Check if AI should return to its guard/orignal starting point
            if (IsAggrevated() && m_characterCombat.CanAttack(m_TargetPlayer))
            {
                    AttackBehavoir();
            }
            else if (m_TimeSinceLastSawPlayer<m_SuspicionTime)
            {
                SuspicionBehavoir();
            }
            else
            {
                //reset aggro flag here to prevent endless chasing due to mobs continiously invoking aggro on each other
                m_AlreadyAggro = false;
                PatrolBehavoir();
            }
            UpdateTimers();
        }

        //Update all the behavoir timers
        private void UpdateTimers()
        {
            m_TimeSinceLastSawPlayer += Time.deltaTime;
            m_TimeSinceWaypointReached += Time.deltaTime;
            m_TimeSinceAggrevated += Time.deltaTime;
        }

        //Used to enable AI's to patrol a set of points within a game.
        private void PatrolBehavoir()
        {
           //Set next location to guard position. For instances when no patrol route is associated
           //with the AI
            Vector3 nextLocation = m_guardPosition.value;

            //Check to see if a patrol route has been assigned. If not will stand/move to
            //guard position
            if (m_PatrolPath != null)
            {
                if (AtWaypoint())
                {
                    //If at next waypoint.  Then wait for a specified time before move to 
                    //next waypoint
                    m_TimeSinceWaypointReached = 0;
                    CycleNextWayPoint();
                }       

                //Set next waypoint location
                nextLocation = m_PatrolPath.GetWaypoint(m_CurrentWaypointIndex);
            }

            if (m_TimeSinceWaypointReached > m_WaypointWaitTime)
            {
                //Move to next loation
                m_Mover.StartMoveAction(nextLocation, m_PatrolSpeedModifier);
            }
        }

        //Is the AI within an acceptible distance from the waypoint location
        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, m_PatrolPath.GetWaypoint(m_CurrentWaypointIndex));
            return distanceToWaypoint <= m_WaypointTolerance;
        }

        private void CycleNextWayPoint()
        {
            m_CurrentWaypointIndex = m_PatrolPath.GetNextWaypointIndex(m_CurrentWaypointIndex);
        }
       
        //Logic for the AI to be suspicious
        private void SuspicionBehavoir()
        {
            //Cancel current actions and have the AI wait at its current position
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        //Logic for AI to guard 
        //NOT CURRENTLY USED
        private void GuardBehavoir()
        {
            //Move AI to its static guard position
            m_Mover.StartMoveAction(m_guardPosition.value,1f);
        }

        //Logic for chasing the player
        void AttackBehavoir()
        {
            m_TimeSinceLastSawPlayer = 0;
            m_characterCombat.Attack(m_TargetPlayer.gameObject);
            AggrevateNearbyEnemies();
        }

        //Cause all nearby enemies in a radius to aggro as well
        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_AlertDistance, Vector3.up, 0);

            foreach(RaycastHit hit in hits)
            {
                AIController ai = hit.transform.GetComponent<AIController>();

                //Don't try to aggro a null
                if (ai == null) continue;

                //Don't aggro yourself
                if (ai == GetComponent<AIController>()) continue;               

                ai.Aggrevate();
            }
        }

        //Aggro on to the player
        public void Aggrevate()
        {
            //If already in aggro to not reset the timer.  This is to prevent the timer from endlessly resetting in a mob
            if (!m_AlreadyAggro)
            {
                m_TimeSinceAggrevated = 0;
                m_AlreadyAggro = true;
            }
        }

        //Is the AI aggrivated or the player within the chase radius
        private bool IsAggrevated()
        {
            if (Vector3.Distance(transform.position, m_TargetPlayer.transform.position) <= m_chaseDistance) return true;
            if (m_TimeSinceAggrevated <= m_AggroCooldownTime) return true;

            return false;
        }

        //Called by Unity.  For displaying an AI's chase radius.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;            
            Gizmos.DrawWireSphere(transform.position, m_chaseDistance);
        }

        //Respond to a player attack
        public void WasAttacked()
        {
            m_TimeSinceAggrevated = 0;
            AggrevateNearbyEnemies();
        }
    }
}

using UnityEngine;
using RPG.UI.SpeechBubble;
using RPG.Movement;
using RPG.Core;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{ 
    public class NPCController : MonoBehaviour
    {
        //This class is for a non-combat Characters that the player can intereact with.
        //This includes shopkeepers, quest givers and so forth.  They cannot engage in combat.
        //It is based off the AIController but not a direct inheritence.

        //These fields are for PATROL behavoirs
        [SerializeField] float m_WaypointTolerance = 1f;
        [SerializeField] float m_WaypointWaitTime = 2f;
        [SerializeField] float m_PatrolSpeedModifier = .5f;
        [SerializeField] PatrolPath m_PatrolPath = null;

        [SerializeField] SpeechBubbleText m_SpeechBubble = null;

        //These fields are for INTERACTION behavoirs
        //When will the character move to interact with the player
        [SerializeField] float m_InteractDistance = 5f;

        //What distance will the interaction actually begin
        [SerializeField] float m_StartInteractionDistance= 1f;

        //How long to wait to see if the player is within interaction distance
        [SerializeField] float m_InteractWaitTime = 5f;

        //How long the interact takes
        [SerializeField] float m_DebugInteractionTime=3f;

        static string PLAYER_TAG = "Player";

        //Used to control PATROL behavoirs
        float m_TimeSinceWaypointReached = Mathf.Infinity;
        float m_BaseSpeed;
        int m_CurrentWaypointIndex = 0;

        //Used to control INTERACTION Behavoirs
        float m_TimeSinceInteracted = Mathf.Infinity;
        float m_TimeSinceLastSawPlayer = Mathf.Infinity;
        bool m_IntereactionComplete = false;
        bool m_InteractingWith = false;

        GameObject m_TargetPlayer;
        Mover m_Mover;

        LazyValue<Vector3> m_guardPosition;

        //Grab component references here
        private void Awake()
        {
            m_Mover = GetComponent<Mover>();
            m_TargetPlayer = GameObject.FindWithTag(PLAYER_TAG);
            m_guardPosition = new LazyValue<Vector3>(SetInitialGuardPosition);

            m_SpeechBubble.MakeSpeechBubbleVisible(false);
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
            if (CanInteract())
            {
                InteractBehavoir();
            }
            else if (m_TimeSinceLastSawPlayer <= m_InteractWaitTime)
            {
                WaitBehavoir();
            }
            else
            {
                PatrolBehavoir();
            }
            UpdateTimers();
        }

        private void WaitBehavoir()
        {
            //Cancel current actions and have the AI wait at its current position
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        //Handles the NPCs interaction behavoir with the player
        private void InteractBehavoir()
        {
            //Move closer to the player
            if (GetDistanceToPlayer()>m_StartInteractionDistance)
            {
                GetComponent<Mover>().MoveTo(m_TargetPlayer.transform.position,.5f);
            }
            else
            {
                if (!m_InteractingWith)
                {
                    DoInteraction();
                }
            }
        }

        //Cause a speech bubble to appear
        private void DoInteraction()
        {
            m_InteractingWith = true;
            m_TimeSinceInteracted = 0;

            //Cancel current actions and have the AI wait at its current position
            GetComponent<ActionScheduler>().CancelCurrentAction();
            m_SpeechBubble.MakeSpeechBubbleVisible(true);
        }

        //Update all the behavoir timers
        private void UpdateTimers()
        {
            m_TimeSinceWaypointReached += Time.deltaTime;
            m_TimeSinceInteracted += Time.deltaTime;
            m_TimeSinceLastSawPlayer += Time.deltaTime;
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

        //Logic for AI to guard 
        //NOT CURRENTLY USED
        private void GuardBehavoir()
        {
            //Move AI to its static guard position
            m_Mover.StartMoveAction(m_guardPosition.value, 1f);
        }
        
        //Called by Unity.  For displaying charatcer interaction radius.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, m_InteractDistance);
        }

        //Checks to see if the player is within the characters interaction radius
        private bool CanInteract()
        {
            if (GetDistanceToPlayer() > m_InteractDistance) return false;
            if (m_InteractingWith)
            {
                if (m_TimeSinceInteracted > m_DebugInteractionTime)
                {
                    m_SpeechBubble.MakeSpeechBubbleVisible(false);
                    return false;
                }
            }
            return true;
        }
        
        private float GetDistanceToPlayer()
        {
            return Vector3.Distance(transform.position, m_TargetPlayer.transform.position);
        }
    }
}


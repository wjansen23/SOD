using UnityEngine;
using UnityEngine.AI;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    //Handles all movement bahavoir for characters
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform m_TargetTransform;  //NOT USED
        [SerializeField] float m_MaxSpeed = 12f;
        [SerializeField] float m_BaseSpeed = 6f;
        [SerializeField] float m_MaxNavPathDistance = 40f;

        Health m_Health;
        NavMeshAgent m_NavMeshAgent;

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Health = GetComponent<Health>();
        }

        private void Start()
        {
            m_NavMeshAgent.speed = m_BaseSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            m_NavMeshAgent.enabled = !m_Health.IsDead();
            UpdateAnimator();
        }

        //Cancel movement actions
        public void Cancel()
        { 
            m_NavMeshAgent.isStopped = true;
        }

        public void StartMoveAction(Vector3 destination, float speedMod)
        {
            //Add to action scheduler
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination,speedMod);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();

            //Check to see if it has a valid path or a complete path, if not return false
            if (!NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path)) return false;

            if (path.status != NavMeshPathStatus.PathComplete) return false;

            //Check to see if the found path is longer then allowed
            if (GetPathLength(path) > m_MaxNavPathDistance) return false;

            return true;
        }

        //Move to sent location
        public void MoveTo(Vector3 destination, float speedMod)
        {
            m_NavMeshAgent.SetDestination(destination);
            m_NavMeshAgent.speed = Mathf.Clamp(m_BaseSpeed * speedMod,0,m_MaxSpeed);
            m_NavMeshAgent.isStopped = false;
        }

        //Used for translating movement velocities from NAV MESH to ANIMATOR
        private void UpdateAnimator()
        {
            Vector3 velocity = m_NavMeshAgent.velocity;

            //Transform from Global to local
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            //Update Animator movement speed setting
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            m_NavMeshAgent.enabled = false;
            transform.position = position.ToVector();
            m_NavMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private float GetPathLength(NavMeshPath path)
        {
            float pathdistance = 0;
            if (path.corners.Length < 2) return pathdistance;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                pathdistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return pathdistance;
        }
    }
}

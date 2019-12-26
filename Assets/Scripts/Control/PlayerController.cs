using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using RPG.Movement;
using RPG.Attributes;
using System;


namespace RPG.Control
{
    //This class executes player actions.
    public class PlayerController : MonoBehaviour
    {
        Health m_Health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] m_CursorMappings = null;
        [SerializeField] float m_MaxNavMeshProjectionDistance = 1f;
        [SerializeField] float m_RayCastRadius = 1f;


        void Awake()
        {
            m_Health = GetComponent<Health>();
        }

        // Update is called once per frame.  Check what the cursor is pointing to and react appropriately.
        void Update()
        {
            if (InteractWithUI()) return;
            if (m_Health.IsDead())
            {
                SetCurosor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCurosor(CursorType.None);
        }

        //Checks to see if the mouse is over a UI element
        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCurosor(CursorType.UI);
                return true;
            }
            return false;
        }

        //Checks to see if the mouse is over a gameobject with the IRaycastable interface implemented
        private bool InteractWithComponent()
        {
            //Get list of raycast hits.
            RaycastHit[] raycastHits = RaycastAllSorted();

            foreach (RaycastHit hit in raycastHits)
            {
                IRaycastable[] castablearray = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in castablearray)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCurosor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        //Sorts the raycast hits by their distance from the player.
        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), m_RayCastRadius);
            float[] distances = new float[hits.Length];

            for (int i = 0; i<hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        //Handles determining whether a player should move to point clicked
        private bool InteractWithMovement()
        {
            //RaycastHit raycastHit;

            ////See if Ray hit something
            //bool hasHit = Physics.Raycast(GetMouseRay(), out raycastHit);
            Vector3 target;

            //Raycast to the nav mesh
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target,1f);
                }
                SetCurosor(CursorType.Movement);
                return true;
            }
            return false;
        }

        //Handles raycasts to the nav mesh to see if it hit an nav mesh enabled area. 
        private bool RaycastNavMesh(out Vector3 target)
        {
            RaycastHit raycastHit;
            NavMeshHit meshhit;
            target = new Vector3();

            //See if Ray hit something
            bool hasHit = Physics.Raycast(GetMouseRay(), out raycastHit);
            if (!hasHit) return false;

            //See if it hit a navmesh enabled position
            if (NavMesh.SamplePosition(raycastHit.point, out meshhit, m_MaxNavMeshProjectionDistance, NavMesh.AllAreas))
            {                               
                target = meshhit.position;
                return true;
            }            
            return false;
        }

        //Get ray to point on screen mouse is point to
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCurosor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in m_CursorMappings)
            {
                if (mapping.type == type) return mapping;
            }
            return m_CursorMappings[0];
        }
    }
}

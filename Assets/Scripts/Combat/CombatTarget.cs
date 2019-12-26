using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    //Empty class used by PLAYER CONTROLLER to determine if an object can be attacked by the player.
    //Placing on the player will enable the player to targt and attack themselves

    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController callingController)
        {
            //Check to see if the target can be attacked.
            if (callingController.GetComponent<CharacterCombat>().CanAttack(gameObject))
            {
                //Process mouse button down action
                if (Input.GetMouseButton(0))
                {
                    callingController.GetComponent<CharacterCombat>().Attack(gameObject);
                }
                return true;
            }
            return false;
        }


        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}

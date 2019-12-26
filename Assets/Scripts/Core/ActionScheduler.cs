using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{

    //This class stores the current action a player or AI character is doing
    public class ActionScheduler : MonoBehaviour
    {
        IAction m_CurrentAction;

        public void StartAction(IAction action)
        {
            //If action is already set then return
            if (m_CurrentAction == action) return;

            //If no action then do not try to cancel
            if (m_CurrentAction != null)
            {
                m_CurrentAction.Cancel();
            }

            m_CurrentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    //Interface for utilizing the Action Scheduler so behavoirs can be canceled or stored.
    public interface IAction
    {
        void Cancel();

    }
}

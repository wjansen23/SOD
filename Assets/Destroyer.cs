using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{

    [SerializeField] GameObject m_TargetToDestory = null;

    public void DestroyTarget()
    {
        Destroy(m_TargetToDestory);
    }
}

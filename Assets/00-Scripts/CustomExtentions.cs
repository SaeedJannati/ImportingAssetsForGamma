using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class CustomExtentions
{
    #region NavmeshAgent
    public static bool ReachedDestinationOrGaveUp(this NavMeshAgent navMeshAgent)
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude < 0.001f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

}
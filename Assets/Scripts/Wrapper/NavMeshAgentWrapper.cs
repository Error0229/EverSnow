using UnityEngine;
using UnityEngine.AI;

namespace Wrapper
{
    public class NavMeshAgentWrapper
    {
        private NavMeshAgent navMeshAgent;

        public NavMeshAgentWrapper( NavMeshAgent navMeshAgent)
        {
            this.navMeshAgent = navMeshAgent;
        }

        public void SetDestination( Vector3 destination)
        {
            navMeshAgent.SetDestination(destination);
        }
    }
}
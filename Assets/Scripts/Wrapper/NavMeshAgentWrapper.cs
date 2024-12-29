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
        public bool Visible(GameObject target)
        {
            RaycastHit hit;
            var direction = target.transform.position - navMeshAgent.transform.position;
            if (Physics.Raycast(navMeshAgent.transform.position, direction, out hit))
            {
                if (hit.collider.gameObject == target)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
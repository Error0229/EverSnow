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
        // public bool IsStopped()
        // {
        //     return navMeshAgent.isStopped;
        // }
        public bool IsArrived()
        {
            if (navMeshAgent)
                return !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
            else return false;
        }
        public enum MoveSpeed { Walk, Run, Sprint }
        public void SetSpeed( float speed)
        {
            navMeshAgent.speed = speed;
        }
        public void SetSpeed( MoveSpeed speed)
        {
            switch (speed)
            {
                case MoveSpeed.Walk:
                    navMeshAgent.speed = 2;
                    break;
                case MoveSpeed.Run:
                    navMeshAgent.speed = 5;
                    break;
                case MoveSpeed.Sprint:
                    navMeshAgent.speed = 7;
                    break;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.AI3.Demo
{
    public class NavAgentMoveDemo : MonoBehaviour
    {
        [Header("Point to Point")]
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;

        [Header("Chase")]
        [SerializeField] private bool chase;
        [SerializeField] private Transform chaseTarget;

        private NavMeshAgent navMeshAgent;
        private Transform targetPoint;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();

            targetPoint = pointA;
        }

        void Update()
        {
            if (chase)
            {
                Vector3 groundPosition = new Vector3(chaseTarget.position.x, 0, chaseTarget.position.z);
                navMeshAgent.SetDestination(groundPosition);
            }
            else
            {
                Vector3 groundPosition = new Vector3(transform.position.x, 0, transform.position.z);
                float distance = Vector3.Distance(groundPosition, targetPoint.position);
                if (distance > 0.2f)
                {
                    if (!navMeshAgent.pathPending && !navMeshAgent.hasPath)
                        navMeshAgent.SetDestination(targetPoint.position);
                }
                else
                {
                    targetPoint = targetPoint == pointA ? pointB : pointA;
                }
            }
        }
    }
}
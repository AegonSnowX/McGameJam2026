using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float fleeDistance = 5f;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        Vector3 fleeDir = (transform.position - target.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDir * fleeDistance;

        agent.SetDestination(fleeTarget);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] GenerateStage generateStage;
    [SerializeField] Transform target;

    Vector3 offsetStartPos = new Vector3(0, 0.5f, 0);
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChangePos(generateStage.GetGoalPos() + offsetStartPos);

    }

    void Update()
    {
        agent.SetDestination(target.transform.position);
    }

    void ChangePos(Vector3 pos)
    {
        agent.enabled = false;
        transform.position = pos;
        agent.enabled = true;
    }
}

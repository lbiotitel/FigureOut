using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShopNPCRouteNav : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int currentWaypointIndex = 0;
    [SerializeField] private NavMeshAgent agent;

    [Header("UI에 따른 움직임 제어")]
    [SerializeField] private ShowUI showUI;

    private void Awake()
    {
        showUI = gameObject.GetComponent<ShowUI>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if(waypoints.Length > 0)
        {
            SetNextDestination();
        }
    }

    private void Update()
    {
        // 목적지에 도착해가는지 확인
        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNextDestination();
        }
    }

    private void SetNextDestination()
    {
        if (waypoints.Length == 0) return;

        // 현재 목적지 설정
        agent.destination = waypoints[currentWaypointIndex].position;

        // 다음 목적지 인덱스로 변경 (마지막 지점이면 다시 0번으로 감)
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    private void ShopNPCStop(bool isEnabled)
    {
        agent.isStopped = isEnabled;
        Debug.Log("상점 UI 활성화 감지!");
        if (isEnabled) agent.velocity = Vector3.zero;
    }

    private void OnEnable()
    {
        if (showUI != null)
            showUI.OnUIEnabled += ShopNPCStop;
    }

    private void OnDisable()
    {
        if (showUI != null)
            showUI.OnUIEnabled -= ShopNPCStop;
    }
}

using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class Animal : NetworkBehaviour
{
    [SerializeField] private int MaxHp;
    [SerializeField] private GameObject SpawnMeat;
    [SerializeField] private NavMeshAgent agent;

    [Networked] public int Hp { get; set; }

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            Hp = MaxHp;
            
            agent = GetComponent<NavMeshAgent>();
            agent.speed = normalSpeed; // 초기 이동 속도 설정
            centerPoint = transform.position; // 현재 위치를 중심점으로 설정
            StartCoroutine(WanderAndRest());
        }
    }

    public void Hit(int damage)
    {
        if (HasStateAuthority)
        {
            OnAttacked();

            Hp -= damage;

            if (Hp <= 0)
            {
                // 죽
                var spawnedMeat = Global.Instance.MyPlayer.Runner.Spawn(SpawnMeat, this.transform.position + Vector3.up * 1f, Quaternion.identity, Object.StateAuthority);
                Runner.Despawn(this.GetComponent<NetworkObject>());
            }
        }
    }
    
    //
    private float wanderRadius = 10f; // 랜덤 이동 반경
    private float newMove = 5f; // 랜덤 이동 시간
    private float restDuration = 2f; // 쉬는 시간
    private float fleeRadius = 15f; // 도망가는 반경
    private float fleeDuration = 3f; // 도망 후 대기 시간
    private float movementRadius = 20f; // 캐릭터가 이동할 수 있는 제한된 반경
    private Vector3 centerPoint; // 이동 제한의 중심점
    
    private float moveTime; // 움직인시간

    public float normalSpeed = 3.5f; // 일반 이동 속도
    public float fleeSpeed = 7f; // 도망갈 때 속도
    
    private bool isResting = false;
    private bool isFleeing = false;

    // 공격받을 때 호출하는 함수
    public void OnAttacked()
    {
        if (!isFleeing) // 이미 도망 중이 아닐 때만 도망 감행
        {
            StartCoroutine(RandomFlee());
        }
    }

    // 이동과 휴식을 번갈아가며 실행하는 코루틴
    IEnumerator WanderAndRest()
    {
        while (true)
        {
            // 도망 중이 아니라면 랜덤 이동 및 쉬기
            if (!isFleeing)
            {
                if (!isResting)
                {
                    moveTime = 0f;
                    Vector3 newDestination = RandomNavSphereWithinRadius(centerPoint, wanderRadius, movementRadius);
                    agent.SetDestination(newDestination);

                    // 목표 지점에 도달할 때까지 기다림
                    while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance || newMove < moveTime)
                    {
                        moveTime += Time.deltaTime;
                        yield return null;
                    }

                    // 쉬는 상태로 전환
                    isResting = true;
                    yield return new WaitForSeconds(restDuration); // 정해진 시간 동안 쉼
                    isResting = false; // 다시 이동 상태로 전환
                }
            }
            yield return null;
        }
    }

    // 공격받았을 때 무작위로 도망가는 코루틴
    IEnumerator RandomFlee()
    {
        isFleeing = true;
        agent.speed = fleeSpeed; // 도망 속도로 변경

        // 도망갈 무작위 위치 설정
        Vector3 fleeDestination = RandomNavSphereWithinRadius(centerPoint, fleeRadius, movementRadius);
        agent.SetDestination(fleeDestination);

        // 도망 목적지에 도착할 때까지 기다림
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // 도망 후 쉬는 시간
        yield return new WaitForSeconds(fleeDuration);

        agent.speed = normalSpeed; // 다시 일반 속도로 변경
        isFleeing = false; // 도망 상태 해제
    }

    // 제한된 반경 내에서 무작위 위치를 찾는 함수
    public static Vector3 RandomNavSphereWithinRadius(Vector3 center, float distance, float maxRadius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance; // 무작위 방향 설정
        randomDirection += center; // 중심점을 기준으로 방향 이동

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, distance, -1))
        {
            // 해당 위치가 제한된 반경 내에 있는지 확인
            if (Vector3.Distance(center, navHit.position) <= maxRadius)
            {
                return navHit.position; // 제한된 반경 내라면 그 위치 반환
            }
        }

        // 유효한 위치를 찾지 못하거나 반경을 벗어날 경우, 중심점 반환
        return center;
    }
}

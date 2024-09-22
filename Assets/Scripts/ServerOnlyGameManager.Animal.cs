using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public partial class ServerOnlyGameManager
{
    public NetworkObject[] objectsToSpawn; // 3개의 소환할 오브젝트 배열
    public int[] spawnLimits; // 각각의 오브젝트에 대한 최대 소환 수
    public float spawnRadius = 20f; // 소환 반경
    public Transform centerPoint; // 소환 중심점
    public int maxAttempts = 30; // 위치를 찾기 위한 최대 시도 횟수

    private List<NetworkObject> activeObjects = new List<NetworkObject>(); // 활성화된 오브젝트 리스트
    
    // 처음에 지정된 수만큼 오브젝트들을 소환
    IEnumerator InitialSpawn()
    {
        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            for (int j = 0; j < spawnLimits[i]; j++)
            {
                SpawnObject(i);
                yield return new WaitForSeconds(0.1f); // 짧은 대기 후 소환
            }
        }
        // 오브젝트 상태를 지속적으로 확인하는 코루틴 시작
        StartCoroutine(CheckAndRespawn());
    }

    // 오브젝트를 랜덤 위치에 소환하는 함수
    void SpawnObject(int objectIndex)
    {
        Vector3 spawnPosition = RandomNavSphereWithinRadius(centerPoint.position, spawnRadius, maxAttempts);

        if (spawnPosition != Vector3.zero)
        {
            var newObject = Runner.Spawn(objectsToSpawn[objectIndex], spawnPosition, Quaternion.identity, Object.StateAuthority);
            activeObjects.Add(newObject);
        }
        else
        {
            Debug.LogWarning("유효한 위치를 찾지 못했습니다.");
        }
    }

    // 제한된 반경 내에서 NavMesh 위의 무작위 위치를 찾는 함수
    public static Vector3 RandomNavSphereWithinRadius(Vector3 center, float distance, int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * distance;
            randomDirection += center;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, distance, -1))
            {
                if (Vector3.Distance(center, navHit.position) <= distance)
                {
                    return navHit.position;
                }
            }
        }

        return Vector3.zero;
    }

    // 오브젝트 상태를 확인하고, 부족할 경우 재소환하는 코루틴
    IEnumerator CheckAndRespawn()
    {
        while (true)
        {
            for (int i = 0; i < objectsToSpawn.Length; i++)
            {
                int currentCount = CountActiveObjects(i);
                if (currentCount < spawnLimits[i])
                {
                    int objectsToRespawn = spawnLimits[i] - currentCount;

                    for (int j = 0; j < objectsToRespawn; j++)
                    {
                        SpawnObject(i);
                    }
                }
            }

            yield return new WaitForSeconds(5f); // 5초마다 상태 확인
        }
    }

    // 특정 오브젝트의 활성화된 수를 카운트하는 함수
    int CountActiveObjects(int objectIndex)
    {
        activeObjects.RemoveAll(item => item == null); // 비활성화된 오브젝트 리스트에서 제거
        int count = 0;
        
        foreach (NetworkObject obj in activeObjects)
        {
            try
            {
                if (obj != null && obj.name.Contains(objectsToSpawn[objectIndex].name))
                {
                    count++;
                }
            }
            catch (Exception e)
            {
                // Missing = 죽음
                count++;
            }
        }

        return count;
    }
}

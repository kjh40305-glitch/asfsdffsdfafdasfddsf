using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("스폰할 프리팹")]
    [SerializeField] private GameObject prefabToSpawn;

    [Header("X축 좌표 범위")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("Y축 좌표 범위")]
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [Header("고정 Z축 깊이")]
    [SerializeField] private float spawnZ = 0f;

    [Header("스폰 제한 및 주기")]
    [SerializeField] private int maxCount = 5;
    [SerializeField] private bool autoSpawn = true;
    [SerializeField] private float spawnInterval;

    [Header("겹침 감지 설정 (2D)")]
    [Tooltip("장애물(벽 등)과 떨어져야 하는 최소 반경")]
    [SerializeField] private float checkRadius = 1.0f;
    [Tooltip("스폰된 개체들끼리 떨어져야 하는 최소 거리")]
    [SerializeField] private float minDistanceBetween = 1.5f;
    [Tooltip("충돌 검사 대상 레이어 (장애물, 벽 등)")]
    [SerializeField] private LayerMask obstacleLayer;
    [Tooltip("빈 자리를 찾기 위한 최대 시도 횟수")]
    [SerializeField] private int maxSpawnAttempts = 20;

    private List<GameObject> activeObjects = new List<GameObject>();
    private float timer;

    void Update()
    {
        activeObjects.RemoveAll(item => item == null);

        if (!autoSpawn) return;

        if (activeObjects.Count < maxCount)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                Spawn();
                timer = 0f;
            }
        }
    }

    public void Spawn()
    {
        if (prefabToSpawn == null) return;

        activeObjects.RemoveAll(item => item == null);
        if (activeObjects.Count >= maxCount) return;

        if (TryGetValidSpawnPosition(out Vector3 spawnPosition))
        {
            GameObject newObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            activeObjects.Add(newObj);
        }
        else
        {
            Debug.LogWarning("스폰 가능한 빈 공간을 찾지 못했습니다.");
        }
    }

    private bool TryGetValidSpawnPosition(out Vector3 validPosition)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector3 testPosition = new Vector3(randomX, randomY, spawnZ);

            // 1. 이미 스폰되어 필드에 있는 오브젝트들과의 거리 검사
            bool tooCloseToSpawned = false;
            foreach (var obj in activeObjects)
            {
                if (obj != null && Vector2.Distance(testPosition, obj.transform.position) < minDistanceBetween)
                {
                    tooCloseToSpawned = true;
                    break;
                }
            }

            if (tooCloseToSpawned) continue;

            // 2. 맵 환경(장애물 레이어)과의 물리 겹침 검사
            Collider2D hitCollider = Physics2D.OverlapCircle(testPosition, checkRadius, obstacleLayer);
            if (hitCollider == null)
            {
                validPosition = testPosition;
                return true;
            }
        }

        validPosition = Vector3.zero;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, spawnZ);
        Vector3 size = new Vector3(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY), 0.1f);

        Gizmos.DrawWireCube(center, size);
    }
}
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("둥실거림 설정")]
    [Tooltip("위아래로 움직이는 속도")]
    [SerializeField] private float floatSpeed = 2f;

    [Tooltip("위아래로 움직이는 폭(거리)")]
    [SerializeField] private float floatAmplitude = 0.25f;

    [Header("회전 설정 (선택 사항)")]
    [SerializeField] private bool useRotation = false;
    [SerializeField] private float rotateSpeed = 90f;

    private Vector3 startPosition;

    void Start()
    {
        // 스폰된 초기 위치를 기준점으로 저장
        startPosition = transform.position;
    }

    void Update()
    {
        // 1. 위아래 부유 운동 (Sin 함수 활용)
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // 2. 부드러운 회전 효과 (2D는 Z축, 3D는 Y축 회전)
        if (useRotation)
        {
            transform.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime));
        }
    }
}
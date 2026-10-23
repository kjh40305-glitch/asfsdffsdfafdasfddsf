using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private float elapsedTime = 0f;

    // 점수 관련 변수
    private float currentScore = 0f;      // 프레임 단위로 부드럽게 누적할 실수형 점수
    public float score = 0f;              // 정수형으로 변환된 최종 점수
    public float scoreMultiplier = 10f;   // 기본 점수 속도 (초당 점수)
    public float bonusPerItem = 0.2f;     // 아이템 1개당 속도 증가량 (0.2 = 20%씩 증가)

    public float thrustForce = 1f;
    Rigidbody2D rb;

    public UIDocument uiDocument;

    Label scoreText;
    Button restartButton;

    public GameObject explosionEffect;

    public float maxSpeed = 5f;
    public float RealmaxSpeed = 10f;
    public int fastItemCount;
    public GameObject boost;
    public GameObject border;

    public AudioSource audio;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // 아이템 개수를 반영한 현재 점수 배율 계산 (기본 10 -> 아이템 1개당 +2씩 가속)
        float currentMultiplier = scoreMultiplier * (1f + fastItemCount * bonusPerItem);

        // 이전 프레임에서 지난 시간만큼만 배율을 곱해 누적 (점수가 튀는 현상 방지)
        currentScore += Time.deltaTime * currentMultiplier;
        score = Mathf.FloorToInt(currentScore);

        scoreText.text = "Score : " + score;

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);

            Vector2 direction = (mousePos - transform.position).normalized;
            transform.up = direction;

            rb.AddForce(direction * thrustForce);

            if(maxSpeed > RealmaxSpeed)
            {
                maxSpeed = RealmaxSpeed;
            }

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boost.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boost.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 씬에 남아있는 모든 Item 태그 오브젝트 일괄 삭제
        GameObject[] remainingItems = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in remainingItems)
        {
            Destroy(item);
        }

        // 2. 추가 스폰 방지 (씬 내의 모든 Spawner 비활성화)
        Spawner[] spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        foreach (Spawner spawner in spawners)
        {
            spawner.enabled = false;
        }

        // 3. 이펙트 생성 및 UI 활성화
        Instantiate(explosionEffect, transform.position, transform.rotation);
        border.SetActive(false);
        restartButton.style.display = DisplayStyle.Flex;

        // 4. 플레이어 오브젝트 파괴
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log("Get the Item");
            audio.Play();
            Destroy(collision.gameObject);

            fastItemCount++;
            maxSpeed++; // 최대 속도 증가와 함께 점수 획득 속도도 함께 증가
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    float minSize = 0.08f;
    float maxSize = 0.2f;
    float minSpeed = 60f;
    float maxSpeed = 110f;
    public float maxSpinspeed = 10f;

    Rigidbody2D rb;
    void Start()
    {
        float randomSize = Random.Range(minSize,maxSize);
        transform.localScale  = new Vector3(randomSize, randomSize, 1);

        rb = GetComponent<Rigidbody2D>();

        float randomSpeed = Random.Range(minSpeed,maxSpeed)/randomSize;
        Vector2 randomDirection = Random.insideUnitCircle;
        rb.AddForce(randomDirection * randomSpeed);

        float randomTorque = Random.Range(-maxSpinspeed,maxSpinspeed);
        rb.AddTorque(randomTorque);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

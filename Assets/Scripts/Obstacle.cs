using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize;
    public float maxSize;
    public float minSpeed;
    public float maxSpeed;
    public float RealmaxSpeed;

    public float maxSpinspeed;
    public GameObject bounceEffectPrefab;
    public GameObject ExplosionEffectPrefab;
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
        Vector3 currentVelocity = rb.linearVelocity;
        if(currentVelocity.magnitude > RealmaxSpeed)
        {
            currentVelocity = currentVelocity.normalized * RealmaxSpeed;
            rb.linearVelocity = currentVelocity;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject ExplosionEffect = Instantiate(ExplosionEffectPrefab, contactPoint, Quaternion.identity);

            Destroy(ExplosionEffect, 1f);
            Destroy(gameObject);
        }
        else
        {
            GameObject bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);

            Destroy(bounceEffect, 1f);
        }
    }
}

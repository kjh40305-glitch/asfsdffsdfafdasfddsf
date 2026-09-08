using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float thrustForce = 1f;
    Rigidbody2D rb;

    public float maxSpeed = 5f;
    public GameObject boost;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Debug.Log(mousePos);

            Vector2 direction = (mousePos - transform.position).normalized;
            transform.up = direction;

            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boost.SetActive(true);
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boost.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}


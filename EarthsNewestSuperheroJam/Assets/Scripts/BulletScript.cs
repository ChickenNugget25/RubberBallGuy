using System.Reflection;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private float impactForce;
    private Vector2 direction;

    private float lifetime;
    private bool shouldBounce;

    private Rigidbody2D rb;

    public void Initialize(Vector2 dir, float spd, float force, float size, bool bounce, float life)
    {
        direction = dir.normalized;
        speed = spd;
        impactForce = force;
        shouldBounce = bounce;
        lifetime = life;

        transform.localScale = Vector3.one * size;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.linearVelocity = direction * speed;

        Destroy(gameObject, lifetime); // destroy bullet after lifetime expires
    }

    private void Update() {
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D otherRb = collision.rigidbody;
        
        if(otherRb == null) {
            if (!shouldBounce)
            {
                Destroy(gameObject);
            }
            return;
        }

        ForceReceiver receiver = otherRb.GetComponent<ForceReceiver>();

        if (receiver != null)
        {
            Vector2 hitDirection = -collision.contacts[0].normal;
            receiver.ApplyForce(hitDirection * impactForce, ForceMode2D.Impulse);
            if (!shouldBounce)
            {
                Destroy(gameObject);
            }
        }
        else
        {

            if (otherRb != null && collision.contactCount > 0)
            {
                Vector2 hitDirection = -collision.contacts[0].normal;
                otherRb.AddForce(hitDirection * impactForce, ForceMode2D.Impulse);
            }   

            if (!shouldBounce)
            {
                Destroy(gameObject);
            }
        }
    }
}
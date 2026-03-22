using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float impactForce;

    public bool destroyOnContact = false;
    private Rigidbody2D rb;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D otherRb = collision.rigidbody;
        
        if(otherRb == null) {
            return;
        }

        ForceReceiver receiver = otherRb.GetComponent<ForceReceiver>();

        if (receiver != null)
        {
            Vector2 hitDirection = -collision.contacts[0].normal;
            receiver.ApplyForce(hitDirection * impactForce, ForceMode2D.Impulse);
        }
        else
        {

            if (otherRb != null && collision.contactCount > 0)
            {
                Vector2 hitDirection = -collision.contacts[0].normal;
                otherRb.AddForce(hitDirection * impactForce, ForceMode2D.Impulse);
            }   
        }

        if (destroyOnContact)
        {
            Destroy(gameObject);
        }
        
    }
}

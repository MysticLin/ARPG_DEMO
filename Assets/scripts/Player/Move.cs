using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public int facingDirection = 1;
    public Rigidbody2D rb;
    public int speed;
    private void FixedUpdate()
    {
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal > 0 && facingDirection < 0 || horizontal < 0 && facingDirection > 0)
        {
            Filp();
        }
        rb.velocity=new Vector2 (horizontal, vertical).normalized*speed;
    }
    private void Filp()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
}

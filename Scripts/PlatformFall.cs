using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFall : MonoBehaviour
{
    private Rigidbody2D rb;
    public float fallDelay = 1f;

    void Start()
    {
      rb = GetComponent<Rigidbody2D>();    
    }

 IEnumerator Fall()
    {
        yield return new WaitForSeconds(0.2f);
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, fallDelay);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }
}

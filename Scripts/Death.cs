using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    public float respawnDelay = 0.1f;
    public AudioClip deathSound;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            Die();
        }
    }

    void Die()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        spriteRenderer.enabled = false;
        Invoke(nameof(RespawnPlayer), respawnDelay);
    }

    void RespawnPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

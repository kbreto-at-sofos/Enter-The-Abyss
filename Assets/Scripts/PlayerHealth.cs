using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int _currentHealth;
    public HealthUI healthUI;

    private SpriteRenderer _spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetHealth();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        EventSubscriber.Subscribe(GameEvent.ResetGame, ResetHealth);
    }

    private void ResetHealth()
    {
        _currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(enemy.damage);
            SoundEffectManager.Play("PlayerHit");
        }
    }

    private void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        healthUI.UpdateHearts(_currentHealth);

        StartCoroutine(FlashRed());
        if (_currentHealth <= 0)
        {
            Debug.Log("Game Over");
            // player dead! call game over.
            EventSubscriber.Publish(GameEvent.PlayerDied);
        }
    }

    private IEnumerator FlashRed()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.color = Color.white;
    }
}

using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int _currentHealth;
    public HealthUI healthUI;

    private SpriteRenderer _spriteRenderer;

    public static event Action OnPlayerDied;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(enemy.damage);
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
            OnPlayerDied?.Invoke();
        }
    }

    private IEnumerator FlashRed()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.color = Color.white;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

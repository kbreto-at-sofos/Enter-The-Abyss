using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    public Image heartPrefab;

    public Sprite fullHeartSprite;

    public Sprite emptyHeartSprite;

    public List<Image> hearts = new List<Image>();

    public void SetMaxHearts(int maxHearts)
    {
        foreach (Image heart in hearts)
        {
            Destroy(heart.gameObject);
        }
        
        hearts.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(heartPrefab, transform);
            newHeart.sprite = fullHeartSprite;
            newHeart.color = Color.red;
            hearts.Add(newHeart);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeartSprite;
                hearts[i].color = Color.red;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
                hearts[i].color = Color.white;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

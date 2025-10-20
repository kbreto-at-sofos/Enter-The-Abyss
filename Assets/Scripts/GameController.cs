using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private int _progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private int _currentLevelIndex;

    void Start()
    {
        _progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        loadCanvas.SetActive(false);
    }

    private void IncreaseProgressAmount(int amount)
    {
        _progressAmount += amount;
        progressSlider.value = _progressAmount;
        if (_progressAmount >= 100)
        {
            // Level complete!
            Debug.Log(_progressAmount);
            Debug.Log("Level Completed!");
            
            loadCanvas.SetActive(true);
        }
    }

    private void LoadNextLevel()
    {
        int nextLevelIndex = (_currentLevelIndex == levels.Count - 1) ? 0 : _currentLevelIndex + 1;
        loadCanvas.SetActive(false);
        
        levels[_currentLevelIndex].gameObject.SetActive(false);
        levels[nextLevelIndex].gameObject.SetActive(true);

        player.transform.position = new Vector3(0, 2, 0);

        _currentLevelIndex = nextLevelIndex;
        _progressAmount = 0;
        progressSlider.value = 0;

    }
}

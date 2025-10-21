using System.Collections.Generic;
using TMPro;
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
    
    public GameObject gameOverScreen;
    public TMP_Text scoreText;
    private int _survivedLevelsCount = 0;
    private int _gemsCollectedCount  = 0;

    void Start()
    {
        _progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;
        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    private void IncreaseProgressAmount(int amount)
    {
        _gemsCollectedCount++;
        Debug.Log(_gemsCollectedCount);
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
        _survivedLevelsCount++;

    }

    private void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        scoreText.text = "YOU GOT "+ _gemsCollectedCount +" GEMS IN YOUR ADVENTURE";
    }
}

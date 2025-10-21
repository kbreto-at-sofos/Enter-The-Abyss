using System;
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
    
    public static event Action OnReset;

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

    private void LoadLevel(int level, bool increaseSurvivedLevels = false)
    {
        loadCanvas.SetActive(false);
        
        levels[_currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = new Vector3(0, 2, 0);

        _currentLevelIndex = level;
        _progressAmount = 0;
        progressSlider.value = 0;
        if(increaseSurvivedLevels) _survivedLevelsCount++;
    }

    private void LoadNextLevel()
    {
        int nextLevelIndex = (_currentLevelIndex == levels.Count - 1) ? 0 : _currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);

    }

    private void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        scoreText.text = "YOU GOT "+ _gemsCollectedCount +" GEMS IN YOUR ADVENTURE";
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        _survivedLevelsCount = 0;
        _gemsCollectedCount = 0;
        LoadLevel(0);
        OnReset?.Invoke();
        Time.timeScale = 1;
    }
}

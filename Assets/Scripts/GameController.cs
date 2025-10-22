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
    
    public GameObject gameOverScreen;
    public TMP_Text scoreText;
    public TMP_Text levelText;

    public LevelConfiguration levelConfiguration;
    public int currentLevel = 0;
    public static event Action OnReset;

    void Start()
    {
        InitialLevelConfiguration();
        _progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;
        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    void InitialLevelConfiguration()
    {
        levelConfiguration.CurrentLevel = currentLevel;
        levelConfiguration.GemCollectedCount = 0;
        levelConfiguration.LevelCompletedCount = 0;
    }

    private void IncreaseProgressAmount(int amount)
    {
        levelConfiguration.GemCollectedCount++;
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
        
        levels[levelConfiguration.CurrentLevel].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = new Vector3(0, 2, 0);
        levelConfiguration.CurrentLevel = level;
        _progressAmount = 0;
        progressSlider.value = 0;
        if(increaseSurvivedLevels) levelConfiguration.LevelCompletedCount++;
    }

    private void LoadNextLevel()
    {
        int nextLevelIndex = (levelConfiguration.CurrentLevel == levels.Count - 1) ? 0 : levelConfiguration.CurrentLevel + 1;
        LoadLevel(nextLevelIndex, true);

    }

    private void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        scoreText.text = "YOU GOT "+ levelConfiguration.GemCollectedCount +" GEMS IN YOUR ADVENTURE";
        levelText.text = "YOU COMPLETED "+ levelConfiguration.LevelCompletedCount +" LEVEL";
        if (levelConfiguration.LevelCompletedCount != 1) levelText.text += "S";
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        levelConfiguration.GemCollectedCount = 0;
        levelConfiguration.LevelCompletedCount = 0;
        LoadLevel(0);
        OnReset?.Invoke();
        Time.timeScale = 1;
    }
}

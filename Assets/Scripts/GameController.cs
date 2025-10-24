using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private int _progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private readonly List<bool> _levelsActive = new List<bool>();

    public GameObject gameOverScreen;
    public TMP_Text scoreText;
    public TMP_Text levelText;
    public Button retryButton;
    public Button continueButton;

    public LevelConfiguration levelConfiguration;
    public History history;
    public static event Action OnReset;

    void Start()
    {
        // set levelConfig counters to 0
        InitialLevelConfiguration();

        // set every level as inactive
        for (int i = 0; i < levels.Count; i++)
        {
            _levelsActive.Add(false);
        }

        // load current level
        LoadLevel(levelConfiguration.CurrentLevel);

        // progress bar
        _progressAmount = 0;
        progressSlider.value = 0;

        // asign functions
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += delegate { GameOverScreen(); };

        // deactivate canvases
        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        // keep track of time
        levelConfiguration.GameTime += Time.deltaTime;
    }

    void InitialLevelConfiguration()
    {
        levelConfiguration.IsWin = false;
        levelConfiguration.GemCollectedCount = 0;
        levelConfiguration.LevelCompletedCount = 0;
        levelConfiguration.GameTime = 0f;
    }

    private void IncreaseProgressAmount(int amount)
    {
        levelConfiguration.GemCollectedCount++;
        _progressAmount += amount;
        progressSlider.value = _progressAmount;
        if (_progressAmount >= 100)
        {
            // Level complete!

            loadCanvas.SetActive(true);
        }
    }

    private void LoadLevel(int level, bool increaseSurvivedLevels = false)
    {
        HistoryManager.ShowHistory(history.levelHistory[level]);
        
        loadCanvas.SetActive(false);
        
        // call credits
        if (level >= levels.Count)
        {
            GameOverScreen(true);
            return;
        }

        // it always will be costly to check/modify activeSelf property 
        //https://discussions.unity.com/t/the-cost-of-gameobject-setactive-with-no-change/940885/6

        // deactivate all other levels but the one sent in the args
        // call setActive only when needed
        levels[levelConfiguration.CurrentLevel].gameObject.SetActive(false);
        _levelsActive[levelConfiguration.CurrentLevel] = false;
        
        for (int i = 0; i < _levelsActive.Count; i++)
        {
            if (i == level && !_levelsActive[i])
            {
                levels[i].gameObject.SetActive(true);
                _levelsActive[i] = true;
            }
            else if (i != level && _levelsActive[i])
            {
                levels[i].gameObject.SetActive(false);
                _levelsActive[i] = false;
            }
        }

        player.transform.position = new Vector3(0, 2, 0);
        levelConfiguration.CurrentLevel = level;
        _progressAmount = 0;
        progressSlider.value = 0;
        if (increaseSurvivedLevels) levelConfiguration.LevelCompletedCount++;
    }

    private void LoadNextLevel()
    {
        // loop game
        int nextLevelIndex = (levelConfiguration.CurrentLevel == levels.Count - 1)
            ? 0
            : levelConfiguration.CurrentLevel + 1;
        LoadLevel(levelConfiguration.CurrentLevel + 1, true);
    }
    public void LoadCredits()
    {
        levelConfiguration.IsWin = true;
        SceneManager.LoadScene("Scenes/StartScene");
    }

    private void GameOverScreen(bool isWin = false)
    {
        MusicManager.PauseBackgroundMusic();

        Time.timeScale = 0;
        ActivateGameOverScreenText();
        retryButton.gameObject.SetActive(!isWin);
        continueButton.gameObject.SetActive(isWin);
        SavePlayerStats(!isWin);
    }

    private void ActivateGameOverScreenText()
    {
        gameOverScreen.SetActive(true);
        scoreText.text = "Obtuviste " + levelConfiguration.GemCollectedCount + " gemas en tu aventura";
        levelText.text = "Completaste " + levelConfiguration.LevelCompletedCount + " niveles" +
                         $"\ntiempo transcurrido: {Mathf.Round(levelConfiguration.GameTime)} segundos";
    }

    private void SavePlayerStats(bool addDeath = false)
    {
        // save total using PlayerPrefs
        float totalGems = PlayerPrefs.GetFloat("TotalGems", 0);
        totalGems += levelConfiguration.GemCollectedCount;
        PlayerPrefs.SetFloat("TotalGems", totalGems);

        float totalLevels = PlayerPrefs.GetFloat("TotalLevels", 0);
        totalLevels += levelConfiguration.LevelCompletedCount;
        PlayerPrefs.SetFloat("TotalLevels", totalLevels);

        if (addDeath)
        {
            int totalDeaths = PlayerPrefs.GetInt("TotalDeaths", 0);
            totalDeaths++;
            PlayerPrefs.SetInt("TotalDeaths", totalDeaths);
        }

        float totalTime = PlayerPrefs.GetFloat("TotalTime", 0);
        totalTime += levelConfiguration.GameTime;
        PlayerPrefs.SetFloat("TotalTime", totalTime);

        PlayerPrefs.Save();
    }

    public void ResetGame()
    {
        MusicManager.PlayBackgroundMusic(true);
        gameOverScreen.SetActive(false);
        loadCanvas.SetActive(false);
        LoadLevel(0);
        InitialLevelConfiguration();
        OnReset?.Invoke();
        Time.timeScale = 1;
    }
}
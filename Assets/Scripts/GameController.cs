using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private int _progressAmount;
    [Header("Slider")] public Slider progressSlider;

    [Header("Game Properties")] public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private readonly List<bool> _levelsActive = new List<bool>();

    [Header("Game Over Properties")] public GameObject gameOverScreen;
    public TMP_Text scoreText;
    public TMP_Text levelText;

    [Header("Pause Properties")] public GameObject pauseScreen;
    private bool _isPaused;

    [Header("Level Completed Properties")] 
    public GameObject levelCompletedScreen;
    public TMP_Text levelCompletedText;
    private int _loadLevel; 


    [Header("Scriptable Objects")] public LevelConfiguration levelConfiguration;
    public History history;

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

        // assign functions
        EventSubscriber<int>.Subscribe(GameEvent.GemCollected, IncreaseProgressAmount);
        EventSubscriber.Subscribe(GameEvent.LevelCompleted, OnLevelCompleted);
        EventSubscriber.Subscribe(GameEvent.PlayerDied, GameOverScreen);
        EventSubscriber.Subscribe(GameEvent.HistoryClosed, OnHistoryClosed);
        

        // deactivate canvases
        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
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


        _loadLevel = level;
        HistoryManager.ShowHistory(history.levelHistory[level]);
        loadCanvas.SetActive(false);
        
        // call credits
        if (level >= levels.Count) return;

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

    private void LoadCredits()
    {
        Time.timeScale = 0;
        SavePlayerStats();
        levelConfiguration.IsWin = true;
        SceneManager.LoadScene("Scenes/StartScene");
    }

    private void GameOverScreen()
    {
        MusicManager.PauseBackgroundMusic();

        Time.timeScale = 0;
        ActivateGameOverScreenText();
        SavePlayerStats(true);
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
        EventSubscriber.Publish(GameEvent.ResetGame);
        Time.timeScale = 1;
    }

    public void OnPause()
    {
        if (_isPaused)
        {
            OnResume();
        }
        else
        {
            MusicManager.PauseBackgroundMusic();
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
            _isPaused = true;
        }
    }

    public void OnResume()
    {
        _isPaused = false;
        MusicManager.PlayBackgroundMusic(false);
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnExit()
    {
        SceneManager.LoadScene("Scenes/StartScene");
        Time.timeScale = 1;
    }

    private void OnLevelCompleted()
    {
        Time.timeScale = 0;
        levelCompletedScreen.SetActive(true);
        levelCompletedText.text = $"Nivel {levelConfiguration.CurrentLevel + 1} Completado!\n\nGemas Recolectadas {levelConfiguration.GemCollectedCount}\nTiempo Transcurrido {levelConfiguration.GameTime} segundos";

    }

    public void OnContinueToNextLevel()
    {
        
        // loop game
        // int nextLevelIndex = (levelConfiguration.CurrentLevel == levels.Count - 1)
        //     ? 0
        //     : levelConfiguration.CurrentLevel + 1;
        
        levelCompletedScreen.SetActive(false);
        var nextLevel = levelConfiguration.CurrentLevel + 1;
        LoadLevel(nextLevel, true);
    }

    private void OnHistoryClosed()
    {
        // call credits
        if (_loadLevel >= levels.Count)
        {
            LoadCredits();
        }
    }
}
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{

    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    public GameObject tutorialCanvas;
    public GameObject progressCanvas;
    public GameObject creditsCanvas;

    public TMP_Text progressText;

    public List<Button> levelSelectionButtons;

    public LevelConfiguration levelConfiguration;

    public void Start()
    {

        for (int i = 0; i < levelSelectionButtons.Count; i++)
        {
            var level = i;
            levelSelectionButtons[i].onClick.AddListener(delegate { OnSelectLevelButton(level); });
        }
        
        if (levelConfiguration.IsWin)
        {
            levelConfiguration.IsWin = false;
            OnCreditsClick();
        }
    }
    

    public void OnStartClick()
    {
        mainMenuCanvas.SetActive(false);
        levelSelectionCanvas.SetActive(true);
    }

    public void OnSelectLevelButton(int level)
    {
        levelConfiguration.CurrentLevel = level;
        SceneManager.LoadScene("Scenes/GameScene");
    }
    
    

    public void OnExitClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    
    // mostrar tutorial
    public void OnTutorialClick()
    {
        mainMenuCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);
    }
    
    // mostrar menu principal
    public void OnGoToMainMenuClick()
    {
        mainMenuCanvas.SetActive(true);
        
        levelSelectionCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        progressCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
    }

    public void OnProgressClick()
    {
        
        mainMenuCanvas.SetActive(false);
        progressCanvas.SetActive(true);
        
        float totalGems = PlayerPrefs.GetFloat("TotalGems", 0);
        float totalLevels = PlayerPrefs.GetFloat("TotalLevels", 0);
        float totalDeaths = PlayerPrefs.GetInt("TotalDeaths", 0);
        float totalTime = PlayerPrefs.GetFloat("TotalTime", 0);
        progressText.text =
            $"ESTADISTICAS DE JUEGO\n\nGemas obtenidas: {totalGems}\n\nNiveles Completados: {totalLevels}\n\nMuertes: {totalDeaths}\n\nTiempo de juego: {Mathf.Round(totalTime)} segundos ";

    }

    public void OnCreditsClick()
    {
        mainMenuCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }
}

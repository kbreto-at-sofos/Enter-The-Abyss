using TMPro;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    private static HistoryManager _instance;

    public GameObject historyCanvas;
    public TMP_Text historyText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void ShowHistory(string text)
    {
        Time.timeScale = 0;
        _instance.historyCanvas.SetActive(true);
        _instance.historyText.text = text;
        
    }

    public static void HideHistory()
    {
        _instance.historyCanvas.SetActive(false);
        _instance.historyText.text = "";
        Time.timeScale = 1;
    }
}

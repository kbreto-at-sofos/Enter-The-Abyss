using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Scriptable Objects/LevelConfiguration")]
public class LevelConfiguration : ScriptableObject
{

    public int GemCollectedCount = 0;
    public int LevelCompletedCount = 0;
    public float GameTime = 0f;

    public int CurrentLevel = 0;
    

    public List<int> LevelMaxEnemies; 
    public List<int> LevelMaxGems;

    public bool IsWin = false;


    public int GetMaxObjects()
    {
        return GetMaxEnemies() + GetMaxGems();
    }

    public int GetMaxGems()
    {
        return LevelMaxGems[CurrentLevel];
    }

    public int GetMaxEnemies()
    {
        return LevelMaxEnemies[CurrentLevel];
    }
}

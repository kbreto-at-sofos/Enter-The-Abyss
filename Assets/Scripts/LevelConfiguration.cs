using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Scriptable Objects/LevelConfiguration")]
public class LevelConfiguration : ScriptableObject
{

    public int GemCollectedCount = 0;
    public int LevelCompletedCount = 0;

    public int CurrentLevel = 0;

    public List<int> LevelMaxEnemies; 
    public List<int> LevelMaxGems; 
    
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "History", menuName = "Scriptable Objects/History")]
public class History : ScriptableObject
{
    public List<string> levelHistory;
}

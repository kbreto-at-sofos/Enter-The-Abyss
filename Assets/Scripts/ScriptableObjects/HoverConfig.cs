using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "HoverConfig", menuName = "Scriptable Objects/HoverConfig")]
public class HoverConfig : ScriptableObject
{
    public float onEnterDuration = 1f;
    public float onExitDuration = 1f;
    public float onEnterScale = 1.2f;
    public float onExitScale = 1f;
    
    public Ease  onEnterEase = Ease.Linear;
    public Ease  onExitEase = Ease.Linear;
}

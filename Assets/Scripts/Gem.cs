using System;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{

    public int worth = 5;

    public void Collect()
    {
        EventSubscriber<int>.Publish(GameEvent.GemCollected, worth);
        SoundEffectManager.Play("Gem");
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}

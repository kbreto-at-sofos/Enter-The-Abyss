using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HoverAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public HoverConfig hoverConfig;
    public bool doSoundEffect;
    private EventTrigger _eventTrigger;
    void Start()
    {
        DOTween.Init();

        // GeventTrigger the EventTrigger component
        _eventTrigger = GetComponent<EventTrigger>();
        if (_eventTrigger == null)
        {
            _eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        AddPointerEnterEvent();
        AddPointerExitEvent();
    }
    
    private void AddPointerEnterEvent()
    {


        // Create a new entry for PointerEnter
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        
        // Create the trigger and set the callback
        pointerEnterEntry.callback = new EventTrigger.TriggerEvent();
        pointerEnterEntry.callback.AddListener((data) => OnPointerEnter());

        // Add the entry to the EventTrigger
        _eventTrigger.triggers.Add(pointerEnterEntry);
    }

    private void AddPointerExitEvent()
    {
        // Create a new entry for PointerExit
        EventTrigger.Entry pointerEnterExit = new EventTrigger.Entry();
        pointerEnterExit.eventID = EventTriggerType.PointerExit;
        
        // Create the trigger and set the callback
        pointerEnterExit.callback = new EventTrigger.TriggerEvent();
        pointerEnterExit.callback.AddListener((data) => OnPointExit());
        
        // Add the entry to the EventTrigger
        _eventTrigger.triggers.Add(pointerEnterExit);
    }


    private void OnPointerEnter()
    {
        gameObject.transform.DOScale(hoverConfig.onEnterScale, hoverConfig.onEnterDuration).SetEase(hoverConfig.onEnterEase).Play();
        if (doSoundEffect)
        {
            SoundEffectManager.Play("ButtonHover");
        }
    }

    private void OnPointExit()
    {
        gameObject.transform.DOScale(hoverConfig.onExitScale, hoverConfig.onExitDuration).SetEase(hoverConfig.onExitEase).Play();
        
    }
    
}

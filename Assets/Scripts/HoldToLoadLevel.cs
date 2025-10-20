using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoadLevel : MonoBehaviour
{
    
    
    public float holdDuration = 1f;
    public Image fillCircle;

    private float _holdTimer;

    private bool _isHolding;

    public static event Action OnHoldComplete;


    // Update is called once per frame
    void Update()
    {
        if (_isHolding)
        {
            _holdTimer += Time.deltaTime;
            fillCircle.fillAmount = _holdTimer / holdDuration;
            if (_holdTimer >= holdDuration)
            {
                //load next level
                OnHoldComplete?.Invoke();
                ResetHold();
            }
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isHolding = true;
        }
        else if (context.canceled)
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        _isHolding = false;
        _holdTimer = 0f;
        fillCircle.fillAmount = 0f;
    }
}

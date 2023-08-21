using System;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public event Action OnPlayerRespawned;
    public void CallPlayerRespawned()
    {
        OnPlayerRespawned?.Invoke();
    }
    
    public event Action OnButtonSequenceCompleted;
    public void CallSequenceCompleted()
    {
        OnButtonSequenceCompleted?.Invoke();
    }    
    public event Action OnCoinSequenceStarted;
    public void CallCoinSequenceStarted()
    {
        OnCoinSequenceStarted?.Invoke();
    }
    
    
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MoveForEvent : MovePlane
{
   
    private Vector3 initialPosition;
    void Start()
    {
        base.Start();
        initialPosition = transform.position;
        EventManager.Instance.OnButtonSequenceCompleted += HandleStoneMove;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnButtonSequenceCompleted -= HandleStoneMove;
    }

    private void HandleStoneMove()
    {
        StartCoroutine(MoveStoneCoroutine());
    }

    private IEnumerator MoveStoneCoroutine()
    {
        float elapsedTime = 0;
        while (elapsedTime < allTime)
        {
            float lerpBase = elapsedTime / allTime;
            float curveValue = movementCurve.Evaluate(lerpBase);
            transform.position = Vector3.Lerp(initialPosition, changePos+initialPosition, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Reset the position
        transform.position = changePos+initialPosition;
        
    }

 
}

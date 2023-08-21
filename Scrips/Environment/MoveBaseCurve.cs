using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBaseCurve : MovePlane
{
    private Vector3 initialPos;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        initialPos = transform.position;
    }

    private void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        float t = currentTime / allTime;
        float curveValue = movementCurve.Evaluate(t);

        Vector3 targetPos = initialPos + changePos * curveValue;
        rb.MovePosition(targetPos);
    }
}

using System.Collections;
using UnityEngine;
public class MovePlane : MonoBehaviour
{
    protected Rigidbody rb;

    public Vector3 changePos;
    public float allTime = 5.0f;
    public AnimationCurve movementCurve;
    protected float currentTime;

    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
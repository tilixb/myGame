using UnityEngine;

public class RespawnPosManager : Singleton<RespawnPosManager>
{
    [SerializeField]
    private Transform initialRespawnPos;

    private Vector3 currentRespawnPos;

    protected override void Awake()
    {
        base.Awake();
        currentRespawnPos = initialRespawnPos.position;
    }

    public void SetRespawnPos(Vector3 newPos)
    {
        currentRespawnPos = newPos;
    }

    public Vector3 GetRespawnPos()
    {
        return currentRespawnPos;
    }
}
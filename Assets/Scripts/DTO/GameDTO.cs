using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// delegates
public delegate void OnFloorRequestCallback(FloorRequest request);
public delegate void OnCallRequestCallback(CallRequest request);

public enum ResultCode
{
    None,
    Failed,
    FloorRequestSucceeded,
    CallRequestSucceeded
}

public class GameDTO
{

}

[SerializeField]
public class FloorRequest
{
    public uint level;
    public Direction direction;
}

[SerializeField]
public class FloorResponse
{
    public ResultCode resultCode;
    public FloorData floorData;
}

[SerializeField]
public class CallRequest
{
    public uint level;
}

[SerializeField]
public class CallResponse
{
    public ResultCode resultCode;
    public uint levelRequested;
}

[SerializeField]
public class ElevatorResponse
{
    public ResultCode resultCode;
    ElevatorData elevatorData;
}

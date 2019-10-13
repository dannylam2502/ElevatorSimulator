using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// delegates
public delegate void OnFloorRequestCallback(FloorRequest request);
public delegate void OnCallRequestCallback(CallRequest request);
public delegate void Callback();
public delegate void OnElevatorStatusUpdateRequestCallback(ElevatorStatusUpdateRequest request);

public enum ResultCode
{
    None,
    Failed,
    Succeeded,
}

public class GameDTO
{

}

[Serializable]
public class FloorRequest
{
    public uint level;
    public Direction direction;
}

[Serializable]
public class FloorResponse
{
    public ResultCode resultCode;
    public FloorData floorData;
}

[Serializable]
public class CallRequest
{
    public uint level;
}

[Serializable]
public class CallResponse
{
    public ResultCode resultCode;
    public uint levelRequested;
}

[Serializable]
public class ElevatorUpdateResponse
{
    public ElevatorData updatedElevatorData;
    // only calculate Y position
    public float destinationY;
}

[Serializable]
public class ElevatorDataResponse
{
    public ElevatorData elevatorData;
}

[Serializable]
public class ElevatorStatusUpdateRequest
{
    public ElevatorStatus newStatus;
}

[Serializable]
public class ElevatorStatusUpdateResponse
{
    public ResultCode resultCode;
    public ElevatorData elevatorData;
}
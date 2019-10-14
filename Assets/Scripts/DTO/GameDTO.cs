using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// delegates
public delegate void OnFloorRequestCallback(CallElevatorRequest request);
public delegate void OnCallRequestCallback(CallFloorRequest request);
public delegate void Callback();
public delegate void OnElevatorStatusUpdateRequestCallback(UpdateElevatorStatusRequest request);
public delegate void OnUpdateElevatorPositionCallback(UpdateElevatorPositionRequest request);

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
public class CallElevatorRequest
{
    public uint level;
    public Direction direction;
}

[Serializable]
public class CallElevatorResponse
{
    public ResultCode resultCode;
    public FloorData floorData;
}

[Serializable]
public class CallFloorRequest
{
    public uint level;
}

[Serializable]
public class CallFloorResponse
{
    public ResultCode resultCode;
    public uint levelRequested;
}

[Serializable]
public class UpdateElevatorResponse
{
    public ElevatorData updatedElevatorData;
    // only calculate Y position
    public float destinationY;
}

[Serializable]
public class GetElevatorDataResponse
{
    public ElevatorData elevatorData;
}

[Serializable]
public class UpdateElevatorStatusRequest
{
    public ElevatorStatus newStatus;
}

[Serializable]
public class UpdateElevatorStatusResponse
{
    public ResultCode resultCode;
    public ElevatorData elevatorData;
}

[Serializable]
public class UpdateElevatorPositionRequest
{
    public float positionY;
}

[Serializable]
public class UpdateElevatorPositionResponse
{
    public ResultCode resultCode;
    public uint newLevel;
}
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// delegates
public delegate void OnCallElevatorRequestCallback(CallElevatorRequest request);
public delegate void OnCallFloorRequestCallback(CallFloorRequest request);
public delegate void Callback();
public delegate void OnUpdateElevatorStatusRequestCallback(UpdateElevatorStatusRequest request);
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

    private CallElevatorRequest() { }

    public CallElevatorRequest(uint level, Direction direction)
    {
        this.level = level;
        this.direction = direction;
    }
}

[Serializable]
public class CallElevatorResponse
{
    public ResultCode resultCode;
    public FloorData floorData;

    private CallElevatorResponse() { }
    
    public CallElevatorResponse(ResultCode resultCode, FloorData floorData)
    {
        this.resultCode = resultCode;
        this.floorData = floorData?.DeepCopy();
    }
}

[Serializable]
public class CallFloorRequest
{
    public uint level;

    private CallFloorRequest() { }

    public CallFloorRequest(uint level)
    {
        this.level = level;
    }
}

[Serializable]
public class CallFloorResponse
{
    public ResultCode resultCode;
    public uint levelRequested;

    private CallFloorResponse() { }

    public CallFloorResponse(ResultCode resultCode, uint levelRequested)
    {
        this.resultCode = resultCode;
        this.levelRequested = levelRequested;
    }
}

[Serializable]
public class UpdateElevatorResponse
{
    public ElevatorData updatedElevatorData;
    // only calculate Y position
    public float destinationY;

    private UpdateElevatorResponse() { }

    public UpdateElevatorResponse(ElevatorData updatedElevatorData, float destinationY)
    {
        this.updatedElevatorData = updatedElevatorData?.DeepCopy();
        this.destinationY = destinationY;
    }
}

[Serializable]
public class GetElevatorDataResponse
{
    public ElevatorData elevatorData;

    private GetElevatorDataResponse() { }

    public GetElevatorDataResponse(ElevatorData elevatorData)
    {
        this.elevatorData = elevatorData?.DeepCopy();
    }
}

[Serializable]
public class UpdateElevatorStatusRequest
{
    public ElevatorStatus newStatus;

    private UpdateElevatorStatusRequest() { }

    public UpdateElevatorStatusRequest(ElevatorStatus newStatus)
    {
        this.newStatus = newStatus;
    }
}

[Serializable]
public class UpdateElevatorStatusResponse
{
    public ResultCode resultCode;
    public ElevatorStatus curStatus;

    private UpdateElevatorStatusResponse() { }

    public UpdateElevatorStatusResponse(ResultCode resultCode, ElevatorStatus curStatus)
    {
        this.resultCode = resultCode;
        this.curStatus = curStatus;
    }
}

[Serializable]
public class UpdateElevatorPositionRequest
{
    public float positionY;

    private UpdateElevatorPositionRequest() { }

    public UpdateElevatorPositionRequest(float positionY)
    {
        this.positionY = positionY;
    }
}

[Serializable]
public class UpdateElevatorPositionResponse
{
    public ResultCode resultCode;
    public uint newLevel;

    private UpdateElevatorPositionResponse() { }

    public UpdateElevatorPositionResponse(ResultCode resultCode, uint newLevel)
    {
        this.resultCode = resultCode;
        this.newLevel = newLevel;
    }
}

[SerializeField]
public class ElevatorArrivedResponse
{
    public FloorData floorData;

    private ElevatorArrivedResponse() { }

    public ElevatorArrivedResponse(FloorData floorData)
    {
        this.floorData = floorData?.DeepCopy();
    }
}
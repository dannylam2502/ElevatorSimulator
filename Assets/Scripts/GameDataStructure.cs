using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataStructure : MonoBehaviour
{
    
}

[Flags]
public enum Direction
{
    None = 0b_0000_0000,
    Up = 0b_0000_0001,
    Down = 0b_0000_0010,
    Both = Up | Down
}

[Flags]
public enum FloorStatus
{
    Waiting = Direction.None,
    RequestingUp = Direction.Up,
    RequestingDown = Direction.Down,
    RequestingUpAndDown = Direction.Both
}

public enum ElevatorStatus
{
    Waiting,
    Opening,
    Opened,
    Closing,
    Closed,
    GoingUp,
    GoingDown
}

public class FloorData
{
    public uint level;
    public FloorStatus status;
    public Direction requestableDirection; // Control if this floor can request to go up, down, or not at all

    public FloorData(uint level)
    {
        this.level = level;
        status = FloorStatus.Waiting;
        requestableDirection = Direction.Both;
        if (IsTopFloor())
        {
            requestableDirection = Direction.Down;
        }
        if (IsBottomFloor())
        {
            requestableDirection = Direction.Up;
        }
    }

    public bool IsTopFloor()
    {
        return level == GameConfig.NumFloor;
    }

    public bool IsBottomFloor()
    {
        return level == GameConfig.kBottomFloor;
    }

    /// <summary>
    /// Process the upcoming direction, return true if succeeded, otherwise false
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool ProcessRequest(Direction direction)
    {
        if ((direction & requestableDirection) == Direction.None)
        {
            return false;
        }
        status = ConvertDirectionToFloorStatus(direction) | status;
        return true;
    }

    FloorStatus ConvertDirectionToFloorStatus(Direction direction)
    {
        switch (direction)
        {
            case Direction.None:
                return FloorStatus.Waiting;
            case Direction.Up:
                return FloorStatus.RequestingUp;
            case Direction.Down:
                return FloorStatus.RequestingDown;
            case Direction.Both:
                return FloorStatus.RequestingUpAndDown;
            default:
                break;
        }
        return FloorStatus.Waiting;
    }
}

public class ElevatorData
{
    public ElevatorStatus status;
    public uint curFloorLevel;
    public HashSet<uint> listFloorsRequesting;

    public ElevatorData()
    {
        status = ElevatorStatus.Waiting;
        curFloorLevel = GameConfig.kBottomFloor;
        listFloorsRequesting = new HashSet<uint>();
    }
}

//public class FloorRequestData
//{
//    public float timeRequested;
//    public uint floorLevel;
//    public Direction direction;
//}

//public class CallRequestData
//{
//    public float timeRequested;
//    public uint floorLevel;
//}
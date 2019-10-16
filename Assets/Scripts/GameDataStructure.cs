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
    MovingUp,
    MovingDown,
    Arrived
}

[SerializeField]
public class FloorData
{
    public uint level;
    public FloorStatus status;
    public Direction requestableDirection; // Control if this floor can request to go up, down, or not at all

    public FloorData DeepCopy()
    {
        FloorData other = (FloorData)this.MemberwiseClone();
        other.level = level;
        other.status = status;
        other.requestableDirection = requestableDirection;
        return other;
    }

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

    public static FloorStatus ConvertDirectionToFloorStatus(Direction direction)
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

    public void OnElevatorArrived(Direction direction)
    {
        if (status == FloorStatus.RequestingUpAndDown)
        {
            status = status ^ ConvertDirectionToFloorStatus(direction);
        }
        //else if (status == ConvertDirectionToFloorStatus(direction))
        //{
        //    status = FloorStatus.Waiting;
        //}
        else
        {
            status = FloorStatus.Waiting;
        }
    }

    public bool HasRequestDown()
    {
        return (status & FloorStatus.RequestingDown) == FloorStatus.RequestingDown;
    }

    public bool HasRequestUp()
    {
        return (status & FloorStatus.RequestingUp) == FloorStatus.RequestingUp;
    }

    public bool HasRequestAtDirection(Direction direction)
    {
        return (status & ConvertDirectionToFloorStatus(direction)) != FloorStatus.Waiting;
    }
}

[Serializable]
public class ElevatorData
{
    public ElevatorStatus status;
    public uint curFloorLevel;
    public HashSet<uint> listFloorsRequesting;

    public ElevatorData()
    {
        status = ElevatorStatus.Waiting;
        curFloorLevel = GameConfig.GetTopFloor();
        listFloorsRequesting = new HashSet<uint>();
    }

    public ElevatorData DeepCopy()
    {
        ElevatorData other = (ElevatorData)this.MemberwiseClone();
        other.curFloorLevel = curFloorLevel;
        other.listFloorsRequesting = new HashSet<uint>(listFloorsRequesting);
        return other;
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
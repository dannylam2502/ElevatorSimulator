using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalController : MonoBehaviour
{
    [SerializeField]
    VerticalLayoutGroup layoutFloor;
    [SerializeField]
    GameObject pfFloor;
    [SerializeField]
    ElevatorController elevatorController;
    [ShowOnly]
    [SerializeField]
    uint curDestFloor; // Tracks the floor to which elevator's going to move.
    [ShowOnly]
    [SerializeField]
    Direction curElevatorDirection; // Tracks the direction elevator's moving.

    ElevatorInterfaceController elevatorInterfaceController;

    // cached ref of floor controller component
    Dictionary<uint, FloorController> dictFloor;

    Dictionary<uint, FloorData> floorData;
    ElevatorData elevatorData;

    private void Awake()
    {
        curDestFloor = GameConfig.kFloorInvalid;
        curElevatorDirection = Direction.Down;
        floorData = new Dictionary<uint, FloorData>();
        for (uint i = 0; i < GameConfig.NumFloor; i++)
        {
            FloorData data = new FloorData(i + 1);
            floorData[data.level] = data;
        }

        elevatorData = new ElevatorData();
    }

    // Start is called before the first frame update
    void Start()
    {
        dictFloor = new Dictionary<uint, FloorController>(floorData.Count);

        foreach (KeyValuePair<uint, FloorData> item in floorData)
        {
            GameObject floor = Instantiate(pfFloor, layoutFloor.transform);
            floor.transform.SetAsFirstSibling(); // for the right visual order
            FloorController component = floor.GetComponent<FloorController>();
            if (component)
            {
                component.SetFloorData(item.Value);
                component.UpdateUI();
                component.SetCallElevatorRequestCallback(OnGetFloorRequest);
                dictFloor.Add(item.Key, component);
            }
        }

        FloorController topFloorController = GetFloorController(GameConfig.GetTopFloor());
        if (topFloorController)
        {
            // Cannot use position of top floor because its correct position will be updated in next frame.
            elevatorController.SetAtDefaultAnchoredPositionY();
        }

        elevatorController.SetOnElevatorStatusUpdateCallback(OnGetElevatorStatusUpdateRequest);
        elevatorController.SetUpdateElevatorPositionCallback(OnGetUpdateElevatorPositionRequest);

        SendElevatorData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UpdateElevator();
        }
    }

    void OnGetFloorRequest(CallElevatorRequest rq)
    {
        Logger.Log(Logger.kTagReq, "OnGetFloorRequest " + JsonUtility.ToJson(rq));
        HandleFloorRequest(rq);
    }

    void HandleFloorRequest(CallElevatorRequest rq)
    {
        // Handle it
        FloorData floor = GetFloorData(rq.level);
        if (floor != null)
        {
            bool isSuccess = floor.ProcessRequest(rq.direction);
            CallElevatorResponse rs = new CallElevatorResponse(isSuccess ? ResultCode.Succeeded : ResultCode.Failed, floor);
            // Send Response to Floor
            SendCallElevatorResponse(rs);

            if (isSuccess)
            {
                // Update Elevator
                UpdateElevator();
            }
        }
        else
        {
            // Send Response to Floor
            CallElevatorResponse response = new CallElevatorResponse(ResultCode.Failed, null);
            SendCallElevatorResponse(response);
        }
    }

    void SendCallElevatorResponse(CallElevatorResponse response)
    {
        FloorController component = GetFloorController(response.floorData.level);
        if (component)
        {
            component.OnGetCallElevatorResponse(response);
        }
        Logger.Log(Logger.kTagRes, "SendCallElevatorResponse " + JsonUtility.ToJson(response));
    }

    void OnGetCallFloorRequest(CallFloorRequest request)
    {
        Logger.Log(Logger.kTagReq, "OnGetCallRequest " + JsonUtility.ToJson(request));
        HandleCallFloorRequest(request);
    }

    void HandleCallFloorRequest(CallFloorRequest request)
    {
        // Handle it

        // Send response
        CallFloorResponse response = new CallFloorResponse(ResultCode.Succeeded, request.level);

        SendCallFloorResponse(response);
        Logger.Log(Logger.kTagRes, "HandleCallRequest " + JsonUtility.ToJson(response));
    }

    void SendCallFloorResponse(CallFloorResponse response)
    {
        if (elevatorInterfaceController)
        {
            elevatorInterfaceController.OnGetCallFloorResponse(response);
        }
    }

    /// <summary>
    /// Processes data and send approriate command to elevator
    /// </summary>
    void UpdateElevator()
    {
        // find the closest floor requested at current direction
        if (elevatorData.status == ElevatorStatus.Waiting)
        {
            FloorData floorData = GetFloorData(elevatorData.curFloorLevel);
            if (floorData.status == FloorStatus.Waiting)
            {
                // Current floor doesn't have request, let's find one in both direction
                uint nextFloor = GameConfig.kFloorInvalid;
                nextFloor = FindNextRequestedFloor(Direction.Both, true);
                if (nextFloor >= GameConfig.GetBottomFloor() && nextFloor <= GameConfig.GetTopFloor())
                {
                    curDestFloor = nextFloor;
                    // Update elevator data and send response to GUI
                    elevatorData.status = curDestFloor > elevatorData.curFloorLevel ? ElevatorStatus.MovingUp : ElevatorStatus.MovingDown;
                    float destinationY = GetFloorController(curDestFloor).GetFittedElevatorAnchoredPositionY();
                    UpdateElevatorResponse response = new UpdateElevatorResponse(elevatorData, destinationY);
                    SendUpdateElevatorResponse(response);
                }
            }
            else
            {
                // Cur floor has request, sends response to floor
                curElevatorDirection = floorData.status == FloorStatus.RequestingDown ? Direction.Down : Direction.Up;
                floorData.OnElevatorArrived(curElevatorDirection);
                SendElevatorArrivedResponse(new ElevatorArrivedResponse(floorData));
                // Then sends response to elevator
                elevatorData.status = ElevatorStatus.Opening;
                UpdateElevatorResponse response = new UpdateElevatorResponse(elevatorData, 0);
                SendUpdateElevatorResponse(response);
            }
        }

        if (elevatorData.status == ElevatorStatus.Closed)
        {
            uint nextFloor = GameConfig.kFloorInvalid;
            // Find next floor with current direction
            nextFloor = FindNextRequestedFloor(curElevatorDirection, true);
            if (nextFloor == GameConfig.kFloorInvalid)
            {
                // Cannot find any floor requested at cur direction, find other direction, include current floor as well
                curElevatorDirection = curElevatorDirection == Direction.Down ? Direction.Up : Direction.Down;
                nextFloor = FindNextRequestedFloor(curElevatorDirection, false);
            }

            if (nextFloor == GameConfig.kFloorInvalid)
            {
                // No Request, waiting
                elevatorData.status = ElevatorStatus.Waiting;
                UpdateElevatorResponse response = new UpdateElevatorResponse(elevatorData, 0);
                SendUpdateElevatorResponse(response);
            }
            else
            {
                // Update elevator data and send response to GUI
                curDestFloor = nextFloor;
                elevatorData.status = curElevatorDirection == Direction.Up ? ElevatorStatus.MovingUp : ElevatorStatus.MovingDown;
                float destinationY = GetFloorController(curDestFloor).GetFittedElevatorAnchoredPositionY();
                UpdateElevatorResponse response = new UpdateElevatorResponse(elevatorData, destinationY);
                SendUpdateElevatorResponse(response);
            }
        }

        if (elevatorData.status == ElevatorStatus.MovingDown)
        {
            uint nextFloor = FindNextRequestedFloor(Direction.Down, true);
            if (nextFloor > curDestFloor) // Found a floor requested on the way, but not current floor!
            {
                curDestFloor = nextFloor;
                float destinationY = GetFloorController(curDestFloor).GetFittedElevatorAnchoredPositionY();
                UpdateElevatorResponse response = new UpdateElevatorResponse(elevatorData, destinationY);
                SendUpdateElevatorResponse(response);
            }
        }

        if (elevatorData.status == ElevatorStatus.MovingUp)
        {
            uint nextFloor = FindNextRequestedFloor(Direction.Up, true);
            if (nextFloor != GameConfig.kFloorInvalid &&
                nextFloor < curDestFloor) // Found a floor requested on the way, but not current floor!
            {
                curDestFloor = nextFloor;
                float destinationY = GetFloorController(curDestFloor).GetFittedElevatorAnchoredPositionY();
                UpdateElevatorResponse response = new UpdateElevatorResponse(elevatorData, destinationY);
                SendUpdateElevatorResponse(response);
            }
        }

        if (elevatorData.status == ElevatorStatus.Closing)
        {
            // Got Requested from current floor while closing
            FloorData floorData = GetFloorData(elevatorData.curFloorLevel);
            if (floorData.HasRequestAtDirection(curElevatorDirection))
            {
                elevatorData.status = ElevatorStatus.Opening;
                UpdateElevatorResponse updateElevatorResponse = new UpdateElevatorResponse(elevatorData, 0);
                SendUpdateElevatorResponse(updateElevatorResponse);

                // Update floor status too
                floorData.OnElevatorArrived(curElevatorDirection);
                ElevatorArrivedResponse elevatorArrivedResponse = new ElevatorArrivedResponse(floorData);
                SendElevatorArrivedResponse(elevatorArrivedResponse);
            }
        }

        if (elevatorData.status == ElevatorStatus.Opened || elevatorData.status == ElevatorStatus.Opening)
        {
            // Got Requested from current floor, then tell it we've arrived
            FloorData floorData = GetFloorData(elevatorData.curFloorLevel);
            if (floorData.HasRequestAtDirection(curElevatorDirection))
            {
                // Update floor status too
                floorData.OnElevatorArrived(curElevatorDirection);
                ElevatorArrivedResponse elevatorArrivedResponse = new ElevatorArrivedResponse(floorData);
                SendElevatorArrivedResponse(elevatorArrivedResponse);
            }
        }
    }

    uint FindNextRequestedFloor(Direction direction, bool isCurFloorExcluded)
    {
        uint variable = 0;
        if (isCurFloorExcluded)
        {
            variable = 1;
        }
        if (direction == Direction.Down)
        {
            // same direction first, then other direction
            for (uint i = elevatorData.curFloorLevel - variable; i >= GameConfig.GetBottomFloor(); i--)
            {
                FloorData floorData = GetFloorData(i);
                if (floorData.HasRequestDown())
                {
                    return floorData.level;
                }
            }
            for (uint i = elevatorData.curFloorLevel - variable; i >= GameConfig.GetBottomFloor(); i--)
            {
                FloorData floorData = GetFloorData(i);
                if (floorData.HasRequestUp())
                {
                    return floorData.level;
                }
            }
        }
        else if (direction == Direction.Up)
        {
            // same direction first, then other direction
            for (uint i = elevatorData.curFloorLevel + variable; i <= GameConfig.GetTopFloor(); i++)
            {
                FloorData floorData = GetFloorData(i);
                if (floorData.HasRequestUp())
                {
                    return floorData.level;
                }
            }
            for (uint i = elevatorData.curFloorLevel + variable; i <= GameConfig.GetTopFloor(); i++)
            {
                FloorData floorData = GetFloorData(i);
                if (floorData.HasRequestDown())
                {
                    return floorData.level;
                }
            }
        }
        else if (direction == Direction.Both)
        {
            for (uint upIndex = elevatorData.curFloorLevel + variable, downIndex = elevatorData.curFloorLevel - variable;
                    upIndex <= GameConfig.GetTopFloor() || downIndex >= GameConfig.GetBottomFloor();
                    upIndex++, downIndex--)
            {
                FloorData upFloor = GetFloorData(upIndex);
                FloorData downFloor = GetFloorData(downIndex);
                // Prioritize going down
                if (downFloor != null)
                {
                    if (downFloor.status != FloorStatus.Waiting)
                    {
                        return downFloor.level;
                    }
                }
                if (upFloor != null)
                {
                    if (upFloor.status != FloorStatus.Waiting)
                    {
                        return upFloor.level;
                    }
                }
            }
        }
        return GameConfig.kFloorInvalid;
    }

    /// <summary>
    /// Update Elevator Data to elevator controller
    /// </summary>
    void SendElevatorData()
    {
        GetElevatorDataResponse response = new GetElevatorDataResponse(elevatorData);
        SendElevatorDataResponse(response);
    }

    void SendElevatorDataResponse(GetElevatorDataResponse response)
    {
        elevatorController?.OnGetElevatorDataResponse(response);
        Logger.Log(Logger.kTagRes, "SendElevatorDataResponse " + JsonUtility.ToJson(response));
    }

    void SendUpdateElevatorResponse(UpdateElevatorResponse response)
    {
        elevatorController?.OnGetElevatorUpdateResponse(response);
        Logger.Log(Logger.kTagRes, "SendElevatorUpdateResponse " + JsonUtility.ToJson(response));
    }

    void OnGetElevatorStatusUpdateRequest(UpdateElevatorStatusRequest request)
    {
        Logger.Log(Logger.kTagReq, "OnGetElevatorStatusUpdateRequest" + JsonUtility.ToJson(request));
        HandleGetElevatorStatusUpdateRequest(request);
    }

    void HandleGetElevatorStatusUpdateRequest(UpdateElevatorStatusRequest request)
    {
        if (elevatorData.status == ElevatorStatus.Arrived && request.newStatus == ElevatorStatus.Opening)
        {
            elevatorData.status = ElevatorStatus.Opening;
            // Send both update status and update response to GUI
            UpdateElevatorStatusResponse updateStatusRepsonse = new UpdateElevatorStatusResponse(ResultCode.Succeeded, elevatorData.status);
            SendUpdateElevatorStatusResponse(updateStatusRepsonse);

            UpdateElevatorResponse updateElevatorResponse = new UpdateElevatorResponse(elevatorData, 0);
            SendUpdateElevatorResponse(updateElevatorResponse);
        }

        if (elevatorData.status == ElevatorStatus.Opening && request.newStatus == ElevatorStatus.Opened)
        {
            elevatorData.status = ElevatorStatus.Opened;
            UpdateElevatorStatusResponse response = new UpdateElevatorStatusResponse(ResultCode.Succeeded, elevatorData.status);
            SendUpdateElevatorStatusResponse(response);
        }

        if (elevatorData.status == ElevatorStatus.Opened && request.newStatus == ElevatorStatus.Closing)
        {
            elevatorData.status = ElevatorStatus.Closing;
            UpdateElevatorStatusResponse updateStatusResponse = new UpdateElevatorStatusResponse(ResultCode.Succeeded, elevatorData.status);
            SendUpdateElevatorStatusResponse(updateStatusResponse);

            UpdateElevatorResponse updateElevatorResponse = new UpdateElevatorResponse(elevatorData, 0);
            SendUpdateElevatorResponse(updateElevatorResponse);
        }

        if (elevatorData.status == ElevatorStatus.Closing && request.newStatus == ElevatorStatus.Closed)
        {
            elevatorData.status = ElevatorStatus.Closed;
            UpdateElevatorStatusResponse updateStatusResponse = new UpdateElevatorStatusResponse(ResultCode.Succeeded, elevatorData.status);
            SendUpdateElevatorStatusResponse(updateStatusResponse);

            UpdateElevator();
        }
    }

    void SendUpdateElevatorStatusResponse(UpdateElevatorStatusResponse response)
    {
        elevatorController?.OnGetElevatorStatusUpdateResponse(response);
        Logger.Log(Logger.kTagRes, "SendElevatorStatusUpdateResponse " + JsonUtility.ToJson(response));
    }

    void OnGetUpdateElevatorPositionRequest(UpdateElevatorPositionRequest request)
    {
        //Logger.Log(Logger.kTagReq, "OnGetUpdateElevatorPositionRequest " + JsonUtility.ToJson(request));
        HandleUpdateElevatorPositionRequest(request);
    }

    void HandleUpdateElevatorPositionRequest(UpdateElevatorPositionRequest request)
    {
        uint nextFloor = elevatorData.curFloorLevel;
        if (elevatorData.status == ElevatorStatus.MovingDown)
        {
            nextFloor = elevatorData.curFloorLevel - 1;
            FloorController floorController = GetFloorController(nextFloor);
            if (floorController)
            {
                if (request.positionY <= floorController.GetFittedElevatorAnchoredPositionY())
                {
                    elevatorData.curFloorLevel = nextFloor;
                }
            }
        }
        else if (elevatorData.status == ElevatorStatus.MovingUp)
        {
            nextFloor = elevatorData.curFloorLevel + 1;
            FloorController floorController = GetFloorController(nextFloor);
            if (floorController)
            {
                if (request.positionY >= floorController.GetFittedElevatorAnchoredPositionY())
                {
                    elevatorData.curFloorLevel = nextFloor;
                }
            }
        }

        // Elevator arrived
        if (elevatorData.curFloorLevel == curDestFloor)
        {
            // Update floor status using elevator status
            if (elevatorData.status == ElevatorStatus.MovingDown || elevatorData.status == ElevatorStatus.MovingUp)
            {
                FloorData floorData = GetFloorData(elevatorData.curFloorLevel);
                floorData.OnElevatorArrived(curElevatorDirection);

                ElevatorArrivedResponse elevatorArrivedResponse = new ElevatorArrivedResponse(floorData);
                SendElevatorArrivedResponse(elevatorArrivedResponse);
            }

            elevatorData.status = ElevatorStatus.Arrived;
            UpdateElevatorResponse response = new UpdateElevatorResponse(elevatorData, 0);
            SendUpdateElevatorResponse(response);
        }
        else
        {
            UpdateElevatorPositionResponse response = new UpdateElevatorPositionResponse(ResultCode.Succeeded, elevatorData.curFloorLevel);
            SendUpdateElevatorPositionResponse(response);
        }
    }

    void SendUpdateElevatorPositionResponse(UpdateElevatorPositionResponse response)
    {
        elevatorController?.OnGetUpdateElevatorPositionResponse(response);
        //Logger.Log(Logger.kTagRes, "SendUpdateElevatorPositionResponse " + JsonUtility.ToJson(response));
    }

    void SendElevatorArrivedResponse(ElevatorArrivedResponse response)
    {
        FloorController floorController = GetFloorController(response.floorData.level);
        floorController?.OnGetElevatorArrivedResponse(response);
    }

    /// <summary>
    /// Get FloorController by level, return null if key not found
    /// </summary>
    /// <param name="level">key floor level</param>
    /// <returns></returns>
    FloorController GetFloorController(uint level)
    {
        if (dictFloor.ContainsKey(level))
        {
            return dictFloor[level];
        }
        return null;
    }

    /// <summary>
    /// Get FloorData by level, return null if key not found
    /// </summary>
    /// <param name="level">key floor level</param>
    /// <returns></returns>
    FloorData GetFloorData(uint level)
    {
        if (floorData.ContainsKey(level))
        {
            return floorData[level];
        }
        return null;
    }

    public void SetElevatorInterface(ElevatorInterfaceController elevatorInterface)
    {
        elevatorInterfaceController = elevatorInterface;
    }

    // Click to elevator to open the interface
    public void OnClickElevator()
    {
        if (elevatorInterfaceController)
        {
            elevatorInterfaceController.Show(OnGetCallFloorRequest);
        }
    }
}

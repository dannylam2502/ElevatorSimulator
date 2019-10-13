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

    ElevatorInterfaceController elevatorInterfaceController;

    // cached ref of floor controller component
    Dictionary<uint, FloorController> dictFloor;

    Dictionary<uint, FloorData> floorData;
    ElevatorData elevatorData;

    private void Awake()
    {
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
                component.SetFloorRequestCallback(OnGetFloorRequest);
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

        SendElevatorData();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGetFloorRequest(FloorRequest rq)
    {
        Logger.Log(Logger.kTagReq, "OnGetFloorRequest " + JsonUtility.ToJson(rq));
        HandleFloorRequest(rq);
    }

    void HandleFloorRequest(FloorRequest rq)
    {
        // Handle it
        FloorData floor = GetFloorData(rq.level);
        if (floor != null)
        {
            bool isSuccess = floor.ProcessRequest(rq.direction);
            FloorResponse rs = new FloorResponse();
            rs.floorData = floor.DeepCopy();
            rs.resultCode = isSuccess ? ResultCode.Succeeded : ResultCode.Failed;

            // Send Response to Floor
            SendFloorResponse(rs);
            Logger.Log(Logger.kTagRes, "HandleFloorRequest " + JsonUtility.ToJson(rs));

            if (isSuccess)
            {
                // Update Elevator
                UpdateElevator();
            }
        }
        else
        {
            // Send Response to Floor
            FloorResponse rs = new FloorResponse();
            rs.resultCode = ResultCode.Failed;

            SendFloorResponse(rs);
            Logger.Log(Logger.kTagRes, "HandleFloorRequest " + JsonUtility.ToJson(rs));
        }
    }

    void SendFloorResponse(FloorResponse rs)
    {
        FloorController component = GetFloorController(rs.floorData.level);
        if (component)
        {
            component.OnGetResponse(rs);
        }
    }

    void OnGetCallRequest(CallRequest request)
    {
        Logger.Log(Logger.kTagReq, "OnGetCallRequest " + JsonUtility.ToJson(request));
        HandleCallRequest(request);
    }

    void HandleCallRequest(CallRequest request)
    {
        // Handle it

        // Send response
        CallResponse response = new CallResponse();
        response.levelRequested = request.level;
        response.resultCode = ResultCode.Succeeded;

        SendCallResponse(response);
        Logger.Log(Logger.kTagRes, "HandleCallRequest " + JsonUtility.ToJson(response));
    }

    void SendCallResponse(CallResponse response)
    {
        if (elevatorInterfaceController)
        {
            elevatorInterfaceController.OnGetCallResponse(response);
        }
    }

    void UpdateElevator()
    {
        // find the closest floor requested at current direction
        uint curFloor = elevatorData.curFloorLevel;
        if (elevatorData.status == ElevatorStatus.Waiting)
        {
            FloorData floorData = GetFloorData(curFloor);
            if (floorData.status == FloorStatus.Waiting)
            {
                uint nextFloor = 0;
                // Current floor doesn't have request, let's find one
                for (uint upIndex = curFloor + 1, downIndex = curFloor - 1;
                    upIndex <= GameConfig.GetTopFloor() || downIndex >= GameConfig.GetBottomFloor();
                    upIndex++, downIndex--)
                {
                    FloorData upFloor = GetFloorData(upIndex);
                    FloorData downFloor = GetFloorData(downIndex);
                    // Prioritize going down
                    if (downFloor != null)
                    {
                        if (downFloor.status == FloorStatus.RequestingDown)
                        {
                            nextFloor = downFloor.level;
                            break;
                        }
                    }
                    else if (upFloor != null)
                    {
                        if (upFloor.status == FloorStatus.RequestingDown)
                        {
                            nextFloor = upFloor.level;
                            break;
                        }
                    }
                }
                if (nextFloor >= GameConfig.GetBottomFloor() && nextFloor <= GameConfig.GetTopFloor())
                {
                    // Update data
                    elevatorData.status = nextFloor > curFloor ? ElevatorStatus.MovingUp : ElevatorStatus.MovingDown;

                    ElevatorUpdateResponse response = new ElevatorUpdateResponse();
                    response.updatedElevatorData = elevatorData.DeepCopy();
                    response.destinationY = GetFloorController(nextFloor).GetFittedElevatorAnchoredPositionY();

                    SendElevatorUpdateResponse(response);
                }
            }
            else
            {
                // Cur floor has request
                elevatorData.status = ElevatorStatus.Opening;

                ElevatorUpdateResponse response = new ElevatorUpdateResponse();
                response.updatedElevatorData = elevatorData.DeepCopy();
                response.destinationY = GetFloorController(curFloor).GetFittedElevatorAnchoredPositionY();

                SendElevatorUpdateResponse(response);
            }
        }
    }

    /// <summary>
    /// Update Elevator Data to elevator controller
    /// </summary>
    void SendElevatorData()
    {
        ElevatorDataResponse response = new ElevatorDataResponse();
        response.elevatorData = elevatorData.DeepCopy();

        SendElevatorDataResponse(response);
    }

    void SendElevatorDataResponse(ElevatorDataResponse response)
    {
        elevatorController?.OnGetElevatorDataResponse(response);
        Logger.Log(Logger.kTagRes, "SendElevatorDataResponse " + JsonUtility.ToJson(response));
    }

    void SendElevatorUpdateResponse(ElevatorUpdateResponse response)
    {
        elevatorController?.OnGetElevatorUpdateResponse(response);
        Logger.Log(Logger.kTagRes, "SendElevatorUpdateResponse " + JsonUtility.ToJson(response));
    }

    void OnGetElevatorStatusUpdateRequest(ElevatorStatusUpdateRequest request)
    {
        Logger.Log(Logger.kTagReq, "OnGetElevatorStatusUpdateRequest" + JsonUtility.ToJson(request));
        HandleGetElevatorStatusUpdateRequest(request);
    }

    void HandleGetElevatorStatusUpdateRequest(ElevatorStatusUpdateRequest request)
    {
        if (elevatorData.status == ElevatorStatus.Opening && request.newStatus == ElevatorStatus.Opened)
        {
            ElevatorStatusUpdateResponse response = new ElevatorStatusUpdateResponse();
            response.resultCode = ResultCode.Succeeded;
            response.elevatorData = elevatorData.DeepCopy();

            SendElevatorStatusUpdateResponse(response);
        }
    }

    void SendElevatorStatusUpdateResponse(ElevatorStatusUpdateResponse response)
    {
        elevatorController?.OnGetElevatorStatusUpdateResponse(response);
        Logger.Log(Logger.kTagRes, "SendElevatorStatusUpdateResponse " + JsonUtility.ToJson(response));
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
            elevatorInterfaceController.Show(OnGetCallRequest);
        }
    }
}

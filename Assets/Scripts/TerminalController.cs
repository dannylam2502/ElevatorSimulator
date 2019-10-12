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

        SendElevatorData();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGetFloorRequest(FloorRequest rq)
    {
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
            rs.resultCode = isSuccess ? ResultCode.FloorRequestSucceeded : ResultCode.Failed;

            // Send Response to Floor
            SendFloorResponse(rs);
            Logger.Log(Logger.kTagRes, JsonUtility.ToJson(rs));

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
            Logger.Log(Logger.kTagRes, JsonUtility.ToJson(rs));
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
        HandleCallRequest(request);
    }

    void HandleCallRequest(CallRequest request)
    {
        // Handle it

        // Send response
        CallResponse response = new CallResponse();
        response.levelRequested = request.level;
        response.resultCode = ResultCode.CallRequestSucceeded;

        SendCallResponse(response);
        Logger.Log(Logger.kTagRes, JsonUtility.ToJson(response));
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

            }
            else
            {
                // Cur floor has request
                elevatorData.status = ElevatorStatus.Opening;

                ElevatorUpdateResponse response = new ElevatorUpdateResponse();
                response.updatedElevatorData = elevatorData.DeepCopy();
                response.destinationY = GetFloorController(floorData.level).GetFittedElevatorAnchoredPositionY();

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
        Logger.Log(Logger.kTagRes, JsonUtility.ToJson(response));
    }

    void SendElevatorUpdateResponse(ElevatorUpdateResponse response)
    {
        elevatorController?.OnGetElevatorUpdateResponse(response);
        Logger.Log(Logger.kTagRes, JsonUtility.ToJson(response));
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

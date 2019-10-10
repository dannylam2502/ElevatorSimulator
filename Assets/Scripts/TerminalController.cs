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

    Dictionary<uint, FloorController> dictFloor;

    // Start is called before the first frame update
    void Start()
    {
        uint numFloor = 100;
        dictFloor = new Dictionary<uint, FloorController>(100);

        for (uint i = 0; i < numFloor; i++)
        {
            GameObject floor = Instantiate(pfFloor, layoutFloor.transform);
            FloorController component = floor.GetComponent<FloorController>();
            if (component)
            {
                uint floorLevel = numFloor - i;
                component.SetFloorLevel(numFloor - i);
                component.SetFloorRequestCallback(OnGetFloorRequest);
                dictFloor.Add(floorLevel, component);
            }
        }

        // Prevent top floor going up
        FloorController topFloor = GetFloorController(numFloor);
        if (topFloor)
        {
            topFloor.SetDirectionInteractable(false, Direction.Up);
        }

        // Prevent bottom floor going down
        FloorController bottomFloor = GetFloorController(1);
        if (bottomFloor)
        {
            bottomFloor.SetDirectionInteractable(false, Direction.Down);
        }
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
        // handle it

        // Send Response
        FloorResponse rs = new FloorResponse();
        rs.level = rq.level;
        rs.direction = rq.direction;
        rs.resultCode = ResultCode.FloorRequestSucceed;

        SendFloorResponse(rs);
        Logger.Log(Logger.kTagRes, JsonUtility.ToJson(rs));
    }

    void SendFloorResponse(FloorResponse rs)
    {
        FloorController component = GetFloorController(rs.level);
        if (component)
        {
            component.OnGetResponse(rs);
        }
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
        response.resultCode = ResultCode.CallRequestSucceed;

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
}

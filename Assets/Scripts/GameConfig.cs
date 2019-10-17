using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public static uint NumFloor = 20;
    public static uint NumElevator = 10;
    public const uint kBottomFloor = 1;
    public const float kTimeWaitBeforeElevatorSendRequest = 1.0f;
    public const uint kFloorInvalid = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetInputData(uint numFloor, uint numElevator)
    {
        NumFloor = numFloor;
        NumElevator = numElevator;
    }

    public static uint GetTopFloor()
    {
        return NumFloor;
    }

    public static uint GetBottomFloor()
    {
        return kBottomFloor;
    }
}

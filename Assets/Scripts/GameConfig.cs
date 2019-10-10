using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public static uint NumFloor = 2;
    public static uint NumElevator = 1;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationController : MonoBehaviour
{
    [SerializeField]
    HorizontalLayoutGroup terminalLayout;
    [SerializeField]
    InputField inputFieldTerminalNum;
    [SerializeField]
    GameObject pfTerminal;
    [SerializeField]
    ElevatorInterfaceController elevatorInterfaceController;
    [SerializeField]
    float SpeedSendRequest = 0.5f;
    [SerializeField]
    bool IsStimulating = true;

    List<TerminalController> terminalControllers;

    private void Awake()
    {
        terminalControllers = new List<TerminalController>((int)GameConfig.NumElevator);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RoutineCreateTerminal());
    }

    IEnumerator RoutineCreateTerminal()
    {
        for (uint i = 1; i <= GameConfig.NumElevator; i++)
        {
            GameObject terminal = Instantiate(pfTerminal, terminalLayout.transform);
            terminal.name = "Terminal" + i.ToString();
            TerminalController component = terminal.GetComponent<TerminalController>();
            if (component)
            {
                component.SetData(i);
                component.SetElevatorInterface(elevatorInterfaceController);
                terminalControllers.Add(component);
            }
            yield return new WaitForSeconds(0.2f);
        }

        StartCoroutine(RoutineStimulation());
    }

    IEnumerator RoutineStimulation()
    {
        while (true)
        {
            if (IsStimulating)
            {
                int randomTerminal = Random.Range(0, (int)GameConfig.NumElevator);
                int randomFloor = Random.Range((int)GameConfig.GetBottomFloor(), (int)GameConfig.GetTopFloor() + 1);
                int randomPlace = Random.Range(1, 3); // Floor or elevator
                if (randomPlace == 1) // floor request elevator
                {
                    // then up or down or both?
                    int randomDirection = Random.Range(0, 2);
                    Direction d = Direction.None;
                    if (randomDirection == 0)
                    {
                        d = Direction.Up;
                    }
                    if (randomDirection == 1)
                    {
                        d = Direction.Down;
                    }
                    if (randomDirection == 2)
                    {
                        d = Direction.Both;
                    }
                    CallElevatorRequest request = new CallElevatorRequest((uint)randomFloor, d);
                    terminalControllers[randomTerminal].FakeOnGetCallElevatorRequest(request);
                }
                else if (randomPlace == 2) // elevator call floor
                {
                    CallFloorRequest request = new CallFloorRequest((uint)randomFloor);
                    terminalControllers[randomTerminal].FakeOnGetCallFloorRequest(request);
                }
            }
            
            yield return new WaitForSeconds(SpeedSendRequest);
        }
    }

    public void OnUserChangedStimulating(bool value)
    {
        IsStimulating = value;
    }
}

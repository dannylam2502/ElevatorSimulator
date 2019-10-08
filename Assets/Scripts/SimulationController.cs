using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationController : MonoBehaviour
{
    [SerializeField]
    HorizontalLayoutGroup terminalLayout; 

    [SerializeField]
    GameObject pfTerminal;

    [SerializeField]
    ElevatorInterfaceController elevatorInterfaceController;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RoutineCreateTerminal());
    }

    IEnumerator RoutineCreateTerminal()
    {
        uint numTerminal = 10;
        for (uint i = 0; i < numTerminal; i++)
        {
            GameObject terminal = Instantiate(pfTerminal, terminalLayout.transform);
            terminal.name = "Terminal" + i.ToString();
            TerminalController component = terminal.GetComponent<TerminalController>();
            if (component)
            {
                component.SetElevatorInterface(elevatorInterfaceController);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

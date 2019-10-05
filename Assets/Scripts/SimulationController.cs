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
            GameObject floor = Instantiate(pfTerminal, terminalLayout.transform);
            floor.name = "Terminal" + i.ToString();
            yield return new WaitForSeconds(0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

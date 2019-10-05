using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalController : MonoBehaviour
{
    [SerializeField]
    VerticalLayoutGroup floorLayout;

    [SerializeField]
    GameObject pfFloor;

    // Start is called before the first frame update
    void Start()
    {
        uint numFloor = 100;
        for (uint i = 0; i < numFloor; i++)
        {
            GameObject floor = Instantiate(pfFloor, floorLayout.transform);
            FloorController component = floor.GetComponent<FloorController>();
            if (component)
            {
                component.SetFloorLevel(numFloor - i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

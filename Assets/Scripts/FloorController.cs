using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorController : MonoBehaviour
{
    private uint floorLevel;

    [SerializeField]
    Text txtFloorLevel;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFloorLevel(uint level)
    {
        floorLevel = level;
        txtFloorLevel.text = floorLevel.ToString();
    }
}

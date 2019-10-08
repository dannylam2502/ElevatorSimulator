using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallButtonController : MonoBehaviour
{
    [SerializeField]
    Text txtFloorLevel;

    [ShowOnly]
    [SerializeField]
    uint floorLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {

    }

    public void SetFloorLevel(uint level)
    {
        floorLevel = level;
        txtFloorLevel.text = level.ToString();
    }
}

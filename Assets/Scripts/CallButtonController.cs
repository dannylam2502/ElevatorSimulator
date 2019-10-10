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

    [SerializeField]
    Button btnCall;

    public delegate void OnClickCallButtonCallback(uint level);
    OnClickCallButtonCallback onClickCallback;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOnClickCallback(OnClickCallButtonCallback callback)
    {
        onClickCallback = callback;
    }

    public void OnClick()
    {
        SendRequest();
    }

    void SendRequest()
    {
        onClickCallback?.Invoke(floorLevel);
    }

    public void SetFloorLevel(uint level)
    {
        floorLevel = level;
        txtFloorLevel.text = level.ToString();
    }

    public void SetSelected(bool isSelected)
    {
        btnCall.interactable = !isSelected;
    }
}

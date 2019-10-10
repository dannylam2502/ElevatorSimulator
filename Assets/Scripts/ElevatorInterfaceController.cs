﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorInterfaceController : MonoBehaviour
{
    [SerializeField]
    GameObject content;
    [SerializeField]
    GameObject pfCallBtn;
    [SerializeField]
    GridLayoutGroup layoutFloorBtn;

    OnCallRequestCallback onCallRequestCallback;

    Dictionary<uint, CallButtonController> dictCallBtnController;
    
    // Start is called before the first frame update
    void Start()
    {
        dictCallBtnController = new Dictionary<uint, CallButtonController>();
        for (uint i = 0; i < 100; i++)
        {
            GameObject floorBtn = Instantiate(pfCallBtn, layoutFloorBtn.transform);
            CallButtonController component = floorBtn.GetComponent<CallButtonController>();
            if (component)
            {
                component.SetFloorLevel(i + 1);
                component.SetOnClickCallback(OnClickCallButton);
                dictCallBtnController.Add(i + 1, component);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(OnCallRequestCallback callback)
    {
        content.SetActive(true);
        onCallRequestCallback = callback;
    }

    public void Hide()
    {
        content.SetActive(false);
    }

    public void OnClickBtnClose()
    {
        Hide();
    }

    void OnClickCallButton(uint level)
    {
        SendCallRequest(level);
    }

    public void SetCallRequestCallback(OnCallRequestCallback callback)
    {
        onCallRequestCallback = callback;
    }

    public void SendCallRequest(uint level)
    {
        CallRequest request = new CallRequest();
        request.level = level;
        onCallRequestCallback?.Invoke(request);
    }

    public void OnGetCallResponse(CallResponse response)
    {
        if (response.resultCode == ResultCode.CallRequestSucceed)
        {
            CallButtonController controller = GetButtonController(response.levelRequested);
            if (controller)
            {
                controller.SetSelected(true);
            }
        }
    }


    /// <summary>
    /// Get FloorController by level, return null if key not found
    /// </summary>
    /// <param name="level">key floor level</param>
    /// <returns></returns>
    CallButtonController GetButtonController(uint level)
    {
        if (dictCallBtnController.ContainsKey(level))
        {
            return dictCallBtnController[level];
        }
        return null;
    }
}
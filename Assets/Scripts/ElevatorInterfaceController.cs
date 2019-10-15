using System.Collections;
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

    OnCallFloorRequestCallback onCallRequestCallback;

    Dictionary<uint, CallButtonController> dictCallBtnController;
    
    // Start is called before the first frame update
    void Start()
    {
        dictCallBtnController = new Dictionary<uint, CallButtonController>();
        for (uint i = 0; i < GameConfig.NumFloor; i++)
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

    public void Show(OnCallFloorRequestCallback callback)
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
        SendCallFloorRequest(level);
    }

    public void SetCallRequestCallback(OnCallFloorRequestCallback callback)
    {
        onCallRequestCallback = callback;
    }

    public void SendCallFloorRequest(uint level)
    {
        CallFloorRequest request = new CallFloorRequest(level);
        onCallRequestCallback?.Invoke(request);
    }

    public void OnGetCallFloorResponse(CallFloorResponse response)
    {
        if (response.resultCode == ResultCode.Succeeded)
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

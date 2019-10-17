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
    [SerializeField]
    Text txtTitle;

    OnCallFloorRequestCallback onCallRequestCallback;

    Dictionary<uint, CallButtonController> dictCallBtnController;
    uint curTerminalNum; // We only use 1 UI for displaying this in entire game, so we need to know which terminal it's showing
    
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public bool IsShow()
    {
        return content.activeInHierarchy;
    }

    public uint GetCurTerminalNum()
    {
        return curTerminalNum;
    }

    public void Show(uint terminalNum, HashSet<uint> listFloorsRequesting, Vector2 position, OnCallFloorRequestCallback callback)
    {
        curTerminalNum = terminalNum;
        content.SetActive(true);
        UpdateUI(listFloorsRequesting);
        onCallRequestCallback = callback;

        // prevent out of camera
        if (position.y > 0)
        {
            position.y = 0;
        }

        if (position.x < 0)
        {
            position.x = 0;
        }
        transform.position = position;
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
            UpdateUI(response.listFloorsRequesting);
        }
    }

    void UpdateUI(HashSet<uint> listFloorsRequesting)
    {
        foreach (var item in listFloorsRequesting)
        {
            CallButtonController controller = GetButtonController(item);
            controller.SetSelected(true);
        }
        txtTitle.text = string.Format("Terminal {0}", curTerminalNum);
    }

    public void OnGetElevatorArrivedResponse(ElevatorArrivedResponse response)
    {
        GetButtonController(response.floorData.level)?.SetSelected(false);
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

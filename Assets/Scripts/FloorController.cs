using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorController : MonoBehaviour
{
    [SerializeField]
    Text txtFloorLevel;
    [SerializeField]
    Button btnUp;
    [SerializeField]
    Button btnDown;

    FloorData floorData;

    OnCallElevatorRequestCallback onCallElevatorRequestCallback;

    public static Color CannotBeUsedColor = Color.grey;
    public static Color CanBeUsedColor = Color.white;

    public const float FloorOffset = 25.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFloorData(FloorData data)
    {
        floorData = data;
    }

    public void UpdateUI()
    {
        btnUp.image.color = CannotBeUsedColor;
        btnDown.image.color = CannotBeUsedColor;
        switch (floorData.requestableDirection)
        {
            case Direction.None:
                break;
            case Direction.Up:
                btnUp.image.color = CanBeUsedColor;
                break;
            case Direction.Down:
                btnDown.image.color = CanBeUsedColor;
                break;
            case Direction.Both:
                btnUp.image.color = CanBeUsedColor;
                btnDown.image.color = CanBeUsedColor;
                break;
            default:
                break;
        }

        btnUp.interactable = true;
        btnDown.interactable = true;

        switch (floorData.status)
        {
            case FloorStatus.Waiting:
                break;
            case FloorStatus.RequestingUp:
                btnUp.interactable = false;
                break;
            case FloorStatus.RequestingDown:
                btnDown.interactable = false;
                break;
            case FloorStatus.RequestingUpAndDown:
                btnUp.interactable = false;
                btnDown.interactable = false;
                break;
            default:
                break;
        }

        txtFloorLevel.text = floorData.level.ToString();
    }

    public void OnClickBtnUp()
    {
        SendCallElevatorRequest(Direction.Up);
    }

    public void OnClickBtnDown()
    {
        SendCallElevatorRequest(Direction.Down);
    }

    void SendCallElevatorRequest(Direction direction)
    {
        CallElevatorRequest request = new CallElevatorRequest(floorData.level, direction);
        onCallElevatorRequestCallback?.Invoke(request);
    }

    public void SetCallElevatorRequestCallback(OnCallElevatorRequestCallback cb)
    {
        onCallElevatorRequestCallback = cb;
    }

    public void OnGetCallElevatorResponse(CallElevatorResponse response)
    {
        if (response.resultCode == ResultCode.Succeeded)
        {
            floorData = response.floorData;
            UpdateUI();
        }
    }

    public float GetFittedElevatorAnchoredPositionY()
    {
        return GetComponent<RectTransform>().anchoredPosition.y + FloorOffset;
    }

    public void OnGetElevatorArrivedResponse(ElevatorArrivedResponse response)
    {
        floorData = response.floorData;
        UpdateUI();
    }
}

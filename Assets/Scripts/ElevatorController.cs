using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorController : MonoBehaviour
{
    [SerializeField]
    private Text txtCurFloor;
    [SerializeField]
    private Text txtCurStatus;
    [ShowOnly]
    [SerializeField]
    private float curDestinationY;
    [SerializeField]
    private float speed = 30.0f;
    [SerializeField]
    private DoorController doorController;

    ElevatorData elevatorData;

    // Use this for displaying UI and handle events
    ElevatorData prevElevatorData;

    Coroutine coroutineElevator;

    public const float DefaultAnchoredPositionY = -55.0f; // Top floor

    OnElevatorStatusUpdateRequestCallback onElevatorStatusUpdateCallback;
    OnUpdateElevatorPositionCallback onUpdateElevatorPositionCallback;

    // cached
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetAtDefaultAnchoredPositionY()
    {
        SetAnchoredPositionY(DefaultAnchoredPositionY);
    }

    public void SetAnchoredPositionY(float posY)
    {
        Vector3 position = rectTransform.localPosition;
        position.y = posY;
        rectTransform.anchoredPosition = position;
    }

    public void Stop()
    {

    }

    public void OnGetElevatorDataResponse(GetElevatorDataResponse response)
    {
        elevatorData = response.elevatorData;
        UpdateUI();
    }

    public void OnGetElevatorUpdateResponse(UpdateElevatorResponse response)
    {
        if (elevatorData != null) // If null, it means haven't got elevatorData before
        {
            prevElevatorData = elevatorData;
            elevatorData = response.updatedElevatorData;
            curDestinationY = response.destinationY;

            if (prevElevatorData.status == ElevatorStatus.Waiting)
            {
                if (elevatorData.status == ElevatorStatus.Opening)
                {
                    doorController.Open(OnDoorOpened);
                }

                if (elevatorData.status == ElevatorStatus.MovingDown)
                {
                    // Run coroutine
                    if (coroutineElevator != null)
                    {
                        StopCoroutine(coroutineElevator);
                    }
                    coroutineElevator = StartCoroutine(RoutineElevator());
                }
            }

            UpdateUI();
        }
        else
        {
            Logger.Log(Logger.kTagError, "Elevator Haven't got elevatorData");
        }
    }

    public IEnumerator RoutineElevator()
    {
        Vector2 temp = rectTransform.anchoredPosition;
        while (rectTransform.anchoredPosition.y > curDestinationY)
        {
            temp = rectTransform.anchoredPosition;
            temp.y -= speed * Time.deltaTime;
            rectTransform.anchoredPosition = temp;

            UpdateElevatorPositionRequest request = new UpdateElevatorPositionRequest();
            request.positionY = rectTransform.anchoredPosition.y;
            onUpdateElevatorPositionCallback?.Invoke(request);
            yield return new WaitForEndOfFrame();
        }
        temp.y = curDestinationY;
        rectTransform.anchoredPosition = temp;
        
        yield return new WaitForEndOfFrame();
    }

    void OnDoorOpened()
    {
        SendStatusUpdateRequest(ElevatorStatus.Opened);
    }

    public void SetOnElevatorStatusUpdateCallback(OnElevatorStatusUpdateRequestCallback callback)
    {
        onElevatorStatusUpdateCallback = callback;
    }

    public void SendStatusUpdateRequest(ElevatorStatus newStatus)
    {
        UpdateElevatorStatusRequest request = new UpdateElevatorStatusRequest();
        request.newStatus = newStatus;
        onElevatorStatusUpdateCallback?.Invoke(request);
    }

    public void OnGetElevatorStatusUpdateResponse(UpdateElevatorStatusResponse response)
    {
        if (response.resultCode == ResultCode.Succeeded)
        {
            elevatorData = response.elevatorData;
        }
    }

    public void SetUpdateElevatorPositionCallback(OnUpdateElevatorPositionCallback callback)
    {
        onUpdateElevatorPositionCallback = callback;
    }

    void UpdateUI()
    {
        txtCurFloor.text = elevatorData.curFloorLevel.ToString();
        txtCurStatus.text = ConvertElevatorStatusToString(elevatorData.status);
    }

    public void OnGetUpdateElevatorPositionResponse(UpdateElevatorPositionResponse response)
    {
        if (response.resultCode == ResultCode.Succeeded)
        {
            elevatorData.curFloorLevel = response.newLevel;
            UpdateUI();
        }
    }

    public static string ConvertElevatorStatusToString(ElevatorStatus status)
    {
        switch (status)
        {
            case ElevatorStatus.Waiting:
                return "Waiting";
            case ElevatorStatus.Opening:
                return "Opening";
            case ElevatorStatus.Opened:
                return "Opened";
            case ElevatorStatus.Closing:
                return "Closing";
            case ElevatorStatus.Closed:
                return "Closed";
            case ElevatorStatus.MovingUp:
                return "Moving Up";
            case ElevatorStatus.MovingDown:
                return "Moving Down";
            default:
                break;
        }
        return "";
    }
}

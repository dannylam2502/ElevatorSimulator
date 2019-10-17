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
    [SerializeField]
    private float speed = 50.0f;
    [SerializeField]
    private DoorController doorController;

    ElevatorData elevatorData;

    // Use this for displaying UI and handle events
    ElevatorData prevElevatorData;

    Coroutine coroutineMovingElevator;

    public const float DefaultAnchoredPositionY = -55.0f; // Top floor

    OnUpdateElevatorStatusRequestCallback onElevatorStatusUpdateCallback;
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
        // Stop running coroutine
        if (coroutineMovingElevator != null)
        {
            StopCoroutine(coroutineMovingElevator);
        }

        if (elevatorData != null) // If null, it means haven't got elevatorData before
        {
            prevElevatorData = elevatorData;
            elevatorData = response.updatedElevatorData;

            if (elevatorData.status == ElevatorStatus.Opening)
            {
                doorController.Open(OnDoorOpened);
            }

            if (elevatorData.status == ElevatorStatus.MovingDown || elevatorData.status == ElevatorStatus.MovingUp)
            {
                // Run coroutine
                coroutineMovingElevator = StartCoroutine(RoutineMoving(response.destinationY));
            }

            if (elevatorData.status == ElevatorStatus.Arrived)
            {
                SendUpdateElevatorStatusRequest(ElevatorStatus.Opening);
            }

            if (elevatorData.status == ElevatorStatus.Closing)
            {
                doorController.Close(OnDoorClosed);
            }

            UpdateUI();
        }
        else
        {
            Logger.Log(Logger.kTagError, "Elevator Haven't got elevatorData");
        }
    }

    public IEnumerator RoutineMoving(float destinationY)
    {
        bool isFinished = false;
        Vector2 temp = rectTransform.anchoredPosition;
        while (isFinished == false)
        {
            temp = rectTransform.anchoredPosition;
            if (elevatorData.status == ElevatorStatus.MovingDown)
            {
                temp.y -= speed * Time.deltaTime;
                if (temp.y <= destinationY)
                {
                    isFinished = true;
                }
            }
            else if (elevatorData.status == ElevatorStatus.MovingUp)
            {
                temp.y += speed * Time.deltaTime;
                if (temp.y >= destinationY)
                {
                    isFinished = true;
                }
            }

            rectTransform.anchoredPosition = temp;

            UpdateElevatorPositionRequest request = new UpdateElevatorPositionRequest(rectTransform.anchoredPosition.y);
            onUpdateElevatorPositionCallback?.Invoke(request);
            yield return new WaitForEndOfFrame();
        }
        temp.y = destinationY;
        rectTransform.anchoredPosition = temp;
        
        yield return new WaitForEndOfFrame();
    }

    void OnDoorOpened()
    {
        SendUpdateElevatorStatusRequest(ElevatorStatus.Opened);
    }

    void OnDoorClosed()
    {
        SendUpdateElevatorStatusRequest(ElevatorStatus.Closed);
    }

    public void SetOnElevatorStatusUpdateCallback(OnUpdateElevatorStatusRequestCallback callback)
    {
        onElevatorStatusUpdateCallback = callback;
    }

    /// <summary>
    /// Run a coroutine to send request update elevator status
    /// </summary>
    /// <param name="newStatus">New status of elevator</param>
    /// <param name="delayTime">Time delay, in seconds</param>
    void SendUpdateElevatorStatusRequest(ElevatorStatus newStatus, float delayTime = GameConfig.kTimeWaitBeforeElevatorSendRequest)
    {
        StartCoroutine(RoutineSendStatusUpdateRequest(newStatus, delayTime));
    }

    public IEnumerator RoutineSendStatusUpdateRequest(ElevatorStatus newStatus, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        UpdateElevatorStatusRequest request = new UpdateElevatorStatusRequest(newStatus);
        onElevatorStatusUpdateCallback?.Invoke(request);
    }

    public void OnGetElevatorStatusUpdateResponse(UpdateElevatorStatusResponse response)
    {
        if (response.resultCode == ResultCode.Succeeded)
        {
            elevatorData.status = response.curStatus;

            if (response.curStatus == ElevatorStatus.Opened)
            {
                // Assume that every passenger goes out in instant, now we need to close it.
                SendUpdateElevatorStatusRequest(ElevatorStatus.Closing);
            }
            UpdateUI();
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
            case ElevatorStatus.Arrived:
                return "Arrived";
            default:
                break;
        }
        return "";
    }
}

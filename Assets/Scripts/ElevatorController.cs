using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [ShowOnly]
    [SerializeField]
    private Vector3 curDestination;
    [SerializeField]
    private float speed = 20.0f;
    [SerializeField]
    private Direction direction;
    [SerializeField]
    private DoorController doorController;

    ElevatorData elevatorData;

    // Use this for displaying UI and handle events
    ElevatorData prevElevatorData;

    Coroutine coroutineElevator;

    public const float DefaultAnchoredPositionY = -55.0f; // Top floor

    OnElevatorStatusUpdateRequestCallback onElevatorStatusUpdateCallback;

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

    public void SetDestination(Vector3 position)
    {
        curDestination = position;
    }

    public void SetDirection(Direction d)
    {
        direction = d;
    }

    public void Stop()
    {

    }

    public void Move(Direction d)
    {

    }

    public void OnGetElevatorDataResponse(ElevatorDataResponse response)
    {
        elevatorData = response.elevatorData;
    }

    public void OnGetElevatorUpdateResponse(ElevatorUpdateResponse response)
    {
        if (elevatorData != null) // If null, it means haven't got elevatorData before
        {
            prevElevatorData = elevatorData;
            elevatorData = response.updatedElevatorData;

            // Run coroutine
            if (coroutineElevator != null)
            {
                StopCoroutine(coroutineElevator);
            }
            coroutineElevator = StartCoroutine(RoutineElevator());
        }
        else
        {
            Logger.Log(Logger.kTagError, "Elevator Haven't got elevatorData");
        }
    }

    public IEnumerator RoutineElevator()
    {
        if (prevElevatorData.status == ElevatorStatus.Waiting)
        {
            if (elevatorData.status == ElevatorStatus.Opening)
            {
                doorController.Open(OnDoorOpened);
            }

            if (elevatorData.status == ElevatorStatus.MovingDown)
            {
                
            }
        }
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
        ElevatorStatusUpdateRequest request = new ElevatorStatusUpdateRequest();
        request.newStatus = newStatus;
        onElevatorStatusUpdateCallback?.Invoke(request);
    }

    public void OnGetElevatorStatusUpdateResponse(ElevatorStatusUpdateResponse response)
    {
        if (response.resultCode == ResultCode.Succeeded)
        {
            elevatorData = response.elevatorData;
        }
    }
}

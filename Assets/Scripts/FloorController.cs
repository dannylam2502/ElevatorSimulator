using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnFloorRequestCallback(FloorRequest request);

public class FloorController : MonoBehaviour
{
    [ShowOnly]
    private uint floorLevel;

    [SerializeField]
    Text txtFloorLevel;
    [SerializeField]
    Button btnUp;
    [SerializeField]
    Button btnDown;

    OnFloorRequestCallback callback;
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

    public uint GetFloorLevel()
    {
        return floorLevel;
    }

    public void SetDirectionInteractable(bool enable, Direction direction)
    {
        if (direction == Direction.Down)
        {
            btnDown.interactable = enable;
        }
        else if (direction == Direction.Up)
        {
            btnUp.interactable = enable;
        }
    }

    public void OnClickBtnUp()
    {
        SendRequest(Direction.Up);
    }

    public void OnClickBtnDown()
    {
        SendRequest(Direction.Down);
    }

    void SendRequest(Direction d)
    {
        FloorRequest rq = new FloorRequest();
        rq.level = floorLevel;
        rq.direction = d;

        callback?.Invoke(rq);

        Logger.Log(Logger.kTagReq, JsonUtility.ToJson(rq));
    }

    public void SetFloorRequestCallback(OnFloorRequestCallback cb)
    {
        callback = cb;
    }

    public void OnGetResponse(FloorResponse response)
    {
        if (response.resultCode == ResultCode.FloorRequestSucceed)
        {
            SetDirectionInteractable(false, response.direction);
        }
    }
}

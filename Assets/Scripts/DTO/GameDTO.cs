using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultCode
{
    None,
    Failed,
    FloorRequestSucceed,
}

public class GameDTO
{

}

[SerializeField]
public class FloorRequest
{
    public uint level;
    public Direction direction;
}

[SerializeField]
public class FloorResponse
{
    public ResultCode resultCode;
    public uint level;
    public Direction direction;
}

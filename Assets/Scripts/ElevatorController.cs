using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down
}

public class ElevatorController : MonoBehaviour
{
    [SerializeField]
    private Vector3 curDestination;
    [SerializeField]
    private float speed = 20.0f;
    [SerializeField]
    private Direction direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDestination(Vector3 position)
    {
        curDestination = position;
    }

    public void SetDirection(Direction d)
    {
        direction = d;
    }
}

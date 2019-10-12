using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    Image imgLeft;
    [SerializeField]
    Image imgRight;
   
    Animator animator;

    Callback onDoorOpened;
    Callback onDoorClosed;

    public const string kStateKey = "State";
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            Open(null);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Close(null);
        }
#endif
    }

    public void Open(Callback onDoorOpenedCallback)
    {
        animator.SetInteger(kStateKey, 1);
        onDoorOpened = onDoorOpenedCallback;
    }

    public void Close(Callback onDoorClosedCallback)
    {
        animator.SetInteger(kStateKey, 2);
        onDoorClosed = onDoorClosedCallback;
    }

    public void Idle()
    {
        animator.SetInteger(kStateKey, 0);
    }

    void OnEndOpenAnimation()
    {
        onDoorOpened?.Invoke();
    }

    void OnEndCloseAnimation()
    {
        onDoorClosed?.Invoke();
    }
}

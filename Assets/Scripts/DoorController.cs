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
    public const int kStateIdle = 0;
    public const int kStateOpening = 1;
    public const int kStateClosing = 2;
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
        animator.SetInteger(kStateKey, kStateOpening);
        onDoorOpened = onDoorOpenedCallback;
    }

    public void Close(Callback onDoorClosedCallback)
    {
        animator.SetInteger(kStateKey, kStateClosing);
        onDoorClosed = onDoorClosedCallback;
    }

    public void Idle()
    {
        animator.SetInteger(kStateKey, kStateIdle);
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

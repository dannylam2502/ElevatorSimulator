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
    // Start is called before the first frame update
    void Start()
    {
        for (uint i = 0; i < 100; i++)
        {
            GameObject floorBtn = Instantiate(pfCallBtn, layoutFloorBtn.transform);
            CallButtonController component = floorBtn.GetComponent<CallButtonController>();
            if (component)
            {
                component.SetFloorLevel(i + 1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        content.SetActive(true);
    }

    public void Hide()
    {
        content.SetActive(false);
    }

    public void OnClickBtnClose()
    {
        Hide();
    }
}

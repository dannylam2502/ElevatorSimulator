using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float speed = 20f;
    Camera cameraRef;

    Vector3 newCamPos;
    Vector3 screenCamPoint;

    Rect edge = Rect.zero;

    float mouseX, mouseY;

    void Start()
    {
        cameraRef = GetComponent<Camera>();
        newCamPos = cameraRef.transform.position;

        edge.x = Screen.width * 0.2f;
        edge.width = Screen.width * 0.6f;
        edge.y = Screen.height * 0.2f;
        edge.height = Screen.height * 0.6f;
    }

    //void Update()
    //{
    //    if (cameraRef != null)
    //    {
    //        mouseX = Input.mousePosition.x;
    //        mouseY = Input.mousePosition.y;

    //        if (mouseY > Screen.height * 1.2) // cheat out of screen, I'm working in editor
    //        {
    //            return;
    //        }

    //        //Debug.LogFormat("MouseX = {0}, MouseY = {1}", Input.mousePosition.x, Input.mousePosition.y);

    //        if (mouseX < edge.x)
    //        {
    //            newCamPos.x -= Time.deltaTime * speed;
    //        }
    //        else if (mouseX > edge.xMax)
    //        {
    //            newCamPos.x += Time.deltaTime * speed;
    //        }

    //        if (mouseY < edge.y)
    //        {
    //            newCamPos.y -= Time.deltaTime * speed;
    //        }
    //        else if (mouseY > edge.yMax)
    //        {
    //            newCamPos.y += Time.deltaTime * speed;
    //        }
    //        cameraRef.transform.position = newCamPos;
    //    }
    //}
}

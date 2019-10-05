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

        edge.x = Screen.width * 0.35f;
        edge.width = Screen.width * 0.3f;
        edge.y = Screen.height * 0.35f;
        edge.height = Screen.height * 0.3f;
    }

    void LateUpdate()
    {
        if (cameraRef != null)
        {
            mouseX = Input.mousePosition.x;
            mouseY = Input.mousePosition.y;

            Debug.LogFormat("MouseX = {0}, MouseY = {1}", Input.mousePosition.x, Input.mousePosition.y);

            if (mouseX < edge.x)
            {
                newCamPos.x -= Time.deltaTime * speed;
            }
            else if (mouseX > edge.xMax)
            {
                newCamPos.x += Time.deltaTime * speed;
            }

            if (mouseY < edge.y)
            {
                newCamPos.y -= Time.deltaTime * speed;
            }
            else if (mouseY > edge.yMax)
            {
                newCamPos.y += Time.deltaTime * speed;
            }
            cameraRef.transform.position = newCamPos;
        }
    }
}

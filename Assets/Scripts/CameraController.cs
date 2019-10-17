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

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
    }

    public void OnClickResetCamera()
    {
        Vector3 position = cameraRef.transform.position;
        position.x = 0;
        position.y = 0;
        cameraRef.transform.position = position;
    }
}

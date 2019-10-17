using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float speed = 20f;


    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0.0f, -speed * Time.deltaTime, 0.0f));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0.0f, speed * Time.deltaTime, 0.0f));
        }
    }

    public void OnClickResetCamera()
    {
        transform.position = new Vector3(0.0f, 0.0f, transform.position.z);
    }
}

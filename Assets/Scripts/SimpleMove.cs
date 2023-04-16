using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    [SerializeField] float speed, slowSpeedMultiplier, fastSpeedMultiplier, rotateSpeed, sensitivity;

    Vector3 position;
    float horizontalRotation;
    float verticalRotation;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        horizontalRotation = 0;
        verticalRotation = 0;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            position += transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            position -= transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            position += transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            position -= transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            position += transform.up * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            position -= transform.up * speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= fastSpeedMultiplier;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            speed *= slowSpeedMultiplier;
        }

        verticalRotation -= Input.GetAxis("Mouse Y") * sensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);

        horizontalRotation += Input.GetAxis("Mouse X") * sensitivity;

        //Debug.Log(verticalRotation);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, position, 0.15f);

        transform.eulerAngles = new Vector3(verticalRotation, horizontalRotation, transform.eulerAngles.z);
    }
}

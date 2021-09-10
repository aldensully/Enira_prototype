using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float sensX = 100f;
    float sensY = 100f;

    Camera cam;
    WallRun wallRun;

    public float mouseX;
    public float mouseY;
    float mult = 0.001f;
    public float tilt;
    public float cameraTiltTime;
    float xRot;
    float yRot;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        wallRun = GetComponent<WallRun>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        MyInput();
        cam.transform.localRotation = Quaternion.Euler(xRot, 0, wallRun.tilt);
        //cam.transform.localRotation = Quaternion.Euler(xRot, 0, tilt);
        transform.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, yRot, 0));
    }
    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRot += mouseX * sensX * mult;
        xRot -= mouseY * sensY * mult;

        xRot = Mathf.Clamp(xRot, -90f, 90f);
    }
    public void Tilt(float degree)
    {
        tilt = Mathf.Lerp(tilt, degree, cameraTiltTime * Time.deltaTime);
    }
    public void TiltReset()
    {
        tilt = 0f;
    }
}

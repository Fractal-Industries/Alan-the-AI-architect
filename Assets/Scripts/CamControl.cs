using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public int CurrentCam = 0;

    float RotX, CarX;
    float RotY, CarY;
    
    [SerializeField] float Sensitivity;
    [SerializeField] float MovmentSpeed;
    [SerializeField] Transform CarCam;

    void Start()
    {
        
    }

    void Update()
    {
        if (CurrentCam == 0)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (Input.GetMouseButton(1))
            {
                RotX += Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
                RotY += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
                transform.localEulerAngles = new Vector3(-RotX, RotY, 0);
            }
            Vector3 Movment = MovmentSpeed * Time.deltaTime * new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Elevation"), Input.GetAxis("Vertical"));
            transform.Translate(Movment);
        }
        else if (CurrentCam == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CarX += Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
            CarY += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
            CarX = Mathf.Clamp(CarX, -90, 90);
            CarCam.localEulerAngles = new Vector3(-CarX, CarY, 0);
        }
    }
}

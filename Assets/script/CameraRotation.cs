using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public GameObject CharacterCamera;
    public GameObject Character;
    public int ScrollSpeed;
    public float wheel;
    public float angle;
    public float rotateSpeed;
    public float maxRotAngle;
    public float minRotAngle;
    
    public Vector3 offset;
    private void Start()
    {
        //offset = CharacterCamera.transform.position - Character.transform.position;
    }
    void Update()
    {
        CameraFOV();//·Æ¹«ºu½üÁY©ñµø¨¤
        float _mouseX = Input.GetAxis("Mouse X");
        float _mouseY = Input.GetAxis("Mouse Y");
        CameraRotate(_mouseX, _mouseY);
    }
    public void CameraFOV()
    {     
        float distance = Vector3.Distance(CharacterCamera.transform.position, Character.transform.position);
        wheel = Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        if(distance >= 45)
        {
            if (wheel < 0)
            {
                wheel = 0;
            }
        }
        if(distance <= 10)
        {
            if (wheel > 0)
            {
                wheel = 0;
            }
        }
        CharacterCamera.transform.Translate(Vector3.forward * wheel);
    }
    public void CameraRotate(float _mouseX, float _mouseY)
    {

        if (Input.GetMouseButton(1))
        {
            CharacterCamera.transform.RotateAround(Character.transform.position, Vector3.up, _mouseX * rotateSpeed);
            angle += _mouseY * rotateSpeed;
            if (angle > maxRotAngle)
            {
                angle = maxRotAngle;
                return;
            }
            if (angle < minRotAngle)
            {
                angle = minRotAngle;
                return;
            }
            CharacterCamera.transform.RotateAround(Character.transform.position, CharacterCamera.transform.right, _mouseY * rotateSpeed);
           // offset = CharacterCamera.transform.position - Character.transform.position;
        }

    }

}

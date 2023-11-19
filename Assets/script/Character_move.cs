using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Character_move : MonoBehaviour
{
    public GameObject CharacterFather;
    public GameObject Character;
    public GameObject CharacterCamera;
    Animator CharacterAnimator;
    public int RotateSpeed;
    public float MoveSpeed;
    Vector2 InputToMove;
    void Start()
    {
        CharacterAnimator = Character.GetComponent<Animator>();
    }
    void Update()
    {
        InputToMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        MovePlayerRelativeToCamera();
        RunAnimation();
    }
    void MovePlayerRelativeToCamera()
    {
        Vector3 forward = CharacterCamera.transform.forward;
        Vector3 right = CharacterCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeVerticalInput = InputToMove.y * forward;
        Vector3 forwardRelativeHorizontalInput = InputToMove.x * right;
        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + forwardRelativeHorizontalInput;
        CharacterFather.transform.Translate(cameraRelativeMovement * MoveSpeed * Time.deltaTime, Space.World);
       
        RotateToMove(forwardRelativeVerticalInput + forwardRelativeHorizontalInput);
    }
    public void RunAnimation()
    {
        if (InputToMove != Vector2.zero)
        {
            CharacterAnimator.SetBool("IsRun", true);
        }
        else
        {
            CharacterAnimator.SetBool("IsRun", false);
        }
    }
    void RotateToMove(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        Character.transform.rotation = Quaternion.Slerp(Character.transform.rotation, rotation, Time.deltaTime * RotateSpeed);
    }
}

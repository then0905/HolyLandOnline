using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//==========================================
//  創建者:    家豪
//  創建日期:  太久不可考
//  創建用途:  玩家角色移動
//==========================================
public class Character_move : MonoBehaviour
{
    public GameObject CharacterFather;
    public GameObject Character;
    public GameObject CharacterCamera;
    Animator CharacterAnimator;
    public int RotateSpeed;
    public float MoveSpeed;
    void Start()
    {
        CharacterAnimator = Character.GetComponent<Animator>();
    }
    void Update()
    {
        MovePlayerRelativeToCamera(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }

    /// <summary>
    /// 角色移動 
    /// </summary>
    public void MovePlayerRelativeToCamera(Vector2 inputToMove)
    {
        //取得角色向量
        Vector3 forward = CharacterCamera.transform.forward;
        Vector3 right = CharacterCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeVerticalInput = inputToMove.y * forward;
        Vector3 forwardRelativeHorizontalInput = inputToMove.x * right;
        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + forwardRelativeHorizontalInput;
        CharacterFather.transform.Translate(cameraRelativeMovement * MoveSpeed * Time.deltaTime, Space.World);
        RunAnimation(inputToMove);
        RotateToMove(forwardRelativeVerticalInput + forwardRelativeHorizontalInput);
    }

    /// <summary>
    /// 設定跑步動畫
    /// </summary>
    public void RunAnimation(Vector2 inputToMove)
    {
        if (SkillDisplayAction.Instance.AutoNavToTarget) return;
        if (inputToMove != Vector2.zero)
        {
            CharacterAnimator.SetBool("IsRun", true);
        }
        else
        {
            CharacterAnimator.SetBool("IsRun", false);
        }
    }

    /// <summary>
    /// 依照攝影機方向校正移動
    /// </summary>
    /// <param name="direction"></param>
    void RotateToMove(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        Character.transform.rotation = Quaternion.Slerp(Character.transform.rotation, rotation, Time.deltaTime * RotateSpeed);
    }
}

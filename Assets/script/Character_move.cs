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
    #region 全域靜態變數

    private static Character_move instance;
    public static Character_move Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Character_move>();
            }
            return instance;
        }
    }

    #endregion

    [SerializeField] private GameObject characterFather;
    /// <summary>
    /// 角色父級
    /// </summary>
    public GameObject CharacterFather
    {
        get
        {
            return characterFather;
        }
    }
    [SerializeField] private GameObject character;
    /// <summary>
    /// 角色本體(動畫 面相等等)
    /// </summary>
    public GameObject Character
    {
        get
        {
            return character;
        }
    }
    [SerializeField] private GameObject characterCamera;
    /// <summary>
    /// 角色攝影機
    /// </summary>
    public GameObject CharacterCamera
    {
        get
        {
            return characterCamera;
        }
    }
    [SerializeField] private Animator characterAnimator;
    /// <summary>
    /// 角色動畫控制器
    /// </summary>
    public Animator CharacterAnimator
    {
        get
        {
            return characterAnimator;
        }
    }
    //旋轉速度
    public int RotateSpeed;
    //是否再自動接近目標
    public bool AutoNavToTarget = false;
    //操作玩家事件
    public Action ControlCharacterEvent;
    //控制動畫速度事件
    public Action<float, string> ControCharacterAnimationEvent;

    //取得玩家速度值
    public float MoveSpeed
    {
        get
        {
            //設定玩家速度值 (移動速度基準值+裝備效果+技能效果)*(1- (2.71828^ −0.2))
            float speed = PlayerData.Speed * (float)(1 - Math.Pow(2.71828, -0.2));
            return speed;
        }
    }
    private void OnEnable()
    {
        ControCharacterAnimationEvent += CallCharacterAnimationSpeed;
    }

    private void OnDisable()
    {
        ControCharacterAnimationEvent -= CallCharacterAnimationSpeed;
    }

    void Update()
    {
        MovePlayerRelativeToCamera(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        if (Input.GetKeyDown(KeyCode.Space) && SelectTarget.Instance.Targetgameobject != null)
        {
            NormalAttackSystem.Instance.StartNormalAttack(
                CharacterFather,
               SelectTarget.Instance.Targetgameobject.gameObject);
        }
    }

    /// <summary>
    /// 角色移動 
    /// </summary>
    public void MovePlayerRelativeToCamera(Vector2 inputToMove)
    {
        if (AutoNavToTarget && inputToMove == Vector2.zero) return;
        else if (AutoNavToTarget)
        {
            AutoNavToTarget = false;
            ControlCharacterEvent?.Invoke();
        }
        //取得角色向量
        Vector3 forward = characterCamera.transform.forward;
        Vector3 right = characterCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeVerticalInput = inputToMove.y * forward;
        Vector3 forwardRelativeHorizontalInput = inputToMove.x * right;
        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + forwardRelativeHorizontalInput;
        characterFather.transform.Translate(cameraRelativeMovement * MoveSpeed, Space.World);
        RunAnimation(inputToMove);
        RotateToMove(forwardRelativeVerticalInput + forwardRelativeHorizontalInput);

    }

    /// <summary>
    /// 設定跑步動畫
    /// </summary>
    public void RunAnimation(Vector2 inputToMove)
    {
        AutoNavToTarget = false;
        ControlCharacterEvent?.Invoke();
        CharacterAnimator.SetBool("IsRun", inputToMove != Vector2.zero);
    }

    /// <summary>
    /// 依照攝影機方向校正移動
    /// </summary>
    /// <param name="direction"></param>
    void RotateToMove(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        character.transform.rotation = Quaternion.Slerp(character.transform.rotation, rotation, Time.deltaTime * RotateSpeed);
    }

    /// <summary>
    /// 設定動畫控制器時間
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="animationTrigger"></param>
    private void CallCharacterAnimationSpeed(float speed, string animationTrigger)
    {
        StartCoroutine(ControCharacterAnimationSpeed(speed, animationTrigger));
    }

    /// <summary>
    /// 修改動畫控制器時間協程
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="animationTrigger"></param>
    /// <returns></returns>
    public IEnumerator ControCharacterAnimationSpeed(float speed, string animationTrigger)
    {
        characterAnimator.SetTrigger(animationTrigger);
        //暫存動畫片段
        AnimatorStateInfo stateInfo = characterAnimator.GetCurrentAnimatorStateInfo(0);
        //暫存原始動畫速度
        float tempAnimatorSpeed = characterAnimator.speed;
        characterAnimator.speed = speed;
        while (stateInfo.IsName(animationTrigger))
        {
            yield return new WaitForEndOfFrame();
        }
        characterAnimator.speed = tempAnimatorSpeed;
    }
}

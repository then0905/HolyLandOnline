﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//==========================================
//  創建者:    家豪
//  創建日期:  太久不可考
//  創建用途:  玩家角色移動
//==========================================
public class Character_move : MonoBehaviour
{
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
    [SerializeField] private Camera characterCamera;
    /// <summary>
    /// 角色攝影機
    /// </summary>
    public Camera CharacterCamera
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
            if (characterAnimator.runtimeAnimatorController == null || (characterAnimator.runtimeAnimatorController.name != PlayerDataOverView.Instance.PlayerData_.Job))
                characterAnimator.runtimeAnimatorController = CommonFunction.LoadObject<AnimatorOverrideController>(GameConfig.AnimatorJob, PlayerDataOverView.Instance.PlayerData_.Job);
            return characterAnimator;
        }
    }

    [Header("指向技攻擊範圍")]
    public Image SkillArrowImage;               //指向技圖示
    [Header("攻擊範圍與圓圈形範圍技")]
    public Image SkillTargetCircle;             //圓圈型範圍技圖示
    public Image SkillPlayerCricle;             //攻擊範圍技圖示
    [Header("扇形攻擊範圍")]
    public Image SkillConeImage;                //扇形範圍技圖示

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
            float speed = PlayerDataOverView.Instance.PlayerData_.Speed * (float)(1 - Math.Pow(2.71828, -0.2));
            speed = Mathf.Clamp(speed, 0.01f, speed);
            return speed;
        }
    }

    /// <summary>
    /// 是否可以自由移動
    /// </summary>
    /// <returns></returns>
    private bool CanMove(Vector2 inputToMove, AnimatorStateInfo stateInfo)
    {
        return (AutoNavToTarget && inputToMove == Vector2.zero || (!stateInfo.IsName("Running") && !stateInfo.IsName("Idle")));
    }

    private void OnEnable()
    {
        ControCharacterAnimationEvent += CallCharacterAnimationSpeed;

        //設定攻擊範圍圖示引導狀態
        SkillArrowImage.GetComponent<Image>().enabled = false;
        SkillTargetCircle.GetComponent<Image>().enabled = false;
        SkillPlayerCricle.GetComponent<Image>().enabled = false;
        SkillConeImage.GetComponent<Image>().enabled = false;
        SkillController.Instance.SkillArrowImage = SkillArrowImage;
        SkillController.Instance.SkillTargetCircle = SkillTargetCircle;
        SkillController.Instance.SkillPlayerCricle = SkillPlayerCricle;
        SkillController.Instance.SkillConeImage = SkillConeImage;
    }

    private void OnDisable()
    {
        ControCharacterAnimationEvent -= CallCharacterAnimationSpeed;
    }

    void LateUpdate()
    {
        //取得玩家控制角色移動的輸出訊息(上下左右 wasd 搖桿等等)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        //當有任何移動方向的輸出訊息
        if (horizontalInput != 0 || verticalInput != 0)
        {
            //強制取消角色移動
            if (!PlayerDataOverView.Instance.MoveIsEnable.Equals(0))
            {
                CommonFunction.MessageHint("TM_MoveEnableError".GetText(), HintType.Warning);
                return;
            }
            Vector2 inputToMove = new Vector2(horizontalInput, verticalInput);
            MovePlayerRelativeToCamera(inputToMove);
        }
        //這邊 若玩家沒有輸出訊息 且並沒有追擊目標的情況下 才關閉跑步動畫
        else if (!AutoNavToTarget)
            RunAnimation(false);

        //MovePlayerRelativeToCamera(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));

        if (Input.GetKeyDown(KeyCode.Space) && SelectTarget.Instance.Targetgameobject != null)
        {
            if (SelectTarget.Instance.Targetgameobject is NpcBehavior)
            {
                NpcBehavior behavior = (NpcBehavior)SelectTarget.Instance.Targetgameobject;
                StartCoroutine(
                CommonFunction.DetectionRangeMethod(PlayerDataOverView.Instance.Povit.gameObject,
                   behavior.Povit.gameObject, 7.5f,
                    () =>
                    {
                        //開啟自動尋路
                        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = true;
                        float pursueDistance = Vector3.Distance(PlayerDataOverView.Instance.Povit.gameObject.transform.position, behavior.Povit.gameObject.transform.position);

                        if (pursueDistance > 7.5f)
                        {
                            // 取得角色面相目標的方向
                            Vector3 direction = SelectTarget.Instance.Targetgameobject.transform.position - PlayerDataOverView.Instance.CharacterMove.Character.transform.position;
                            // 鎖定y軸的旋轉 避免角色在x軸和z軸上傾斜
                            direction.y = 0;
                            // 如果 direction 的長度不為零，設定角色的朝向
                            if (direction != Vector3.zero)
                                PlayerDataOverView.Instance.CharacterMove.Character.transform.rotation = Quaternion.LookRotation(direction);

                            PlayerDataOverView.Instance.CharacterMove.RunAnimation(true);
                            PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position =
                                Vector3.MoveTowards(PlayerDataOverView.Instance.Povit.transform.position,
                                SelectTarget.Instance.Targetgameobject.Povit.position,
                                PlayerDataOverView.Instance.CharacterMove.MoveSpeed);
                        }
                    }
                    , () =>
                    {
                        PlayerDataOverView.Instance.CharacterMove.RunAnimation(false);
                        behavior.NpcInit();
                    }));
            }
            else if (SelectTarget.Instance.Targetgameobject is MonsterBehaviour)
            {
                MonsterBehaviour monsterBehaviour = (MonsterBehaviour)SelectTarget.Instance.Targetgameobject;

                NormalAttackSystem.Instance.StartNormalAttack(
                   PlayerDataOverView.Instance,
                   monsterBehaviour);
            }
            //NormalAttackSystem.Instance.StartNormalAttack(
            //    CharacterFather,
            //   SelectTarget.Instance.Targetgameobject.gameObject);
        }
    }

    /// <summary>
    /// 角色移動 
    /// </summary>
    public void MovePlayerRelativeToCamera(Vector2 inputToMove)
    {
        //暫存動畫片段
        AnimatorStateInfo stateInfo = characterAnimator.GetCurrentAnimatorStateInfo(0);

        //若玩家在導航 或是 正在進行非跑步 站立 的其他動畫時 不執行移動
        if (CanMove(inputToMove, stateInfo)) return;
        //打斷導航 恢復為 玩家控制移動
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

        RunAnimation(inputToMove != Vector2.zero);
        RotateToMove(forwardRelativeVerticalInput + forwardRelativeHorizontalInput);

    }


    /// <summary>
    ///  設定跑步動畫
    /// </summary>
    /// <param name="runStatus">設定跑步狀態</param>
    public void RunAnimation(bool runStatus)
    {
        CharacterAnimator.SetBool("IsRun", runStatus);
        if (runStatus)
            ControCharacterAnimationEvent.Invoke(MoveSpeed * 5.5f, "");
    }

    /// <summary>
    /// 依照攝影機方向校正移動
    /// </summary>
    /// <param name="direction"></param>
    private void RotateToMove(Vector3 direction)
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
        if (!string.IsNullOrEmpty(animationTrigger))
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

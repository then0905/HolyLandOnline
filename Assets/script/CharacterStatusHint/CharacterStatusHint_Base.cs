using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/10/17
//  創建用途: 角色狀態效果提示物件_基底
//==========================================

/// <summary>
/// 角色狀態效果提示接口
/// </summary>
public interface ICharacterStatus
{
    /// <summary>
    /// Buff效果名稱
    /// </summary>
    string CharacterStatusName { get; }

    /// <summary>
    /// BuffID
    /// </summary>
    string CharacterStatusID { get; }

    /// <summary>
    /// Buff內容描述
    /// </summary>
    string CharacterStatusIntro { get; }

    /// <summary>
    /// Buff類型
    /// </summary>
    string CharacterStatusType { get; }

    /// <summary>
    /// Buff持續時間
    /// </summary>
    float CharacterStatusDuration { get; }

    /// <summary>
    /// 更新狀態效果運行時間 協程 自運算使用
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    IEnumerable UpdateTimerCoroutine(float deltaTime);

    /// <summary>
    /// 更新狀態效果運行時間 方法 外部更新
    /// </summary>
    /// <param name="deltaTime"></param>
    void UpdateTimerMethod(object o, float deltaTime);

    /// <summary>
    /// 狀態效果移除
    /// </summary>
    /// <param name="deltaTime"></param>
    void RemoveCharacterStatusHint(object o, string id);
}

/// <summary>
/// 角色狀態效果提示標籤
/// </summary>
public enum CharacterStatusType
{
    /// <summary>
    /// 增益效果
    /// </summary>
    Buff,
    /// <summary>
    /// 減益效果
    /// </summary>
    DeBuff,
    /// <summary>
    /// 控制效果
    /// </summary>
    CrowdControl,
    /// <summary>
    /// 被動效果
    /// </summary>
    Passive
}

public abstract class CharacterStatusHint_Base : MonoBehaviour, ICharacterStatus
{
    [Header("狀態提示基本物件")]
    [SerializeField] protected Image buffIcon;      //Buff圖示
    [SerializeField] protected CanvasGroup canvasGroup;     //Buff時間快到閃爍消失需要的腳本

    [Header("狀態詳細資訊物件")]
    [SerializeField] protected CharacterStatusHintIntroSetting buffIntroObj;      //Buff詳細物件

    public float TempCoolDownTime { get; set; }     //暫存剩餘時間

    public string CharacterStatusName { get; set; }

    public string CharacterStatusID { get; protected set; }

    public float CharacterStatusDuration { get; set; }

    public string CharacterStatusIntro { get; set; }

    public string CharacterStatusType { get; set; }

    private Coroutine alphaAinmCoroutine;       //閃爍動畫協成

    private Skill_Base_Buff skill_Base_Buff;

    public EventHandler<float> CharacterHintTimeEvent;     //狀態效果時間更新事件

    /// <summary>
    /// 初始化狀態提示資料
    /// </summary>
    public virtual void BuffHintInit(Skill_Base_Buff skillBase_Buff)
    {
        //狀態提示基本資料設定
        CharacterStatusName = skillBase_Buff.SkillName;
        CharacterStatusID = skillBase_Buff.SkillID;
        CharacterStatusDuration = skillBase_Buff.EffectDurationTime;
        CharacterStatusIntro = skillBase_Buff.SkillIntro;
        CharacterStatusType = skillBase_Buff.CharacterStatusType.ToString();
        buffIcon.sprite = CommonFunction.LoadSkillIcon(CharacterStatusID);
        skill_Base_Buff = skillBase_Buff;

        //訂閱移除事件
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent += RemoveCharacterStatusHint;

        //設定狀態提示物件父級 與 大小 位置
        gameObject.transform.parent = CharacterStatusManager.Instance.ReturnCharacterStatusArea(CharacterStatusType);
        gameObject.transform.localScale = Vector3.one;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

        //暫存
        CharacterStatusManager.Instance.CharacterStatusHintDic.Add(CharacterStatusID, this);
    }

    public virtual IEnumerable UpdateTimerCoroutine(float deltaTime)
    {
        //更新時間
        TempCoolDownTime = deltaTime;
        while (TempCoolDownTime > 0)
        {
            TempCoolDownTime -= 0.1f;
            //更新持續時間文字
            //buffTimerText.text = TempCoolDownTime.ToString();
            CharacterHintTimeEvent?.Invoke(null, TempCoolDownTime);

            if (TempCoolDownTime <= 10)
                alphaAinmCoroutine = StartCoroutine(CanvasGroupAnim());
            else
            {
                if (alphaAinmCoroutine != null)
                {
                    StopCoroutine(alphaAinmCoroutine);
                    alphaAinmCoroutine = null;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public virtual void UpdateTimerMethod(object o, float deltaTime)
    {
        TempCoolDownTime = deltaTime;
        CharacterHintTimeEvent?.Invoke(null, TempCoolDownTime);
        //buffTimerText.text = TempCoolDownTime.ToString();
        if (TempCoolDownTime <= 10)
            alphaAinmCoroutine = StartCoroutine(CanvasGroupAnim());
        else
        {
            if (alphaAinmCoroutine != null)
            {
                StopCoroutine(alphaAinmCoroutine);
                alphaAinmCoroutine = null;
            }
        }
    }

    public virtual void RemoveCharacterStatusHint(object o, string id)
    {
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent -= RemoveCharacterStatusHint;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 閃爍動畫協程
    /// </summary>
    /// <returns></returns>
    protected IEnumerator CanvasGroupAnim()
    {
        float alpha = canvasGroup.alpha;
        if (alpha > 0)
        {
            while (alpha > 0)
            {
                alpha -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            while (alpha < 1)
            {
                alpha += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }
        alphaAinmCoroutine = StartCoroutine(CanvasGroupAnim());
    }

    /// <summary>
    /// 點擊事件
    /// </summary>
    public void OnClick()
    {
        CharacterStatusHintIntroSetting tempObj = Instantiate(buffIntroObj);
        tempObj.Init(this, skill_Base_Buff);
    }
}

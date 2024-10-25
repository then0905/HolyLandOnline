using System;
using System.Collections;
using System.Linq;
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
    /// 紀錄持續時間資料
    /// </summary>
    float TempCoolDownTime { get; set; }
    /// <summary>
    /// 狀態效果移除
    /// </summary>
    /// <param name="deltaTime"></param>
    void RemoveCharacterStatusHint(params SkillOperationData[] skillcomponent);
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

    public SkillOperationData[] SkillOperationDatas { get; set; }

    public EventHandler<float> CharacterHintTimeEvent;     //狀態效果時間更新事件

    /// <summary>
    /// 初始化狀態提示資料
    /// </summary>
    public virtual IEnumerator BuffHintInit(SkillComponent skillcomponent, params SkillOperationData[] skillOperationData)
    {
        //狀態提示基本資料設定
        skill_Base_Buff = skillcomponent.SkillBase as Skill_Base_Buff;
        SkillOperationDatas = skillOperationData;

        CharacterStatusName = skill_Base_Buff.SkillName;
        CharacterStatusID = skill_Base_Buff.SkillID;
        CharacterStatusIntro = skill_Base_Buff.SkillIntro;
        CharacterStatusType = CharacterStatusManager.Instance.ReturnCharacterTypeStr(skillOperationData[0].SkillComponentID);
        buffIcon.sprite = CommonFunction.LoadSkillIcon(CharacterStatusID);

        //訂閱移除事件
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent += RemoveCharacterStatusHint;

        //設定狀態提示物件 位置 大小 與 旋轉方向
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;

        //暫存
        CharacterStatusManager.Instance.CharacterStatusHintDic.Add(this);

        if (CharacterStatusManager.Instance.ReturnTimerCheck(skillOperationData[0].SkillComponentID))
            //開始運行計時
            StartCoroutine(UpdateTimer(skillOperationData[0].EffectDurationTime));

        yield return new WaitForEndOfFrame();

        //gameObject.transform.eulerAngles = Vector3.zero;
    }

    public IEnumerator UpdateTimer(float timer)
    {
        TempCoolDownTime = timer;
        while (TempCoolDownTime > 0)
        {
            TempCoolDownTime -= 0.1f;
            if (TempCoolDownTime <= 10)
                StartCoroutine(CanvasGroupAnim());
            yield return new WaitForSeconds(0.1f);
        }
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent?.Invoke(this.SkillOperationDatas);
    }

    public virtual void RemoveCharacterStatusHint(params SkillOperationData[] skillcomponent)
    {
        if (skillcomponent.ToArray().SequenceEqual(this.SkillOperationDatas))
        {
            CharacterStatusManager.Instance.CharacterSatusRemoveEvent -= RemoveCharacterStatusHint;
            foreach (var item in skill_Base_Buff.SkillComponentList)
            {
                if (item is BuffComponent)
                {
                    (item as BuffComponent).ReverseExecute(skillcomponent);
                }
            }
            CharacterStatusManager.Instance.CharacterStatusHintDic.Remove(this);
            Destroy(this.gameObject);
        }
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

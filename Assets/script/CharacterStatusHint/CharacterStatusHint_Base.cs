using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
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
    void RemoveCharacterStatusHint(params OperationData[] operationData);
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

    protected Coroutine alphaAinmCoroutine;       //閃爍動畫協成

    public OperationData[] OperationDatas { get; set; }     //儲存 運算資料

    public EventHandler<float> CharacterHintTimeEvent;     //狀態效果時間更新事件
    
    protected Skill_Base_Buff skill_Base_Buff;      //技能類型狀態效果資料
    protected ItemEffectBase_Buff itemEffectBase_Buff;      //道具狀態效果資料

    /// <summary>
    /// 初始化狀態提示資料
    /// </summary>
    public virtual IEnumerator BuffHintInit(BaseComponent basecomponent, params OperationData[] operationData)
    {
        //狀態提示基本資料設定
        if (basecomponent is SkillComponent skillComponent
            && operationData is SkillOperationData[] skillOperationDatas)
        {
            //儲存技能類型狀態效果
            skill_Base_Buff = skillComponent.SkillBase as Skill_Base_Buff;

            //儲存技能狀態效果的各基本資料
            CharacterStatusName = skill_Base_Buff.SkillName;
            CharacterStatusID = skill_Base_Buff.SkillID;
            CharacterStatusIntro = skill_Base_Buff.SkillIntro;
            CharacterStatusType = CharacterStatusManager.Instance.ReturnCharacterTypeStr(skillOperationDatas[0].SkillComponentID);
            buffIcon.sprite = CommonFunction.LoadSkillIcon(CharacterStatusID);

            if (CharacterStatusManager.Instance.ReturnTimerCheck(skillOperationDatas[0].SkillComponentID))
                //開始運行計時
                UpdateTimer(skillOperationDatas[0].EffectDurationTime);
        }
        else if (basecomponent is ItemComponent itemComponent
            && operationData is ItemEffectData[] itemOperationDatas)
        {
            //儲存道具類型狀態效果
            itemEffectBase_Buff = itemComponent.ItemBase as ItemEffectBase_Buff;

            //儲存道具狀態效果的各基本資料
            CharacterStatusName = itemEffectBase_Buff.ItemName;
            CharacterStatusID = itemEffectBase_Buff.ItemID;
            CharacterStatusIntro = itemEffectBase_Buff.ItemIntro;
            CharacterStatusType = CharacterStatusManager.Instance.ReturnCharacterTypeStr(itemOperationDatas[0].ItemComponentID);
            buffIcon.sprite = CommonFunction.LoadSkillIcon(CharacterStatusID);

            if (CharacterStatusManager.Instance.ReturnTimerCheck(itemOperationDatas[0].ItemComponentID))
                //開始運行計時
                UpdateTimer(itemOperationDatas[0].EffectDurationTime);
        }
        else
            yield break;

        //訂閱移除事件
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent += RemoveCharacterStatusHint;

        //設定狀態提示物件 位置 大小 與 旋轉方向
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;

        //暫存 這個生成的狀態物件資料
        CharacterStatusManager.Instance.CharacterStatusHintDic.Add(this);
        //儲存 運算資料
        OperationDatas = operationData;
        yield return new WaitForEndOfFrame();

        //gameObject.transform.eulerAngles = Vector3.zero;
    }

    public virtual async void UpdateTimer(float timer)
    {
        TempCoolDownTime = timer;
        while (TempCoolDownTime > 0)
        {
            await Task.Delay(100); // 100ms = 0.1秒
            TempCoolDownTime -= 0.1f;
            CharacterHintTimeEvent?.Invoke(this, TempCoolDownTime);
            if (TempCoolDownTime <= 10)
                CanvasGroupAnim();
        }
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent?.Invoke(this.OperationDatas);
    }

    public virtual void RemoveCharacterStatusHint(params OperationData[] operationData)
    {
        if (operationData.ToArray().SequenceEqual(this.OperationDatas))
        {
            //解除效果移除事件的訂閱
            CharacterStatusManager.Instance.CharacterSatusRemoveEvent -= RemoveCharacterStatusHint;
            
            //若是 技能類型的buff效果
            var buffComponen = skill_Base_Buff.SkillComponentList.Where(x => x is BuffComponent).Select(x => x as BuffComponent).FirstOrDefault();
            //檢查空值 並還原加成效果
            buffComponen?.ReverseExecute(OperationDatas);
            
            //若是 道具類型的buff效果
            var itemComponen = itemEffectBase_Buff.ItemComponentList.Where(x => x is BuffItemComponent).Select(x => x as BuffItemComponent).FirstOrDefault();
            //檢查空值 並還原加成效果
            itemComponen?.ReverseExecute(OperationDatas);
            
            CharacterStatusManager.Instance.CharacterStatusHintDic.Remove(this);
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 閃爍動畫協程
    /// </summary>
    /// <returns></returns>
    protected async void CanvasGroupAnim()
    {
        //float alpha = canvasGroup.alpha;
        //if (alpha > 0)
        //{
        //    while (alpha > 0)
        //    {
        //        await Task.Delay(100); // 100ms = 0.1秒
        //        alpha -= 0.1f;
        //    }
        //}
        //else
        //{
        //    while (alpha < 1)
        //    {
        //        await Task.Delay(100); // 100ms = 0.1秒
        //        alpha += 0.1f;
        //    }
        //}
        //CanvasGroupAnim();
    }

    /// <summary>
    /// 點擊事件
    /// </summary>
    public void OnClick()
    {
        CharacterStatusHintIntroSetting tempObj = Instantiate(buffIntroObj);
        tempObj.Init(this);
    }
}

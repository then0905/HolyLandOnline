
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/10/16
//  創建用途:Buff型技能基底(分類)
//==========================================
public abstract class Skill_Base_Buff : Skill_Base
{
    protected new readonly SkillEffectCategory category = SkillEffectCategory.Buff;

    //角色狀態訊息類別標籤
    protected abstract CharacterStatusType characterStatusType { get; }
    public CharacterStatusType CharacterStatusType => characterStatusType;

    //暫存 角色狀態提示物件
    private CharacterStatusHint_Base characterStatusHintObj;

    //紀錄技能效果是否正在啟動
    protected bool buffIsRun = false;

    //Buff效果時間更新事件
    public EventHandler<float> BuffTimerUpdateEvent;

    /// <summary>
    /// Buff型技能啟動
    /// <para> 先檢查有無條件資料 有資料卻沒達成return </para>
    /// </summary>
    protected void SkillBuffEffectStart()
    {
        //若有條件資料 卻沒達成 return
        if (condition != null && condition.Count > 0 && gameObject)
        {
            //檢查所有條件 未達成
            if (!CheckCondition())
            {
                print("技能ID:" + skillID + "  條件未達成 取消執行");
                SkillEffectEnd();
                return;
            }
            //達成條件
            else
            {
                for (int i = 0; i < influenceStatus.Count; i++)
                {
                    //將技能效果進行加成運算
                    StatusOperation.Instance.SkillEffectStatusOperation(influenceStatus[i], addType[i].Contains("Rate"), effectValue[i]);
                    //紀錄技能啟動狀態
                    buffIsRun = true;
                }
                //若技能為主動 開始計時
                if (characteristic)
                    SkillEffectTime();
                else
                {
                    CharacterStatusHintSetting();
                }
            }
        }
        //若沒條件資料 或 達成條件 執行
        //因為升級
        else if ((condition == null && condition.Count < 1) || CheckCondition() && gameObject)
        {
            for (int i = 0; i < influenceStatus.Count; i++)
            {
                StatusOperation.Instance.SkillEffectStatusOperation(influenceStatus[i], addType[i].Contains("Rate"), effectValue[i]);
                //若技能為主動 開始計時
                //紀錄技能啟動狀態
                buffIsRun = true;
            }
            if (characteristic)
                SkillEffectTime();
            else
            {
                CharacterStatusHintSetting();
            }
        }
    }

    /// <summary>
    /// 技能持續時間計算
    /// </summary>
    protected void SkillEffectTime()
    {
        CharacterStatusHintSetting();
        StartCoroutine(EffectTimer());
    }

    /// <summary>
    /// 技能效果結束
    /// </summary>
    protected override void SkillEffectEnd()
    {
        if (buffIsRun)
            for (int i = 0; i < influenceStatus.Count; i++)
            {
                //還原加成效果
                StatusOperation.Instance.SkillEffectStatusOperation(influenceStatus[i], addType[i].Contains("Rate"), -1 * effectValue[i]);
                //紀錄技能啟動狀態
                buffIsRun = false;
            }
        //解除事件訂閱
        BuffTimerUpdateEvent -= characterStatusHintObj.UpdateTimerMethod;
        //呼叫角色狀態物件移除事件
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent?.Invoke(null, characterStatusHintObj.CharacterStatusID);
        print("移除的被動技能:" + skillID);

        //刪除自己
        if (this.gameObject)
        {
            if (this is Skill_Base_Buff_Passive)
                PassiveSkillManager.Instance.SkillPassiveBuffList.Remove(GetComponent<Skill_Base_Buff_Passive>());
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 技能時間計時協程
    /// </summary>
    /// <param name="statusType"></param>
    /// <param name="Rate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private IEnumerator EffectTimer()
    {
        //取得持續時間
        float time = effectDurationTime;

        //計時迴圈
        while (time > 0)
        {
            time -= Time.deltaTime;
            BuffTimerUpdateEvent?.Invoke(this, time);
            yield return null;
        }
        SkillEffectEnd();
    }

    /// <summary>
    /// 角色狀態效果設定
    /// </summary>
    protected void CharacterStatusHintSetting()
    {
        CharacterStatusManager.Instance.CharacterSatusAddEvent?.Invoke(null, this);
    }
}

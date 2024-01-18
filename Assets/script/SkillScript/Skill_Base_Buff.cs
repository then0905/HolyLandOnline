
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
    protected bool buffIsRun = false;
    /// <summary>
    /// Buff型技能啟動
    /// <para> 先檢查有無條件資料 有資料卻沒達成return </para>
    /// </summary>
    protected void SkillBuffEffectStart()
    {
        //若有條件資料 卻沒達成 return
        if (condition != null && condition.Count > 0 && gameObject)
        {
            if (!CheckCondition())
            {
                print("技能ID:" + skillName + "  條件未達成 取消執行");
                Destroy(this.gameObject);
                return;
            }
            else
            {
                for (int i = 0; i < influenceStatus.Count; i++)
                {
                    StatusOperation.Instance.SkillEffectStatusOperation(influenceStatus[i], addType[i].Contains("Rate"), effectValue[i]);
                    //若技能為主動 開始計時
                    if (characteristic) SkillEffectTime(influenceStatus[i], addType[i].Contains("Rate"), effectValue[i] * -1);
                    //紀錄技能啟動狀態
                    buffIsRun = true;
                }
            }
        }
        //若沒條件資料 或 達成條件 執行
        //因為升級
        else if ((condition == null && condition.Count < 1) || CheckCondition() && gameObject )
        {
            for (int i = 0; i < influenceStatus.Count; i++)
            {
                StatusOperation.Instance.SkillEffectStatusOperation(influenceStatus[i], addType[i].Contains("Rate"), effectValue[i]);
                //若技能為主動 開始計時
                if (characteristic) SkillEffectTime(influenceStatus[i], addType[i].Contains("Rate"), effectValue[i] * -1);
                //紀錄技能啟動狀態
                buffIsRun = true;
            }
        }
    }

    /// <summary>
    /// 技能持續時間計算
    /// </summary>
    protected void SkillEffectTime(string statusType, bool Rate, float value)
    {
        StartCoroutine(EffectTimer(statusType, Rate, value));
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
        print("移除的被動技能:" + skillName);
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
    private IEnumerator EffectTimer(string statusType, bool Rate, float value)
    {
        //取得持續時間
        float time = effectDurationTime;

        //計時迴圈
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        SkillEffectEnd();
    }
}

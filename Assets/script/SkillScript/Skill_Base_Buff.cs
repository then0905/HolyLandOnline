
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/10/16
//  創建用途:Buff型技能基底
//==========================================
public abstract class Skill_Base_Buff : Skill_Base
{
    protected new readonly SkillEffectCategory category = SkillEffectCategory.Buff;

    /// <summary>
    /// Buff型技能啟動
    /// <para> 先檢查有無條件資料 有資料卻沒達成return </para>
    /// </summary>
    protected void SkillBuffEffectStart()
    {
        //若有條件資料 卻沒達成 return
        if (condition != null && condition.Count > 0)
        {
            if (!CheckCondition())
            {
                Destroy(this.gameObject);
                return;
            }
        }
        //若沒條件資料 或 達成條件 執行
        else if ((condition == null && condition.Count < 1) || CheckCondition())
        {
            for (int i = 0; i < influenceStatus.Count; i++)
            {
                StatusOperation.Instance.SkillEffectStatusOperation(influenceStatus[i], addType[i].Contains("Rate"), effectValue[i]);
                //若技能為主動 開始計時
                if (!characteristic) SkillEffectTime(influenceStatus[i], addType[i].Contains("Rate"), effectValue[i] * -1);
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
    protected override void SkillEffectEnd(string statusType, bool Rate, float value)
    {
        //還原加成效果
        StatusOperation.Instance.SkillEffectStatusOperation(statusType, Rate, value);
        //效果時間結束刪除自己
        Destroy(this.gameObject);
    }

    private IEnumerator EffectTimer(string statusType, bool Rate, float value)
    {
        //取得持續時間
        float time = effectDurationTime;

        //計時迴圈
        while (time > 0)
        {
            time -= Time.deltaTime;
        }
        SkillEffectEnd(statusType, Rate, value);
        yield return null;
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/10/16
//  創建用途:Buff型技能基底(分類)
//==========================================
public abstract class Skill_Base_Buff : Skill_Base
{
    //紀錄技能效果是否正在啟動
    protected bool buffIsRun = false;

    /// <summary>
    /// 技能效果結束
    /// </summary>
    protected override void SkillEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {
        print("移除的被動技能:" + SkillName);

        //刪除自己
        if (this.gameObject)
        {
            if (this is Skill_Base_Buff_Passive)
                PassiveSkillManager.Instance.SkillPassiveBuffList.Remove(GetComponent<Skill_Base_Buff_Passive>());
            Destroy(this.gameObject);
        }
    }
}

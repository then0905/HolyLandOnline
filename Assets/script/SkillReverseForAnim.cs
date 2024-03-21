using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/23
//  創建用途:技能施放結束的還原 For 動畫事件
//==========================================
public class SkillReverseForAnim : MonoBehaviour
{
    /// <summary>
    /// 還原技能施放(For AnimationEvent)
    /// </summary>
    public void ReverseUsingSkill()
    {
        SkillDisplayAction.Instance.UsingSkill = false;
        Character_move.Instance.AutoNavToTarget = false;
        if (SkillDisplayAction.Instance.UsingSkillObj.GetComponent<Skill_Base>() is Skill_Base_Attack)
        {
            SkillDisplayAction.Instance.UsingSkillObj.GetComponent<Skill_Base>().SkillEndForAnimation();
        }
    }
}

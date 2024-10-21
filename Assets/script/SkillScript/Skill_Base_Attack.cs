using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/20
//  創建用途:攻擊型技能基底(分類)
//==========================================
public abstract class Skill_Base_Attack : Skill_Base
{
    protected new readonly SkillEffectCategory category = SkillEffectCategory.Attack;

    /// <summary>
    /// 技能是否升級狀態
    /// </summary>
    [HideInInspector] public bool SkillBeUpgrade { get { return skillBeUpgrade; } }

    /// <summary>
    /// 攻擊型技能標籤
    /// </summary>
    public enum AttackCategory
    {
        /// <summary>
        /// 近距離攻擊
        /// </summary>
        MeleeATK,
        /// <summary>
        /// 遠距離攻擊
        /// </summary>
        RemoteATK,
        /// <summary>
        /// 魔法攻擊
        /// </summary>
        MageATK,
    }

    /// <summary>
    /// 確認是否有選擇到目標
    /// </summary>
    /// <param name="attacker">施放者</param>
    /// <param name="defender">被施放者</param>
    protected void CheckGetAnyTarget(ICombatant attacker = null, ICombatant defender = null)
    {
        if (SelectTarget.Instance.Targetgameobject != null/* && !SkillBeUpgrade*/)
        {
            BattleOperation.Instance.SkillAttackEvent?.Invoke(this, attacker, defender);
            PlayerDataOverView.Instance.CharacterMove.CharacterAnimator.SetTrigger(skillID);
        }
        else
        {
            SkillEffectEnd();
        }
    }
}

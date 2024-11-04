using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/11/04
//  創建用途: 負面效果基底
//==========================================
public abstract class DebuffEffectBase : MonoBehaviour
{
    /// <summary>
    /// 效果ID
    /// </summary>
    public string EffectID => skilldata.SkillOperationData.SkillID;

    /// <summary>
    /// 影響屬性
    /// </summary>
    public string InfluenceStatus => skilldata.SkillOperationData.InfluenceStatus;

    /// <summary>
    /// 效果名稱文字
    /// </summary>
    public string EffectName => ("TM_" + InfluenceStatus + "_Name").GetText();

    /// <summary>
    /// 效果介紹文字
    /// </summary>
    public string EffectIntro => ("TM_" + InfluenceStatus + "_Intro").GetText();

    /// <summary>
    /// 持續時間
    /// </summary>
    public float Duration => skilldata.SkillOperationData.EffectDurationTime;

    /// <summary>
    /// 效果類型文字
    /// </summary>
    public abstract string EffectType { get; }

    protected ICombatant caster;
    protected ICombatant target;
    protected BuffComponent skilldata;

    public BuffComponent SkillData => skilldata;

    /// <summary>
    /// 效果啟動
    /// </summary>
    public abstract void EffectStart(ICombatant caster, ICombatant target, BuffComponent skillTarget);

    /// <summary>
    /// 效果結束
    /// </summary>
    public abstract void EffectEnd();
}


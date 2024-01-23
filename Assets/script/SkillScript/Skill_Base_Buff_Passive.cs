
//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:被動型buff技能(無須額外效果)
//==========================================
public  class Skill_Base_Buff_Passive : Skill_Base_Buff
{
    /*
     被動型技能幾乎為buff屬性(加強玩家)
     */
    protected override void SkillEffectStart()
    {
        SkillBuffEffectStart();
        print("施放的被動技能效果:"+skillName);
        if (gameObject != null)
            gameObject.transform.parent = PassiveSkillManager.Instance.transform;
    }

    private void OnDestroy()
    {
        SkillEffectEnd();
    }

    /// <summary>
    /// 重新啟動技能效果 用來穿脫裝時 重新計算數值
    /// </summary>
    public virtual void RestartSkillEffect()
    {
        SkillEffectEnd();

        SkillBuffEffectStart();
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
        print("關閉的被動技能:" + skillName);
    }
}

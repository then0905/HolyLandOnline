using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//==========================================
//  創建者:家豪
//  創建日期:2023/12/27
//  創建用途:被動型buff技能_強化指定技能
//==========================================
public class Skill_Base_Buff_SkillUpgrade : Skill_Base_Buff_Passive
{
    [Header("升級的指定技能"), SerializeField] protected string upgradeSkillID;

    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        SkillComponentList.ForEach(x => x.Execute(attacker, defenfer));
        if (gameObject != null)
            gameObject.transform.parent = PassiveSkillManager.Instance.transform;
    }

    protected override void SkillEffectEnd(ICombatant attacker = null, ICombatant defenfer = null)
    {
        foreach (var component in SkillComponentList)
        {
            if (component is UpgradeSkillComponent skillcomponentData)
            {
                skillcomponentData.ReverseExecute();
            }
        }
        buffIsRun = false;
        if (this.gameObject)
        {
            if (this is Skill_Base_Buff_Passive)
                PassiveSkillManager.Instance.SkillPassiveBuffList.Remove(GetComponent<Skill_Base_Buff_Passive>());
            Destroy(this.gameObject);
        }
    }
}

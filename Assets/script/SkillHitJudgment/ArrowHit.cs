using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/12/16
//  創建用途:  技能範圍檢測(方向直線型)
//==========================================
public class ArrowHit : MonoBehaviour
{
    //此範圍物件
    public GameObject SKillArrow;
    //此範圍碰撞
    public BoxCollider ArrowCollider;
    //存取技能資料
    private SkillData skillData;

    /// <summary>
    /// 設定有效命中範圍
    /// </summary>
    /// <param name="skillBaseAttack">技能資料</param>
    /// <param name="skillID">技能ID</param>
    public List<ICombatant> SetSkillSize(Skill_Base_Attack skillBaseAttack, SkillOperationData skill)
    {
        //獲取當前技能資料
        skillData = skillBaseAttack.SkillData;
        //設定碰撞範圍
        ArrowCollider.size = new Vector3(skillData.Width, skillData.Height, 1);
        ArrowCollider.enabled = true;

        //範圍內所有怪物
        if (skill.TargetCount.Equals(-4))
            return Physics.OverlapBox(transform.localPosition, ArrowCollider.size, Quaternion.identity)
       .Where(x => x != null && x.GetComponent<MonsterBehaviour>() != null).Select(x => x.GetComponent<ICombatant>()).ToList();
        //所有敵對玩家
        else if (skill.TargetCount.Equals(-2))
            return Physics.OverlapBox(transform.localPosition, ArrowCollider.size, Quaternion.identity)
       .Where(x => x != null && x.GetComponent<OtherPlayerCharacter>() != null).Select(x => x.GetComponent<ICombatant>()).ToList();
        //範圍內數量
        else
        {
            return Physics.OverlapBox(transform.localPosition, ArrowCollider.size, Quaternion.identity)
                   .Where(x => x != null && x.GetComponent<ICombatant>() != null).Select(x => x.GetComponent<ICombatant>()).Take(skill.TargetCount).ToList();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;//
        Vector3 v3 = new Vector3(ArrowCollider.size.x, 1, ArrowCollider.size.y);
        Gizmos.DrawWireCube(transform.position, v3);//
    }
}

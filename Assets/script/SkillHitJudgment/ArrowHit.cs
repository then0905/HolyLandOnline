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
    public List<ICombatant> SetSkillSize(Skill_Base_Attack skillBaseAttack, SkillOperationData skill, ICombatant attacker, ICombatant defenfer)
    {
        //獲取當前技能資料
        skillData = skillBaseAttack.SkillData;

        //計算朝向
        Vector3 direction = defenfer.Obj.transform.position - attacker.Obj.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        //設定碰撞範圍
        ArrowCollider.size = new Vector3(skillData.Width, skillData.Height, 5);
        ArrowCollider.enabled = true;

        //設置碰撞體的旋轉
        ArrowCollider.transform.rotation = rotation;

        //宣告範圍內所有碰撞物
        List<Collider> colliderList = new List<Collider>();
        //宣告清單暫存命中目標
        List<ICombatant> targetList = new List<ICombatant>();

        //使用rotated的OverlapBox
        colliderList = Physics.OverlapBox(
            defenfer.Obj.transform.position,
            ArrowCollider.size / 2, // size要除以2因為OverlapBox使用半尺寸
            rotation).ToList();

        //範圍內所有怪物
        if (skill.TargetCount.Equals(-4))
            targetList = colliderList
                .Where(x => x != null &&
                x.GetComponent<MonsterBehaviour>() != null &&
                x.GetComponent<ICombatant>() != attacker)
                .Select(x => x.GetComponent<ICombatant>())
                .ToList();
        //所有敵對玩家
        else if (skill.TargetCount.Equals(-2))
            targetList = colliderList
                .Where(x => x != null &&
                x.GetComponent<OtherPlayerCharacter>() != null &&
                x.GetComponent<ICombatant>() != attacker)
                .Select(x => x.GetComponent<ICombatant>())
                .ToList();
        //範圍內數量
        else
        {
            targetList = colliderList
                .Where(x => x != null &&
                       x.GetComponent<ICombatant>() != null &&
                       x.GetComponent<ICombatant>() != attacker)
                .Select(x => x.GetComponent<ICombatant>())
                .Take(skill.TargetCount)
                .ToList();
        }
        return targetList;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // 只在遊戲運行時繪製

        Gizmos.color = Color.red;
        // 使用矩陣來正確繪製旋轉後的方體
        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            ArrowCollider.transform.rotation,
            Vector3.one
        );

        Vector3 v3 = new Vector3(ArrowCollider.size.x, ArrowCollider.size.z, ArrowCollider.size.y);
        Gizmos.DrawWireCube(Vector3.zero, v3);
    }
}

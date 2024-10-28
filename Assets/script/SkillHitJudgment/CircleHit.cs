using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CircleHit : MonoBehaviour
{
    public SkillController Skillrange;
    public GameObject SkillTargetCircle;
    public SphereCollider CircleCollider; 
    //存取技能資料
    private SkillData skillData;
    //public List<ICombatant> SetSkillSize(Skill_Base_Attack skillBaseAttack, SkillOperationData skill, ICombatant attacker, ICombatant defenfer)
    //{
    //    //獲取當前技能資料
    //    skillData = skillBaseAttack.SkillData;

    //    //計算朝向
    //    Vector3 direction = defenfer.Obj.transform.position - attacker.Obj.transform.position;
    //    Quaternion rotation = Quaternion.LookRotation(direction);

    //    //設定碰撞範圍
    //    CircleCollider.radius = skillData.CircleDistance;
    //    CircleCollider.enabled = true;

    //    //宣告範圍內所有碰撞物
    //    List<Collider> colliderList = new List<Collider>();
    //    //宣告清單暫存命中目標
    //    List<ICombatant> targetList = new List<ICombatant>();

    //    //使用rotated的OverlapBox
    //    colliderList = Physics.OverlapSphere(
    //        defenfer.Obj.transform.position,
    //        CircleCollider.radius 
    //        ).ToList();

    //    //範圍內所有怪物
    //    if (skill.TargetCount.Equals(-4))
    //        targetList = colliderList
    //            .Where(x => x != null &&
    //            x.GetComponent<MonsterBehaviour>() != null &&
    //            x.GetComponent<ICombatant>() != attacker)
    //            .Select(x => x.GetComponent<ICombatant>())
    //            .ToList();
    //    //所有敵對玩家
    //    else if (skill.TargetCount.Equals(-2))
    //        targetList = colliderList
    //            .Where(x => x != null &&
    //            x.GetComponent<OtherPlayerCharacter>() != null &&
    //            x.GetComponent<ICombatant>() != attacker)
    //            .Select(x => x.GetComponent<ICombatant>())
    //            .ToList();
    //    //範圍內數量
    //    else
    //    {
    //        targetList = colliderList
    //            .Where(x => x != null &&
    //                   x.GetComponent<ICombatant>() != null &&
    //                   x.GetComponent<ICombatant>() != attacker)
    //            .Select(x => x.GetComponent<ICombatant>())
    //            .Take(skill.TargetCount)
    //            .ToList();
    //    }
    //    return targetList;
    //}

    //// 添加 Gizmos 來視覺化碰撞範圍
    //private void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying || !CircleCollider.enabled) return;

    //    Gizmos.color = Color.red;
    //    // 繪製線框球體來表示碰撞範圍
    //    Gizmos.DrawWireSphere(transform.position, CircleCollider.radius);
    //}
    public List<ICombatant> SetSkillSize(Skill_Base_Attack skillBaseAttack, SkillOperationData skill, ICombatant attacker, ICombatant defenfer)
    {
        //獲取當前技能資料
        skillData = skillBaseAttack.SkillData;

        //設定碰撞範圍
        CircleCollider.radius = skillData.CircleDistance;
        CircleCollider.enabled = true;

        // 獲取目標位置，但保持在地面高度
        Vector3 targetPosition = defenfer.Obj.transform.position;
        float detectHeight = 1f; // 設置檢測的高度範圍

        // 創建一個扁平的膠囊體檢測範圍
        // point1 和 point2 是膠囊體的兩個端點，相距很近來模擬平面
        Vector3 point1 = targetPosition + Vector3.up * (detectHeight / 2);
        Vector3 point2 = targetPosition - Vector3.up * (detectHeight / 2);

        //宣告範圍內所有碰撞物
        List<Collider> colliderList = new List<Collider>();
        //宣告清單暫存命中目標
        List<ICombatant> targetList = new List<ICombatant>();

        // 使用 OverlapCapsule 進行檢測
        colliderList = Physics.OverlapCapsule(
            point1,
            point2,
            CircleCollider.radius
        ).ToList();

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

    // 視覺化檢測範圍
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !CircleCollider.enabled) return;

        Gizmos.color = Color.red;

        // 獲取檢測位置和參數
        Vector3 position = transform.position;
        float detectHeight = 1f;
        float radius = CircleCollider.radius;

        // 繪製圓形
        int segments = 32;
        float angleStep = 360f / segments;

        // 繪製上下兩個圓形
        for (float h = -detectHeight / 2; h <= detectHeight / 2; h += detectHeight)
        {
            Vector3 prevPoint = Vector3.zero;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 point = position + new Vector3(
                    Mathf.Cos(angle) * radius,
                    h,
                    Mathf.Sin(angle) * radius
                );

                if (i > 0)
                    Gizmos.DrawLine(prevPoint, point);

                prevPoint = point;
            }
        }

        // 繪製連接上下圓的線
        for (int i = 0; i < segments; i += segments / 4)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            Vector3 bottom = position + direction * radius - Vector3.up * (detectHeight / 2);
            Vector3 top = position + direction * radius + Vector3.up * (detectHeight / 2);

            Gizmos.DrawLine(bottom, top);
        }
    }
}

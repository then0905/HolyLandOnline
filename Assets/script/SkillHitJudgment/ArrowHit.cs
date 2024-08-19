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
    private SkillDataModel skillData;
    //存取技能基底
    private Skill_Base_Attack skillAttackData;
    //獲取範圍內其他目標
    private List<ICombatant> otherObjList = new List<ICombatant>();
    //重新設定範圍(從ArrowCollider碰撞上校正x旋轉為0後的範圍)
    private Vector3 correctTriggerRange = new Vector3();

    /// <summary>
    /// 設定有效命中範圍
    /// </summary>
    /// <param name="skillBaseAttack">技能資料</param>
    /// <param name="skillID">技能ID</param>
    public void SetSkillSize(Skill_Base_Attack skillBaseAttack, string skillID)
    {
        //ArrowCollider.size = new Vector3(SKillArrow.GetComponent<RectTransform>().sizeDelta.x, SKillArrow.GetComponent<RectTransform>().sizeDelta.y, 1);

        //初始化目標清單
        otherObjList = new List<ICombatant>();
        //獲取當前技能資料
        skillData = GameData.SkillsDataDic[skillID];
        //設定碰撞範圍
        ArrowCollider.size = new Vector3(skillData.Width, skillData.Height, 1);
        ArrowCollider.enabled = true;
        //設定偵測範圍
        correctTriggerRange = new Vector3(skillData.Width, 1, skillData.Height);
        //儲存技能基底資料
        skillAttackData = skillBaseAttack;
    }


    /// <summary>
    /// 獲取範圍內目標清單
    /// </summary>
    public void CatchTarget(List<ICombatant> otherObj)
    {
        //存取可攻擊對象
        otherObjList.AddRange(otherObj);
        if (otherObjList.Count > skillData.TargetCount)
        {
            otherObjList = otherObjList.Take(skillData.TargetCount).ToList();
        }

        //當可攻擊對象達到目標數量後進行傷害
        //if (otherObjList.Count == skillData.TargetCount)
        //{
        foreach (ICombatant obj in otherObjList)
        {
            //戰鬥計算
            BattleOperation.Instance.SkillAttackEvent?.Invoke(skillAttackData, PlayerDataOverView.Instance, obj);
        }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ICombatant>() != null)
        {
            if (SkillController.Instance.UsingSkill)
            {
                List<Collider> targetList = Physics.OverlapBox(gameObject.transform.position, correctTriggerRange, Quaternion.identity).ToList();
                List<ICombatant> combatantList = new List<ICombatant>();
                foreach (Collider coll in targetList)
                {
                    print("偵測到的目標:" + coll.name);
                    //獲取可攻擊目標(怪物行為腳本或是其他)
                    if (coll.GetComponent<ICombatant>() != null && coll.GetComponent<ActivityCharacterBase>().CanBeChoose)
                    {
                        combatantList.Add(coll.GetComponent<ICombatant>());
                    }

                }
                CatchTarget(combatantList);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;//
        Gizmos.DrawWireCube(transform.position, correctTriggerRange);//
    }
}

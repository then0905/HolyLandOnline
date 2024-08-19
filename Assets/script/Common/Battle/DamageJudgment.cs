using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/20
//  創建用途:  戰鬥系統碰撞判斷(生取資料)
//==========================================
public class DamageJudgment : MonoBehaviour
{


    //戰鬥計算事件
    //public Action BattleEvent;

    private void OnTriggerEnter(Collider other)
    {
        //抓tag判斷是否怪物                     為武器(物理)或是特效物件(技能)
        if (other.gameObject.tag == "Monster"/*other.gameObject.tag == "Weapon" || other.gameObject.tag == "SkillEffect"*/)
        {
           //檢測是否在使用中
            if (SkillController.Instance.UsingSkill)
            {
                //傳送命中s資料
               // BattleOperation.Instance.CatchBattleTarget(other.gameObject);
            }
        }
    }
}

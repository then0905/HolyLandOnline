using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:管理玩家擁有的被動技能
//==========================================
public class PassiveSkillManager : MonoBehaviour
{
    #region 全域靜態變數

    private static PassiveSkillManager instance;

    public static PassiveSkillManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PassiveSkillManager>();
            return instance;
        }
    }
    
    #endregion
}

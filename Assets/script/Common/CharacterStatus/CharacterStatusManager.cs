using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/10/18
//  創建用途:玩家狀態效果提示物件管理器
//==========================================
public class CharacterStatusManager : MonoBehaviour
{
    #region 全域靜態變數

    public static CharacterStatusManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CharacterStatusManager>();
            return instance;
        }
    }

    private static CharacterStatusManager instance;

    #endregion

    [Header("面板檢視")]
    [SerializeField] private Transform characterBuffStatusTrans;     //增益狀態效果物件生成參考
    [SerializeField] private Transform characterDeBuffStatusTrans;     //減益&CC狀態效果物件生成參考
    [SerializeField] private Transform characterPassiveStatusTrans;     //被動狀態效果物件生成參考

    ////儲存目前狀態效果
    public Dictionary<string, CharacterStatusHint_Base> CharacterStatusHintDic = new Dictionary<string, CharacterStatusHint_Base>();

    //事件區
    public EventHandler<Skill_Base_Buff> CharacterSatusAddEvent;      //狀態效果增加事件
    public EventHandler<string> CharacterSatusRemoveEvent;      //狀態效果移除事件

    private void OnEnable()
    {
        CharacterSatusAddEvent += InitCharacterStatusHintCheck;
    }
    private void OnDisable()
    {
        CharacterSatusAddEvent -= InitCharacterStatusHintCheck;
    }

    /// <summary>
    /// 依照狀態效果類型 回傳該生成的位置
    /// </summary>
    /// <param name="buffType">狀態效果類型</param>
    /// <returns></returns>
    public Transform ReturnCharacterStatusArea(string buffType)
    {
        switch (buffType)
        {
            case "Buff":
                return characterBuffStatusTrans;
            case "DeBuff":
            case "CrowdControl":
                return characterDeBuffStatusTrans;
            case "Passive":
                return characterPassiveStatusTrans;
            default:
                return null;
        }
    }

    /// <summary>
    /// 生成角色狀態提示物件前置檢查
    /// </summary>
    /// <param name="o"></param>
    /// <param name="skillbuff"></param>
    public void InitCharacterStatusHintCheck(object o, Skill_Base_Buff skillbuff)
    {
        bool checkCharacterStatusIsExist = CharacterStatusHintDic.TryGetValue(skillbuff.SkillID, out CharacterStatusHint_Base value);
        //若已存在同樣效果 且 低於10秒
        if (checkCharacterStatusIsExist&& value.TempCoolDownTime<=10)
        {
            //更新效果時間
            value.TempCoolDownTime = skillbuff.TempCooldownTime;
        }
        //不存在同樣效果
        else if (!checkCharacterStatusIsExist)
        {
            //直接生成
            CharacterStatusHint_Base characterStatusHintObj = Instantiate(CommonFunction.LoadObject<GameObject>("CharacterStatusHint", "CharacterStatusHint_Buff")).GetComponent<CharacterStatusHint_Base>();
            characterStatusHintObj.BuffHintInit(skillbuff);
            CharacterStatusHintDic.TryAdd(characterStatusHintObj.CharacterStatusID, characterStatusHintObj);
            skillbuff.BuffTimerUpdateEvent += characterStatusHintObj.UpdateTimerMethod;
        }
    }
}

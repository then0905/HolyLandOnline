using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<CharacterStatusHint_Base> CharacterStatusHintDic = new List<CharacterStatusHint_Base>();

    //事件區
    public Action<SkillComponent, SkillOperationData[]> CharacterSatusAddEvent;      //狀態效果增加事件
    public Action<SkillOperationData[]> CharacterSatusRemoveEvent;      //狀態效果移除事件

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
            case "ContinuanceBuff":
            case "AdditiveBuff":
                return characterBuffStatusTrans;
            case "Debuff":
            case "CrowdControl":
                return characterDeBuffStatusTrans;
            case "UpgradeSkill":
            case "EnhanceSkill":
            case "PassiveBuff":
                return characterPassiveStatusTrans;
            default:
                return null;
        }
    }

    /// <summary>
    /// 依照狀態效果類型 回傳技能類型文字
    /// </summary>
    /// <param name="buffType">狀態效果類型</param>
    /// <returns></returns>
    public string ReturnCharacterTypeStr(string buffType)
    {
        switch (buffType)
        {
            case "ContinuanceBuff":
            case "AdditiveBuff":
                return "TM_Buff".GetText();
            case "Debuff":
                return "TM_DeBuff".GetText();
            case "CrowdControl":
                return "TM_CrowdControl".GetText();
            case "UpgradeSkill":
            case "EnhanceSkill":
            case "PassiveBuff":
                return "TM_Passive".GetText();
            default:
                return null;
        }
    }

    /// <summary>
    /// 依照狀態效果類型 回傳是否可執行計時
    /// </summary>
    /// <param name="buffType">狀態效果類型</param>
    /// <returns></returns>
    public bool ReturnTimerCheck(string buffType)
    {
        switch (buffType)
        {
            case "CrowdControl":
            case "Debuff":
            case "ContinuanceBuff":
            case "AdditiveBuff":
                return true;

            case "UpgradeSkill":
            case "EnhanceSkill":
            case "PassiveBuff":
                return false;
            default:
                return true;
        }
    }

    /// <summary>
    /// 生成角色狀態提示物件前置檢查
    /// </summary>
    /// <param name="o"></param>
    /// <param name="skillComponent"></param>
    public void InitCharacterStatusHintCheck(SkillComponent skillcomponent, params SkillOperationData[] skillOperationData)
    {
        var checkCharacterStatusIsExist = CharacterStatusHintDic.Where(x => x.SkillOperationDatas.SequenceEqual(skillOperationData.ToArray())).FirstOrDefault();
        //若已存在同樣效果 且 低於10秒
        if (checkCharacterStatusIsExist && checkCharacterStatusIsExist.TempCoolDownTime <= 10)
        {
            //重新設定資料
            StartCoroutine(checkCharacterStatusIsExist.BuffHintInit(skillcomponent, skillOperationData));
        }
        //不存在同樣效果
        else if (!checkCharacterStatusIsExist)
        {
            //直接生成
            CharacterStatusHint_Base characterStatusHintObj = Instantiate(CommonFunction.LoadObject<GameObject>("CharacterStatusHint", "CharacterStatusHint_Buff"), ReturnCharacterStatusArea(skillOperationData[0].SkillComponentID.ToString())).GetComponent<CharacterStatusHint_Base>();
            StartCoroutine(characterStatusHintObj.BuffHintInit(skillcomponent, skillOperationData));
        }
    }

    /// <summary>
    /// 生成角色狀態提示物件 Debuff版本
    /// </summary>
    /// <param name="o"></param>
    /// <param name="skillComponent"></param>
    public void InitCharacterStatusHintCheck(DebuffEffectBase debuffEffectData)
    {
        //直接生成
        CharacterStatusHint_DeBuff characterStatusHintObj = 
            Instantiate(CommonFunction.LoadObject<GameObject>("CharacterStatusHint", "CharacterStatusHint_Debuff"), ReturnCharacterStatusArea(debuffEffectData.EffectType))
            .GetComponent<CharacterStatusHint_DeBuff>();
        StartCoroutine(characterStatusHintObj.BuffHintInit(debuffEffectData));
    }
}

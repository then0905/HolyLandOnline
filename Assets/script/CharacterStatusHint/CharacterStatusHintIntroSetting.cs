using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/10/20
//  創建用途: 角色狀態效果提示物件詳細資料設定
//==========================================
public class CharacterStatusHintIntroSetting : MonoBehaviour
{
    [SerializeField] protected GameObject characterStatusHintObj;      //詳細說明物件
    [SerializeField] protected Image buffIntroIcon;      //Buff詳細介面使用的圖示
    [SerializeField] protected TextMeshProUGUI buffNameText;      //Buff名稱文字
    [SerializeField] protected TextMeshProUGUI buffIntroText;      //Buff詳細說明文字
    [SerializeField] protected TextMeshProUGUI buffTimerText;      //Buff時間文字
    [SerializeField] protected TextMeshProUGUI buffTypeText;      //Buff類型文字
    [SerializeField] protected Transform buffContentTrans;      //Buff內容文字生成參考
    [SerializeField] protected GameObject buffContentTextObj;      //Buff內容文字物件

    private CharacterStatusHint_Base tempCharacterStatusHintbase;
    protected List<GameObject> buffContentTextObjList = new List<GameObject>();      //暫存生成的內容文字物件

    private void Start()
    {
        transform.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void OnDestroy()
    {
        if (tempCharacterStatusHintbase != null)
            tempCharacterStatusHintbase.CharacterHintTimeEvent -= TimeSetting;
    }

    /// <summary>
    /// 初始化狀態詳細資料
    /// </summary>
    public void Init(ICharacterStatus characterStatus)
    {
        //狀態提示基本資料設定
        buffIntroIcon.sprite = CommonFunction.LoadSkillIcon(characterStatus.CharacterStatusID);
        buffNameText.text = characterStatus.CharacterStatusName;
        buffIntroText.text = characterStatus.CharacterStatusIntro;
        buffTimerText.text = string.Format("TM_TimeLeft".GetText(), (characterStatus.CharacterStatusType == "TM_Passive".GetText()) ? "--" : characterStatus.TempCoolDownTime.ToString());
        buffTypeText.text = characterStatus.CharacterStatusType.ToString();

        //訂閱事件處理
        if (characterStatus is CharacterStatusHint_Base)
        {
            tempCharacterStatusHintbase = characterStatus as CharacterStatusHint_Base;
            tempCharacterStatusHintbase.CharacterHintTimeEvent += TimeSetting;
        }

        //設定buff強化相關資訊
        for (int i = 0; i < tempCharacterStatusHintbase.OperationDatas.Length; i++)
        {
            if (tempCharacterStatusHintbase.OperationDatas[i] is SkillOperationData skillOperationData)
            {
                if (string.IsNullOrEmpty(skillOperationData.InfluenceStatus)) continue;

                GameObject tempObj = Instantiate(buffContentTextObj, buffContentTrans);

                tempObj.SetActive(true);

                TextMeshProUGUI tempObjText = tempObj.GetComponentInChildren<TextMeshProUGUI>();

                tempObjText.text = string.Format("TM_CharacterStatusHintBuffFomat".GetText(),
                    ("TM_" + skillOperationData.InfluenceStatus).GetText(),
                    string.Format(("TM_" + skillOperationData.AddType).GetText(),
                    skillOperationData.EffectValue.ToString()));

                buffContentTextObjList.Add(tempObj);
            }
            else if (tempCharacterStatusHintbase.OperationDatas[i] is ItemEffectData itemEffectData)
            {
                if (string.IsNullOrEmpty(itemEffectData.InfluenceStatus)) continue;

                GameObject tempObj = Instantiate(buffContentTextObj, buffContentTrans);

                tempObj.SetActive(true);

                TextMeshProUGUI tempObjText = tempObj.GetComponentInChildren<TextMeshProUGUI>();

                tempObjText.text = string.Format("TM_CharacterStatusHintBuffFomat".GetText(),
                    ("TM_" + itemEffectData.InfluenceStatus).GetText(),
                    string.Format(("TM_" + itemEffectData.AddType).GetText(),
                    itemEffectData.EffectValue.ToString()));

                buffContentTextObjList.Add(tempObj);
            }
            else continue;
        }
    }

    /// <summary>
    /// 時間設定
    /// </summary>
    /// <param name="o"></param>
    /// <param name="timer"></param>
    private void TimeSetting(object o, float timer)
    {
        buffTimerText.text = string.Format("TM_TimeLeft".GetText(), ((int)timer).ToString()); ;
    }

    public void DestroyGameObject()
    {
        Destroy(this.gameObject);
    }
}

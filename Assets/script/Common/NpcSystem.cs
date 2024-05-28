using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.UI;

//==========================================
//  創建者:    家豪
//  翻修日期:  2024/05/13
//  創建用途:  NPC互動系統
//==========================================

public enum NpcType
{

}

[Serializable]
public struct ButtonFunction
{
    public string ButtonID;
    public UnityEvent ButtonAct;
}

public class NpcSystem : MonoBehaviour
{
    #region 靜態變數
    private static NpcSystem instance;
    public static NpcSystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<NpcSystem>(true);
            return instance;
        }
    }
    #endregion 

    [Header("遊戲物件")]
    [SerializeField] private TextMeshProUGUI chatContent;        //對話框內容
    [SerializeField] private Image avatar;      //NPC頭像
    [SerializeField] private TextMeshProUGUI characterName;      //NPC名稱
    [SerializeField] private Button buttonObj;      //按鈕參考
    [SerializeField] private TextMeshProUGUI buttonText;      //按鈕文字
    [SerializeField] private Transform buttonParent;      //生成父級 按鈕參考


    [Header("遊戲資料"), SerializeField]
    public List<ButtonFunction> ButtonFunctionList = new List<ButtonFunction>();        //按鈕功能清單
    private List<Button> buttonRecordList = new List<Button>();     //紀錄已生成的按鈕清單
    private List<QuestDataModel> questDataList = new List<QuestDataModel>();     //NPC所有任務清單
    private QuestDataModel tempQuestData = null;       //暫存此次任務資料
    private int questStep;      //紀錄任務總步驟
    private int tempStep = 0;      //紀錄任務當前步驟

    /// <summary>
    /// 初始化 NPC 互動頁面
    /// </summary>
    /// <param name="npcData"></param>
    public void InitNpcSystem(NpcDataModel npcData)
    {
        if (gameObject.activeSelf) return;
        chatContent.text = npcData.NpcChatContent[0];
        avatar.sprite = CommonFunction.LoadObject<Sprite>(npcData.NpcAvatarPath, npcData.NpcAvatarName);
        characterName.text = npcData.NpcName;
        ButtonFunctionSetting(npcData.NpcButtonFuncList);
        gameObject.SetActive(true);
        if (npcData.QuestIDList != null && npcData.QuestIDList.Count > 0)
            QuestInit(npcData.QuestIDList.ToArray());
    }

    /// <summary>
    /// 設定按鈕事件
    /// </summary>
    /// <param name="btnList"></param>
    public void ButtonFunctionSetting(List<NpcButtonFunc> btnList)
    {
        for (int i = 0; i < btnList.Count; i++)
        {
            ButtonFunction buttonFunction = ButtonFunctionList.Find(x => x.ButtonID == btnList[i].ButtonActionID);
            buttonText.text = btnList[i].ButtonName;
            Button btn = Instantiate(buttonObj, buttonParent);
            Action act = () => buttonFunction.ButtonAct.Invoke();
            btn.onClick.AddListener(() => act.Invoke());
            btn.gameObject.SetActive(true);
            buttonRecordList.Add(btn);
        }
    }

    /// <summary>
    /// 任務資料初始化
    /// </summary>
    /// <param name="questIDs"></param>
    public void QuestInit(params string[] questIDs)
    {
        questDataList = new List<QuestDataModel>();
        for (int i = 0; i < questIDs.Length; i++)
        {
            questDataList.Add(GameData.QuestDataDic[questIDs[i]]);
        }
        foreach (var quest in questDataList)
        {
            buttonText.text = quest.QuestName;
            Button btn = Instantiate(buttonObj, buttonParent);
            Action act = () => RunQuest(quest);
            btn.onClick.AddListener(() => act.Invoke());
            btn.gameObject.SetActive(true);
            buttonRecordList.Add(btn);
        }
    }

    #region 按鈕功能

    /// <summary>
    /// 清除此次NPC產生資料(將視窗資料還原初始)
    /// </summary>
    public void ClearTempData()
    {
        if (buttonRecordList.Count > 0)
        {
            buttonRecordList.ForEach(x => Destroy(x.gameObject));
        }
        buttonRecordList.Clear();
    }

    /// <summary>
    /// 執行任務
    /// </summary>
    public void RunQuest(QuestDataModel questData)
    {
        questStep = 0;
        questStep = questData.QuestChatContent.Count;
        tempQuestData = questData;
        ClearTempData();
        QuestStepFunction();
    }

    /// <summary>
    /// 任務步驟執行
    /// </summary>
    public void QuestStepFunction()
    {
        if (tempStep < tempQuestData.QuestChatContent.Count - 1)
        {
            chatContent.text = tempQuestData.QuestChatContent[tempStep];
            ClearTempData();
            ButtonFunction buttonFunction1 = ButtonFunctionList.Find(x => x.ButtonID == "QuestContinue");
            buttonText.text = "繼續";
            Button btn1 = Instantiate(buttonObj, buttonParent);
            Action act1 = () => buttonFunction1.ButtonAct.Invoke();
            btn1.onClick.AddListener(() => act1.Invoke());
            btn1.gameObject.SetActive(true);
            buttonRecordList.Add(btn1);

        } //若任務還有繼續的交談
        else
        {
            chatContent.text = tempQuestData.QuestChatContent[tempStep];
            ClearTempData();
            ButtonFunction buttonFunction1 = ButtonFunctionList.Find(x => x.ButtonID == "QuestAccept");
            buttonText.text = "接受";
            Button btn1 = Instantiate(buttonObj, buttonParent);
            Action act1 = () => buttonFunction1.ButtonAct.Invoke();
            btn1.onClick.AddListener(() => act1.Invoke());
            btn1.gameObject.SetActive(true);
            buttonRecordList.Add(btn1);
            ButtonFunction buttonFunction2 = ButtonFunctionList.Find(x => x.ButtonID == "QuestReject");
            buttonText.text = "拒絕";
            Button btn2 = Instantiate(buttonObj, buttonParent);
            Action act2 = () => buttonFunction2.ButtonAct.Invoke();
            btn2.onClick.AddListener(() => act2.Invoke());
            btn2.gameObject.SetActive(true);
            buttonRecordList.Add(btn2);

        }
    }

    /// <summary>
    /// 繼續任務對話
    /// </summary>
    public void ContinusQuest()
    {
        tempStep += 1;
        QuestStepFunction();
    }

    /// <summary>
    /// 接受任務處理
    /// </summary>
    public void AcceptQuest()
    {
        MissionManager.Instance.AcceptMisstion(tempQuestData.QuestID, tempQuestData.QuestConditionList);
        Exit();
        Debug.Log("接受任務:" + tempQuestData);
    }

    /// <summary>
    /// 拒絕任務
    /// </summary>
    public void RejectQuest()
    {
        Exit();
        Debug.Log("拒絕任務:" + tempQuestData);
    }

    /// <summary>
    /// 離開對話
    /// </summary>
    public void Exit()
    {
        tempQuestData = null;
        tempStep = 0;
        gameObject.SetActive(false);
        ClearTempData();
    }
    #endregion
}

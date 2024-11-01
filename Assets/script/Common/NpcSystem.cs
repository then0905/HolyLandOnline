using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
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

[Serializable]
public struct RewardImage
{
    public GameObject Obj;
    public Image Sprite;
    public TextMeshProUGUI Text;
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
    [SerializeField] private RewardImage rewardImage;      //獎勵道具物件
    [SerializeField] private TextMeshProUGUI characterName;      //NPC名稱
    [SerializeField] private Button buttonObj;      //按鈕參考
    [SerializeField] private TextMeshProUGUI buttonText;      //按鈕文字
    [SerializeField] private Transform buttonParent;      //生成父級 按鈕參考
    [SerializeField] private TextMeshProUGUI coinReward;      //獎勵文字:金幣
    [SerializeField] private TextMeshProUGUI expReward;      //獎勵文字:經驗
    [SerializeField] private Transform itemRewardParent;      //獎勵道具生成參考
    [SerializeField] private RectTransform rewardArea;      //獎勵區RectTransform參考


    [Header("遊戲資料"), SerializeField]
    public List<ButtonFunction> ButtonFunctionList = new List<ButtonFunction>();        //按鈕功能清單
    private List<Button> buttonRecordList = new List<Button>();     //紀錄已生成的按鈕清單
    private List<GameObject> tempRewardItemList = new List<GameObject>();     //紀錄已生成的獎勵圖片清單
    private List<QuestDataModel> questDataList = new List<QuestDataModel>();     //NPC所有任務清單
    private List<ShopInventoryData> npcTradeItemList = new List<ShopInventoryData>();     //NPC所有商品清單
    private QuestDataModel tempQuestData = null;       //暫存此次任務資料
    private int questStep;      //紀錄任務總步驟
    private int tempStep = 0;      //紀錄任務當前步驟
    /// <summary>
    /// 初始化 NPC 互動頁面
    /// </summary>
    /// <param name="npcData"></param>
    public void InitNpcSystem(NpcDataModel npcData)
    {
        //若視窗已經是顯示狀態 Return
        if (gameObject.activeSelf) return;

        //設定NPC預設對話
        chatContent.text = npcData.NpcChatContent[0].GetText();
        //設定NPC頭像
        avatar.sprite = CommonFunction.LoadObject<Sprite>(npcData.NpcAvatarPath, npcData.NpcAvatarName);
        //設定NPC名稱
        characterName.text = npcData.NpcName.GetText();
        //設定按鈕事件
        ButtonFunctionSetting(npcData.NpcButtonFuncList);
        //設定NPC販賣項目清單
        npcTradeItemList = npcData.ShopInventoryList ?? npcData.ShopInventoryList;
        //顯示視窗
        gameObject.SetActive(true);

        //取得NPC任務資料
        var getNpcQuest = GameData.QuestDataDic.Values.Where(x => x.StartNpcID == npcData.NpcID).ToList();
        if (getNpcQuest.CheckAnyData())
        {
            //查詢階段1 排除玩家接取的任務
            var queryResult1 = getNpcQuest.Where(x => !MissionManager.Instance.AlllMission.Any(y => y.QuestData.QuestID == x.QuestID)).ToList();
            //查詢階段2 排除玩家完成的任務
            var queryResult2 = queryResult1.Where(x => !MissionManager.Instance.FinishedMissionList.Any(y => y == x.QuestID)).Select(x => x.QuestID).ToList();
            QuestInit(queryResult2.ToArray());
            //檢查玩家是否身上有已完成的任務
            FinishedQuestInit(npcData);
        }
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
            buttonText.text = btnList[i].ButtonName.GetText();
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
            buttonText.text = quest.QuestName.GetText();
            Button btn = Instantiate(buttonObj, buttonParent);
            Action act = () => RunQuest(quest);
            btn.onClick.AddListener(() => act.Invoke());
            btn.gameObject.SetActive(true);
            buttonRecordList.Add(btn);
        }
    }

    /// <summary>
    /// 完成任務的初始化
    /// </summary>
    public void FinishedQuestInit(NpcDataModel npcData)
    {
        //查詢玩家以接取的資料
        var queryResult = MissionManager.Instance.AlllMission.Where(x => x.GetMissionFinishStatus && x.QuestData.EndNpcID == npcData.NpcID).ToList();

        if (queryResult.CheckAnyData())
        {
            foreach (var quest in queryResult)
            {
                buttonText.text = quest.QuestData.QuestName.GetText();
                Button btn = Instantiate(buttonObj, buttonParent);
                Action act = () => FinishedQuest(quest.QuestData);
                btn.onClick.AddListener(() => act.Invoke());
                btn.gameObject.SetActive(true);
                buttonRecordList.Add(btn);
            }
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
        QuestStepFunction(true);
    }

    /// <summary>
    /// 完成任務
    /// </summary>
    /// <param name="questData"></param>
    public void FinishedQuest(QuestDataModel questData)
    {
        questStep = 0;
        questStep = questData.QuestFinishList.Count;
        tempQuestData = questData;
        ClearTempData();
        QuestStepFunction(false);
    }

    /// <summary>
    /// 任務步驟執行
    /// </summary>
    /// <param name="missionStatus">任務狀態 True:接取 False:完成</param>
    public void QuestStepFunction(bool missionStatus)
    {
        if (missionStatus)
        {
            //若任務還有繼續的交談
            if (tempStep < tempQuestData.QuestChatContent.Count - 1)
            {
                chatContent.text = tempQuestData.QuestChatContent[tempStep].GetText();
                ClearTempData();
                //ButtonFunction buttonFunction1 = ButtonFunctionList.Find(x => x.ButtonID == "QuestContinue");
                buttonText.text = "TM_Continue".GetText();
                Button btn1 = Instantiate(buttonObj, buttonParent);
                Action act1 = () => ContinusQuest(missionStatus);
                btn1.onClick.AddListener(() => act1.Invoke());
                btn1.gameObject.SetActive(true);
                buttonRecordList.Add(btn1);

            }
            else
            {
                chatContent.text = tempQuestData.QuestChatContent[tempStep].GetText();
                ClearTempData();
                ButtonFunction buttonFunction1 = ButtonFunctionList.Find(x => x.ButtonID == "QuestAccept");
                buttonText.text = "TM_Accept".GetText();
                Button btn1 = Instantiate(buttonObj, buttonParent);
                Action act1 = () => buttonFunction1.ButtonAct.Invoke();
                btn1.onClick.AddListener(() => act1.Invoke());
                btn1.gameObject.SetActive(true);
                buttonRecordList.Add(btn1);
                ButtonFunction buttonFunction2 = ButtonFunctionList.Find(x => x.ButtonID == "QuestReject");
                buttonText.text = "TM_Reject".GetText();
                Button btn2 = Instantiate(buttonObj, buttonParent);
                Action act2 = () => buttonFunction2.ButtonAct.Invoke();
                btn2.onClick.AddListener(() => act2.Invoke());
                btn2.gameObject.SetActive(true);
                buttonRecordList.Add(btn2);
            }
        }
        else
        {
            var getFinishData = tempQuestData.QuestFinishList[0];
            if (tempStep < getFinishData.QuestChatContent.Count - 1)
            {
                chatContent.text = getFinishData.QuestChatContent[tempStep].GetText();
                ClearTempData();
                //ButtonFunction buttonFunction1 = ButtonFunctionList.Find(x => x.ButtonID == "QuestContinue");
                buttonText.text = "TM_Continue".GetText();
                Button btn1 = Instantiate(buttonObj, buttonParent);
                Action act1 = () => ContinusQuest(missionStatus);
                btn1.onClick.AddListener(() => act1.Invoke());
                btn1.gameObject.SetActive(true);
                buttonRecordList.Add(btn1);

            }
            else
            {
                chatContent.text = getFinishData.QuestChatContent[tempStep].GetText();
                //設定金幣文字
                coinReward.enabled = true;
                coinReward.text = "TM_Coin".GetText(true) + tempQuestData.Coin.ToString();
                //設定經驗文字
                expReward.enabled = true;
                expReward.text = "TM_Exp".GetText(true) + tempQuestData.Exp.ToString();

                //若有獎勵資料 設定獎勵物件
                if (tempQuestData.QuestRewardList.CheckAnyData())
                {
                    foreach (var item in tempQuestData.QuestRewardList)
                    {
                        var itemData = GameData.ItemsDic.Where(x => x.Key == item.RewardID).Select(x => x.Value).FirstOrDefault();
                        var weaponData = GameData.WeaponsDic.Where(x => x.Key == item.RewardID).Select(x => x.Value).FirstOrDefault();
                        var armorData = GameData.ArmorsDic.Where(x => x.Key == item.RewardID).Select(x => x.Value).FirstOrDefault();
                        if (itemData != null)
                            rewardImage.Sprite.sprite = CommonFunction.LoadObject<Sprite>(GameConfig.SpriteItem, item.RewardID);
                        if (weaponData != null)
                            rewardImage.Sprite.sprite = CommonFunction.LoadObject<Sprite>(GameConfig.SpriteWeapon, weaponData.CodeID);
                        if (armorData != null)
                            rewardImage.Sprite.sprite = CommonFunction.LoadObject<Sprite>(GameConfig.SpriteArmor, armorData.CodeID);
                        rewardImage.Text.text = item.RewardQty.ToString();
                        GameObject obj = Instantiate(rewardImage.Obj, itemRewardParent);
                        obj.SetActive(true);
                        tempRewardItemList.Add(obj);

                    }
                }
                //強迫重新布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(itemRewardParent.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(rewardArea);

                ClearTempData();
                buttonText.text = "TM_ClaimReward".GetText();
                Button btn2 = Instantiate(buttonObj, buttonParent);
                btn2.onClick.AddListener(() => GetMissionReward(tempQuestData));
                btn2.gameObject.SetActive(true);
                buttonRecordList.Add(btn2);
            }
        }
    }

    /// <summary>
    /// 繼續任務對話
    /// </summary>
    /// <param name="missionStatus">帶入任務狀態 True:接取 False:完成</param>
    public void ContinusQuest(bool missionStatus)
    {
        tempStep += 1;
        QuestStepFunction(missionStatus);
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
    /// 任務完成領取獎勵處理
    /// </summary>
    /// <param name="questData"></param>
    public void GetMissionReward(QuestDataModel questData)
    {
        //經驗值獎勵
        PlayerDataOverView.Instance.PlayerData_.Exp += questData.Exp;
        PlayerDataOverView.Instance.ExpProcessor();
        //金幣獎勵
        ItemManager.Instance.PickUp(questData.Coin);
        //道具獎勵
        if (questData.QuestRewardList.CheckAnyData())
        {
            foreach (var item in questData.QuestRewardList)
            {
                var itemData = GameData.ItemsDic.Where(x => x.Key == item.RewardID).Select(x => x.Value).FirstOrDefault();
                var weaponData = GameData.WeaponsDic.Where(x => x.Key == item.RewardID).Select(x => x.Value).FirstOrDefault();
                var armorData = GameData.ArmorsDic.Where(x => x.Key == item.RewardID).Select(x => x.Value).FirstOrDefault();
                if (itemData != null)
                    ItemManager.Instance.PickUp(CommonFunction.LoadObject<Sprite>(GameConfig.SpriteItem, itemData.CodeID), itemData);
                if (weaponData != null)
                    ItemManager.Instance.PickUp(CommonFunction.LoadObject<Sprite>(GameConfig.SpriteWeapon, weaponData.CodeID), weaponData);
                if (armorData != null)
                    ItemManager.Instance.PickUp(CommonFunction.LoadObject<Sprite>(GameConfig.SpriteArmor, armorData.CodeID), armorData);
            }
        }
        //清除任務獎勵圖示
        if (tempRewardItemList.Count > 0)
        {
            tempRewardItemList.ForEach(x => Destroy(x));
            tempRewardItemList = new List<GameObject>();
        }
        coinReward.enabled = false;
        expReward.enabled = false;
        //紀錄完成任務ID
        MissionManager.Instance.FinishedMisstion(questData);
        Exit();
    }

    /// <summary>
    /// 呼叫交易介面
    /// </summary>
    public void CallTradePanel()
    {
        TradeManager.Instance.Init(npcTradeItemList.Select(x => x.ProductID).ToList(), ItemManager.Instance.BagItems);
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

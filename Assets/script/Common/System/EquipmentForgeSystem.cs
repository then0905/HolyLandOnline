using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/11/05
//  創建用途: 裝備強化系統
//==========================================
public class EquipmentForgeSystem : MonoBehaviour
{
    #region 靜態變數

    private static EquipmentForgeSystem instance;
    public static EquipmentForgeSystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<EquipmentForgeSystem>(true);
            return instance;
        }
    }

    #endregion

    [Header("遊戲物件")]
    [SerializeField] private Image targetEquipmentImg;      //強化物件圖片
    [SerializeField] private TextMeshProUGUI targetEquipmentName;       //強化物件名稱
    [SerializeField] private TextMeshProUGUI successRateText;       //成功率文字
    [SerializeField] private TextMeshProUGUI destroyedRateText;     //裝備損壞率文字
    [SerializeField] private TextMeshProUGUI currentForgeLvText;        //當前強化等級文字
    [SerializeField] private TextMeshProUGUI currentForgeStatus;        //當前強化等級效果文字
    [SerializeField] private TextMeshProUGUI nextForgeLvText;       //下一強化等級文字
    [SerializeField] private TextMeshProUGUI nextForgeStatus;       //下一強化等級效果文字
    [SerializeField] private GameObject successWindow;      //強化成功視窗
    [SerializeField] private GameObject failWindow;     //強化失敗視窗
    [SerializeField] private GameObject destroyedWindow;        //物件損壞視窗
    [SerializeField] private GameObject forgeButton;        //強化按鈕
    [SerializeField] private GameObject cancelButton;        //取消強化按鈕

    [SerializeField] private Equipment playerEquipTradeItem;       //生成玩家交易物品資料的物件
    [SerializeField] private Transform playerEquipTradeItemTrans;       //生成玩家交易物品資料的參考
    [SerializeField] private TextMeshProUGUI playerCoinText;       //玩家的金幣文字

    private List<Equipment> tempPlayerEquipTradeItemList = new List<Equipment>();

    [Header("遊戲資料")]
    [SerializeField] private Sprite bagItemOriginImage; //空背包圖
    private Equipment currentEqquipment;       //目前點選的強化物件
    //背包資料
    private List<Equipment> itemList => ItemManager.Instance.BagItems;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="playerItemList"></param>
    public void Init()
    {
        ClearData();
        //獲取最大生成數量
        int bagMaxCount = itemList.Count;
        //玩家物件生成區
        for (int i = 0; i < bagMaxCount; i++)
        {
            Equipment tempData = Instantiate(playerEquipTradeItem, playerEquipTradeItemTrans);
            tempData.gameObject.SetActive(true);
            if (itemList.Count >= i)
            {
                tempData.EquipmentDatas = itemList[i].EquipmentDatas;
                tempData.EquipImage.sprite = CommonFunction.GetItemSprite(tempData.EquipmentDatas) == null ?
                    bagItemOriginImage : CommonFunction.GetItemSprite(tempData.EquipmentDatas);
                tempData.Qty = itemList[i].Qty;
            }
            tempPlayerEquipTradeItemList.Add(tempData);
        }
        //玩家金幣
        playerCoinText.text = PlayerDataOverView.Instance.PlayerData_.Coin.ToString();
        PanelManager.Instance.SetPanelOpen("ForgePanel");
    }

    /// <summary>
    /// 清除生成的資料
    /// </summary>
    public void ClearData()
    {
        if (tempPlayerEquipTradeItemList.Count > 0)
        {
            tempPlayerEquipTradeItemList.ForEach(x => Destroy(x.gameObject));
            tempPlayerEquipTradeItemList.Clear();
        }
    }

    /// <summary>
    /// 設定鍛造目標資料
    /// </summary>
    private void SettingForgeTarget(Equipment equipment)
    {
        if (equipment.EquipmentDatas.ItemCommonData is ItemDataModel)
        {
            CommonFunction.MessageHint("TM_ForgeError_CantForge".GetText(), HintType.Warning);
            return;
        }
        //暫存資料
        currentEqquipment = equipment;
        //設定物品圖片
        targetEquipmentImg.sprite = currentEqquipment.EquipImage.sprite;
        //設定物品文字
        targetEquipmentName.enabled = true;
        targetEquipmentName.text = currentEqquipment.EquipmentDatas.ItemCommonData.Name.GetText();
        //選擇物品的強化等級
        int currentForgeLv = currentEqquipment.EquipmentDatas.ForceLv;
        //取得強化資料清單
        List<ForgeData> forgeDatas = equipment.EquipmentDatas.Weapon != null ? equipment.EquipmentDatas.Weapon.ForgeConfigList : equipment.EquipmentDatas.Armor.ForgeConfigList;
        //當前物品的強化等級資料
        ForgeData currentForgeData = forgeDatas.Where(x => x.ForgeLv == (currentForgeLv)).FirstOrDefault();
        //當前物品的下一強化等級資料
        ForgeData nextForgeData = forgeDatas.Where(x => x.ForgeLv == (currentForgeLv + 1)).FirstOrDefault();
        currentForgeLvText.enabled = true;
        currentForgeLvText.text = string.Format("TM_ForgeLV".GetText(), currentForgeLv.ToString());
        //當前等級
        currentForgeStatus.enabled = currentForgeData != null;
        if (currentForgeData != null)
            currentForgeStatus.text = forgeStatusTextProcessor(currentForgeData);

        //下一等級相關資訊
        successRateText.enabled = nextForgeData != null;
        destroyedRateText.enabled = nextForgeData != null;
        if (nextForgeData != null)
        {
            successRateText.text = string.Format("TM_ForgeSuccessRate".GetText(), nextForgeData.SuccessProbability.ToString());
            destroyedRateText.text = string.Format("TM_ForgeDestroyedRate".GetText(), nextForgeData.DestroyedProbability.ToString());
            nextForgeLvText.enabled = true;
            nextForgeLvText.text = string.Format("TM_NextForgeLV".GetText(), (currentForgeLv + 1).ToString());
            nextForgeStatus.enabled = true;
            nextForgeStatus.text = forgeStatusTextProcessor(nextForgeData);
        }
        forgeButton.SetActive(true);
        cancelButton.SetActive(true);
    }

    /// <summary>
    /// 獲取強化屬性文字內容
    /// </summary>
    /// <param name="forgeData"></param>
    /// <returns></returns>
    private string forgeStatusTextProcessor(ForgeData forgeData)
    {
        string temp;
        if (GameData.WeaponsDic[forgeData.CodeID] != null)
            temp =
            "TM_MaxHP".GetText(true) + forgeData.HP + "\n" +
            "TM_MaxMP".GetText(true) + forgeData.MP + "\n" +
            "TM_MeleeATK".GetText(true) + forgeData.MeleeATK + "\n" +
            "TM_RemoteATK".GetText(true) + forgeData.RemoteATK + "\n" +
            "TM_MageATK".GetText(true) + forgeData.MageATK + "\n" +
            "TM_MeleeHit".GetText(true) + forgeData.MeleeHit + "\n" +
            "TM_RemoteHit".GetText(true) + forgeData.RemoteHit + "\n" +
            "TM_MageHit".GetText(true) + forgeData.MageHit + "\n" +
            "TM_DEF".GetText(true) + forgeData.DEF + "\n" +
            "TM_MDEF".GetText(true) + forgeData.MDEF + "\n" +
            "TM_DamageReduction".GetText(true) + forgeData.DamageReduction + "\n" +
            "TM_Avoid".GetText(true) + forgeData.Avoid + "\n" +
            "TM_Crt".GetText(true) + forgeData.Crt + "\n" +
            "TM_CrtDamage".GetText(true) + forgeData.CrtDamage + "\n" +
            "TM_STR".GetText(true) + forgeData.STR + "\n" +
            "TM_DEX".GetText(true) + forgeData.DEX + "\n" +
            "TM_INT".GetText(true) + forgeData.INT + "\n" +
            "TM_AGI".GetText(true) + forgeData.AGI + "\n" +
            "TM_VIT".GetText(true) + forgeData.VIT + "\n" +
            "TM_WIS".GetText(true) + forgeData.WIS + "\n" +
            "TM_ElementDamageIncrease".GetText(true) + forgeData.ElementDamageIncrease + "\n" +
            "TM_BlockRate".GetText(true) + forgeData.BlockRate + "\n";
        else
            temp =
            "TM_MaxHP".GetText(true) + forgeData.HP + "\n" +
            "TM_HP_Recovery".GetText(true) + forgeData.HpRecovery + "\n" +
            "TM_MaxMP".GetText(true) + forgeData.MP + "\n" +
            "TM_MP_Recovery".GetText(true) + forgeData.MpRecovery + "\n" +
            "TM_DEF".GetText(true) + forgeData.DEF + "\n" +
            "TM_MDEF".GetText(true) + forgeData.MDEF + "\n" +
            "TM_DamageReduction".GetText(true) + forgeData.DamageReduction + "\n" +
            "TM_Avoid".GetText(true) + forgeData.Avoid + "\n" +
            "TM_CrtResistance".GetText(true) + forgeData.CrtResistance + "\n" +
            "TM_Speed".GetText(true) + forgeData.Speed + "\n" +
            "TM_STR".GetText(true) + forgeData.STR + "\n" +
            "TM_DEX".GetText(true) + forgeData.DEX + "\n" +
            "TM_INT".GetText(true) + forgeData.INT + "\n" +
            "TM_AGI".GetText(true) + forgeData.AGI + "\n" +
            "TM_VIT".GetText(true) + forgeData.VIT + "\n" +
            "TM_WIS".GetText(true) + forgeData.WIS + "\n" +
            "TM_ElementDamageReduction".GetText(true) + forgeData.ElementDamageReduction + "\n" +
            "TM_DisorderResistance".GetText(true) + forgeData.DisorderResistance + "\n";
        return temp;
    }

    #region 按鈕事件

    /// <summary>
    /// 清除當前強化資料
    /// </summary>
    public void ClearForgeData()
    {
        currentEqquipment = null;
        targetEquipmentImg.sprite = bagItemOriginImage;
        successRateText.enabled = false;
        destroyedRateText.enabled = false;
        currentForgeStatus.enabled = false;
        targetEquipmentName.enabled = false;
        currentForgeLvText.enabled = false;
        nextForgeLvText.enabled = false;
        nextForgeStatus.enabled = false;
        forgeButton.SetActive(false);
        cancelButton.SetActive(false);
    }

    /// <summary>
    /// 進行強化
    /// </summary>
    public void Forge()
    {
        //取得下一等級資料
        int nextLv = currentEqquipment.EquipmentDatas.ForceLv + 1;
        if (nextLv > 10)
        {
            CommonFunction.MessageHint("TM_ForgeError_LvLimit".GetText(), HintType.Warning);
            return;
        }
        //取得強化資料清單
        List<ForgeData> forgeDatas = currentEqquipment.EquipmentDatas.Weapon != null ? currentEqquipment.EquipmentDatas.Weapon.ForgeConfigList : currentEqquipment.EquipmentDatas.Armor.ForgeConfigList;
        //強化資料
        ForgeData currentForgeData = forgeDatas.Where(x => x.ForgeLv == (nextLv)).FirstOrDefault();
        //取得成功率
        float successRate = currentForgeData.SuccessProbability;
        //取得隨機值
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if (successRate >= randomValue)
        {
            Debug.Log($"物品強化成功率{successRate}  && 隨機值{randomValue}");
            //強化成功 
            successWindow.SetActive(true);
            currentEqquipment.EquipmentDatas.ForceLv += 1;
            SettingForgeTarget(currentEqquipment);
        }
        else
        {
            //取得失敗裝備損壞率
            float destroyedRate = currentForgeData.DestroyedProbability;
            //取得隨機值
            randomValue = UnityEngine.Random.Range(0f, 100f);
            if(destroyedRate >= randomValue)
            {
                Debug.Log($"失敗裝備損壞率{destroyedRate}  && 隨機值{randomValue}");
                //裝備毀損
                ItemManager.Instance.RemoveItem(currentEqquipment.EquipmentDatas.ItemCommonData.CodeID);
                destroyedWindow.SetActive(true);
                ClearForgeData();
                Init();
            }
            else
            {
                failWindow.SetActive(true);
                SettingForgeTarget(currentEqquipment);
            }       
        }
    }

    #endregion

    #region 觸發事件

    /// <summary>
    /// 點擊事件
    /// </summary>
    /// <param name="data"></param>
    public void OnClick(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;
        Equipment tempData = data.pointerClick.gameObject.GetComponent<Equipment>();
        SettingForgeTarget(tempData);
    }

    #endregion
}

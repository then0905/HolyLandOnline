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
    [SerializeField] private TMP_Dropdown enhanceItemDropdown;        //選擇強化石類型

    [SerializeField] private Equipment playerEquipTradeItem;       //生成玩家交易物品資料的物件
    [SerializeField] private Transform playerEquipTradeItemTrans;       //生成玩家交易物品資料的參考
    [SerializeField] private TextMeshProUGUI playerCoinText;       //玩家的金幣文字

    private List<Equipment> tempPlayerEquipTradeItemList = new List<Equipment>();

    [Header("遊戲資料")]
    [SerializeField] private Sprite bagItemOriginImage; //空背包圖
    private Equipment currentEqquipment;       //目前點選的強化物件
    private string currenntUesdEnhanceItem;     //當前使用的強化道具ID
    private int currentEnhandceDropdown;        //紀錄當前選擇的強化道具選擇(下拉是選單)
    //背包資料
    private List<Equipment> itemList => ItemManager.Instance.BagItems;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="playerItemList"></param>
    public void Init()
    {
        InitDropdown();
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
        if (equipment == null) return;
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
        ForgeData nextForgeData = forgeDatas.Where(x => x.ForgeLv == (currenntUesdEnhanceItem == "Enhance_04" ? (currentForgeLv - 1) : (currentForgeLv + 1))).FirstOrDefault();
        currentForgeLvText.enabled = true;
        currentForgeLvText.text = string.Format("TM_ForgeLV".GetText(), currentForgeLv.ToString());
        //當前等級
        currentForgeStatus.enabled = currentForgeData != null;
        if (currentForgeData != null)
            currentForgeStatus.text = CurrentForgeStatusTextProcessor(currentForgeData);

        //下一等級相關資訊
        successRateText.enabled = nextForgeData != null;
        destroyedRateText.enabled = nextForgeData != null;
        nextForgeStatus.enabled = nextForgeData != null;
        nextForgeLvText.enabled = nextForgeData != null;
        if (nextForgeData != null)
        {
            successRateText.text = string.Format("TM_ForgeSuccessRate".GetText(), ReturnSuccessRate(nextForgeData).ToString());
            destroyedRateText.text = string.Format("TM_ForgeDestroyedRate".GetText(), ReturnDestroyedRate(nextForgeData).ToString());
            nextForgeLvText.text = string.Format("TM_NextForgeLV".GetText(), (currenntUesdEnhanceItem == "Enhance_04" ? (currentForgeLv - 1) : (currentForgeLv + 1)).ToString());
            nextForgeStatus.text = NextForgeStatusTextProcessor(currentForgeData, nextForgeData);
        }
        forgeButton.SetActive(true);
        cancelButton.SetActive(true);
    }

    /// <summary>
    /// 獲取當前強化屬性文字內容
    /// </summary>
    /// <param name="currentForgeLvData">當前強化等級的屬性資料</param>
    /// <returns></returns>
    private string CurrentForgeStatusTextProcessor(ForgeData currentForgeLvData)
    {
        string temp;
        if (GameData.WeaponsDic[currentForgeLvData.CodeID] != null)
            temp =
            "TM_MaxHP".GetText(true) + currentForgeLvData.HP + "\n" +
            "TM_MaxMP".GetText(true) + currentForgeLvData.MP + "\n" +
            "TM_MeleeATK".GetText(true) + currentForgeLvData.MeleeATK + "\n" +
            "TM_RemoteATK".GetText(true) + currentForgeLvData.RemoteATK + "\n" +
            "TM_MageATK".GetText(true) + currentForgeLvData.MageATK + "\n" +
            "TM_MeleeHit".GetText(true) + currentForgeLvData.MeleeHit + "\n" +
            "TM_RemoteHit".GetText(true) + currentForgeLvData.RemoteHit + "\n" +
            "TM_MageHit".GetText(true) + currentForgeLvData.MageHit + "\n" +
            "TM_DEF".GetText(true) + currentForgeLvData.DEF + "\n" +
            "TM_MDEF".GetText(true) + currentForgeLvData.MDEF + "\n" +
            "TM_DamageReduction".GetText(true) + currentForgeLvData.DamageReduction + "\n" +
            "TM_Avoid".GetText(true) + currentForgeLvData.Avoid + "\n" +
            "TM_Crt".GetText(true) + currentForgeLvData.Crt + "\n" +
            "TM_CrtDamage".GetText(true) + currentForgeLvData.CrtDamage + "\n" +
            "TM_STR".GetText(true) + currentForgeLvData.STR + "\n" +
            "TM_DEX".GetText(true) + currentForgeLvData.DEX + "\n" +
            "TM_INT".GetText(true) + currentForgeLvData.INT + "\n" +
            "TM_AGI".GetText(true) + currentForgeLvData.AGI + "\n" +
            "TM_VIT".GetText(true) + currentForgeLvData.VIT + "\n" +
            "TM_WIS".GetText(true) + currentForgeLvData.WIS + "\n" +
            "TM_ElementDamageIncrease".GetText(true) + currentForgeLvData.ElementDamageIncrease + "\n" +
            "TM_BlockRate".GetText(true) + currentForgeLvData.BlockRate + "\n";
        else
            temp =
            "TM_MaxHP".GetText(true) + currentForgeLvData.HP + "\n" +
            "TM_HP_Recovery".GetText(true) + currentForgeLvData.HpRecovery + "\n" +
            "TM_MaxMP".GetText(true) + currentForgeLvData.MP + "\n" +
            "TM_MP_Recovery".GetText(true) + currentForgeLvData.MpRecovery + "\n" +
            "TM_DEF".GetText(true) + currentForgeLvData.DEF + "\n" +
            "TM_MDEF".GetText(true) + currentForgeLvData.MDEF + "\n" +
            "TM_DamageReduction".GetText(true) + currentForgeLvData.DamageReduction + "\n" +
            "TM_Avoid".GetText(true) + currentForgeLvData.Avoid + "\n" +
            "TM_CrtResistance".GetText(true) + currentForgeLvData.CrtResistance + "\n" +
            "TM_Speed".GetText(true) + currentForgeLvData.Speed + "\n" +
            "TM_STR".GetText(true) + currentForgeLvData.STR + "\n" +
            "TM_DEX".GetText(true) + currentForgeLvData.DEX + "\n" +
            "TM_INT".GetText(true) + currentForgeLvData.INT + "\n" +
            "TM_AGI".GetText(true) + currentForgeLvData.AGI + "\n" +
            "TM_VIT".GetText(true) + currentForgeLvData.VIT + "\n" +
            "TM_WIS".GetText(true) + currentForgeLvData.WIS + "\n" +
            "TM_ElementDamageReduction".GetText(true) + currentForgeLvData.ElementDamageReduction + "\n" +
            "TM_DisorderResistance".GetText(true) + currentForgeLvData.DisorderResistance + "\n";
        return temp;
    }

    /// <summary>
    ///  獲取下一等級強化屬性文字內容
    /// </summary>
    /// <param name="currentForgeLvData">當前強化等級屬性資料</param>
    /// <param name="nextForgeLvData">下一強化等級屬性資料</param>
    /// <returns></returns>
    private string NextForgeStatusTextProcessor(ForgeData currentForgeLvData, ForgeData nextForgeLvData)
    {
        string temp;
        if (GameData.WeaponsDic[nextForgeLvData.CodeID] != null)
            temp =
            "TM_MaxHP".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.HP), nextForgeLvData.HP) + "\n" +
            "TM_MaxMP".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MP), nextForgeLvData.MP) + "\n" +
            "TM_MeleeATK".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MeleeATK), nextForgeLvData.MeleeATK) + "\n" +
            "TM_RemoteATK".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.RemoteATK), nextForgeLvData.RemoteATK) + "\n" +
            "TM_MageATK".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MageATK), nextForgeLvData.MageATK) + "\n" +
            "TM_MeleeHit".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MeleeHit), nextForgeLvData.MeleeHit) + "\n" +
            "TM_RemoteHit".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.RemoteHit), nextForgeLvData.RemoteHit) + "\n" +
            "TM_MageHit".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MageHit), nextForgeLvData.MageHit) + "\n" +
            "TM_DEF".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.DEF), nextForgeLvData.DEF) + "\n" +
            "TM_MDEF".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MDEF), nextForgeLvData.MDEF) + "\n" +
            "TM_DamageReduction".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.DamageReduction), nextForgeLvData.DamageReduction) + "\n" +
            "TM_Avoid".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.Avoid), nextForgeLvData.Avoid) + "\n" +
            "TM_Crt".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.Crt), nextForgeLvData.Crt) + "\n" +
            "TM_CrtDamage".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.CrtDamage), nextForgeLvData.CrtDamage) + "\n" +
            "TM_STR".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.STR), nextForgeLvData.STR) + "\n" +
            "TM_DEX".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.DEX), nextForgeLvData.DEX) + "\n" +
            "TM_INT".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.INT), nextForgeLvData.INT) + "\n" +
            "TM_AGI".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.AGI), nextForgeLvData.AGI) + "\n" +
            "TM_VIT".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.VIT), nextForgeLvData.VIT) + "\n" +
            "TM_WIS".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.WIS), nextForgeLvData.WIS) + "\n" +
            "TM_ElementDamageIncrease".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.ElementDamageIncrease), nextForgeLvData.ElementDamageIncrease) + "\n" +
            "TM_BlockRate".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.BlockRate), nextForgeLvData.BlockRate) + "\n";
        else
            temp =
            "TM_MaxHP".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.HP), nextForgeLvData.HP) + "\n" +
            "TM_HP_Recovery".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.HpRecovery), nextForgeLvData.HpRecovery) + "\n" +
            "TM_MaxMP".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MP), nextForgeLvData.MP) + "\n" +
            "TM_MP_Recovery".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MpRecovery), nextForgeLvData.MpRecovery) + "\n" +
            "TM_DEF".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.DEF), nextForgeLvData.DEF) + "\n" +
            "TM_MDEF".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.MDEF), nextForgeLvData.MDEF) + "\n" +
            "TM_DamageReduction".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.DamageReduction), nextForgeLvData.DamageReduction) + "\n" +
            "TM_Avoid".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.Avoid), nextForgeLvData.Avoid) + "\n" +
            "TM_CrtResistance".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.CrtResistance), nextForgeLvData.CrtResistance) + "\n" +
            "TM_Speed".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.Speed), nextForgeLvData.Speed) + "\n" +
            "TM_STR".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.STR), nextForgeLvData.STR) + "\n" +
            "TM_DEX".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.DEX), nextForgeLvData.DEX) + "\n" +
            "TM_INT".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.INT), nextForgeLvData.INT) + "\n" +
            "TM_AGI".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.AGI), nextForgeLvData.AGI) + "\n" +
            "TM_VIT".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.VIT), nextForgeLvData.VIT) + "\n" +
            "TM_WIS".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.WIS), nextForgeLvData.WIS) + "\n" +
            "TM_ElementDamageReduction".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.ElementDamageReduction), nextForgeLvData.ElementDamageReduction) + "\n" +
            "TM_DisorderResistance".GetText(true) + ForgeStatusTextProcessor((currentForgeLvData == null ? 0 : currentForgeLvData.DisorderResistance), nextForgeLvData.DisorderResistance) + "\n";
        return temp;
    }

    /// <summary>
    /// 處理強化屬性值的文字顏色 根據變強變弱不變改變指定顏色
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="nextValue"></param>
    /// <returns></returns>
    private string ForgeStatusTextProcessor(float currentValue, float nextValue)
    {
        string tempText;
        if ((currentValue < nextValue))
        {
            tempText = $"<color=#1FFF00>{nextValue}</color>";
        }
        else if ((currentValue > nextValue))
        {
            tempText = $"<color=#ff0d0d>{nextValue}</color>";
        }
        else
        {
            tempText = nextValue.ToString();
        }
        return tempText;
    }

    /// <summary>
    /// 初始化強化石下拉選單
    /// </summary>
    private void InitDropdown()
    {
        //取得強化道具資料清單
        List<ItemDataModel> enhanceItemList = GameData.ItemsDic.Where(x => x.Value.TypeID == "EnhanceItem").Select(x => x.Value).ToList();
        enhanceItemDropdown.ClearOptions();
        enhanceItemDropdown.AddOptions(
    enhanceItemList.Select(id => new TMP_Dropdown.OptionData(id.Name.GetText())).ToList());

        //刷新選擇的下拉選單
        OnDropdownValueChanged(currentEnhandceDropdown);
    }

    /// <summary>
    /// 依照選擇的強化石 回傳成功率
    /// </summary>
    /// <param name="currentForgeData"></param>
    /// <returns></returns>
    private float ReturnSuccessRate(ForgeData currentForgeData)
    {
        float temp;
        switch (currenntUesdEnhanceItem)
        {
            case "Enhance_03":
            case "Enhance_01":
                temp = currentForgeData.SuccessProbability;
                break;
            case "Enhance_02":
                temp = currentForgeData.SuperiorSuccessProbability;
                break;
            case "Enhance_04":
                temp = 100;
                break;
            default:
                temp = 0;
                break;
        }
        return temp;
    }

    /// <summary>
    /// 依照選擇的強化石 回傳成功率
    /// </summary>
    /// <param name="currentForgeData"></param>
    /// <returns></returns>
    private float ReturnDestroyedRate(ForgeData currentForgeData)
    {
        float temp;
        switch (currenntUesdEnhanceItem)
        {
            case "Enhance_02":
            case "Enhance_01":
                temp = currentForgeData.DestroyedProbability;
                break;
            default:
            case "Enhance_03":
            case "Enhance_04":
                temp = 0;
                break;
        }
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
        if ((nextLv > 10&& currenntUesdEnhanceItem != "Enhance_04"))
        {
            CommonFunction.MessageHint("TM_ForgeError_LvUpLimit".GetText(), HintType.Warning);
            return;
        }
        else if ((nextLv.Equals(1) && currenntUesdEnhanceItem == "Enhance_04"))
        {
            CommonFunction.MessageHint("TM_ForgeError_LvDownLimit".GetText(), HintType.Warning);
            return;
        }

        //取得強化資料清單
        List<ForgeData> forgeDatas = currentEqquipment.EquipmentDatas.Weapon != null ? currentEqquipment.EquipmentDatas.Weapon.ForgeConfigList : currentEqquipment.EquipmentDatas.Armor.ForgeConfigList;
        //強化資料
        ForgeData currentForgeData = forgeDatas.Where(x => x.ForgeLv == (nextLv)).FirstOrDefault();
        //取得成功率
        float successRate = ReturnSuccessRate(currentForgeData);
        //取得隨機值
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if (successRate >= randomValue)
        {
            Debug.Log($"物品強化成功率{successRate}  && 隨機值{randomValue}");
            //強化成功 
            successWindow.SetActive(true);
            var queryResult = itemList.Where(x => x.EquipmentDatas == currentEqquipment.EquipmentDatas).FirstOrDefault();
            queryResult.EquipmentDatas.ForceLv += (currenntUesdEnhanceItem == "Enhance_04" ? -1 : 1);
            SettingForgeTarget(queryResult);
            LoadPlayerData.SaveUserData();
        }
        else
        {
            //取得失敗裝備損壞率
            float destroyedRate = ReturnDestroyedRate(currentForgeData);
            //取得隨機值
            randomValue = UnityEngine.Random.Range(0f, 100f);
            if (destroyedRate >= randomValue)
            {
                Debug.Log($"失敗裝備損壞率{destroyedRate}  && 隨機值{randomValue}");
                //裝備毀損
                ItemManager.Instance.RemoveItem(currentEqquipment.EquipmentDatas.ItemCommonData.CodeID);
                destroyedWindow.SetActive(true);
                ClearForgeData();
                LoadPlayerData.SaveUserData();
            }
            else
            {
                failWindow.SetActive(true);
                SettingForgeTarget(currentEqquipment);
            }
        }
        //扣除背包內的強化石
        ItemManager.Instance.RemoveItem(currenntUesdEnhanceItem, 1);
        //刷新所有資訊
        Init();
        //重新記錄背包資料
        LoadPlayerData.SaveUserData();
    }

    /// <summary>
    /// 強化石下拉選單改變時執行的事件
    /// </summary>
    /// <param name="index"></param>
    public void OnDropdownValueChanged(int index)
    {
        currentEnhandceDropdown = index;
        string selectedOption = enhanceItemDropdown.options[index].text;

        //紀錄當前所使用的強化石ID
        currenntUesdEnhanceItem =
     GameData.ItemsDic.Where(x => x.Value.Name.GetText() == selectedOption).Select(x => x.Key).FirstOrDefault();

        //設定強化按鈕是否可使用
        forgeButton.GetComponent<Button>().interactable = (itemList.Any(x => x.EquipmentDatas?.Item?.CodeID == currenntUesdEnhanceItem) ||
           (currenntUesdEnhanceItem == "Enhance_04" && currentEqquipment.EquipmentDatas.ForceLv.Equals(0)));
        //強化石不足夠的警告通知
        if (!forgeButton.GetComponent<Button>().interactable)
            CommonFunction.MessageHint("TM_ForgeError_EnhanceItemNotEnough".GetText(), HintType.Warning);
        //刷新強化面板資訊
        SettingForgeTarget(currentEqquipment);
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

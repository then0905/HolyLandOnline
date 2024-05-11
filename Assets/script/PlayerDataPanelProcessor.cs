using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//==========================================
//  創建者:家豪
//  創建日期:2023/11/26
//  創建用途:角色介面資料處理
//==========================================
public class PlayerDataPanelProcessor : MonoBehaviour
{
    #region 靜態變數
    private static PlayerDataPanelProcessor instance;
    public static PlayerDataPanelProcessor Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerDataPanelProcessor>();
            return instance;
        }
    }
    #endregion 
    [SerializeField] private TextMeshProUGUI playerID;
    [SerializeField] private TextMeshProUGUI playerDataContent;

    /// <summary>
    /// 初始化角色介面資料
    /// </summary>
    public void Init()
    {
        playerID.text += PlayerDataOverView.Instance.PlayerData_.PlayerName; 
        SetPlayerDataContent();
    }

    /// <summary>
    /// 設定基礎詳細屬性資料呈現
    /// </summary>
    public void SetPlayerDataContent()
    {
        playerDataContent.text =
            "玩家等級:" + PlayerDataOverView.Instance.PlayerData_.Lv + "\n" +
            "種族:" + PlayerDataOverView.Instance.PlayerData_.Race + "\n" +
            "職業:" + PlayerDataOverView.Instance.PlayerData_.Job + "\n" +
            "最大血量:" + PlayerDataOverView.Instance.PlayerData_.MaxHP + "\n" +
            "當前血量:" + PlayerDataOverView.Instance.PlayerData_.HP + "\n" +
            "血量回復:" + PlayerDataOverView.Instance.PlayerData_.HP_Recovery + "\n" +
            "最大魔力:" + PlayerDataOverView.Instance.PlayerData_.MaxMP + "\n" +
            "當前魔力:" + PlayerDataOverView.Instance.PlayerData_.MP + "\n" +
            "魔力回復:" + PlayerDataOverView.Instance.PlayerData_.MP_Recovery + "\n" +
            "經驗量:" + PlayerDataOverView.Instance.PlayerData_.Exp + "\n" +
            "最大經驗值:" + PlayerDataOverView.Instance.PlayerData_.MaxExp + "\n" +
            "近距離攻擊力:" + PlayerDataOverView.Instance.PlayerData_.MeleeATK + "\n" +
            "遠距離攻擊力:" + PlayerDataOverView.Instance.PlayerData_.RemoteATK + "\n" +
            "魔法攻擊力:" + PlayerDataOverView.Instance.PlayerData_.MageATK + "\n" +
            "近距離命中值:" + PlayerDataOverView.Instance.PlayerData_.MeleeHit + "\n" +
            "遠距離命中值:" + PlayerDataOverView.Instance.PlayerData_.RemoteHit + "\n" +
            "魔法命中值:" + PlayerDataOverView.Instance.PlayerData_.MageHit + "\n" +
            "總防禦值:" + PlayerDataOverView.Instance.PlayerData_.DEF + "\n" +
            "總魔法防禦值:" + PlayerDataOverView.Instance.PlayerData_.MDEF + "\n" +
            "傷害減緩:" + PlayerDataOverView.Instance.PlayerData_.DamageReduction + "\n" +
            "總迴避值:" + PlayerDataOverView.Instance.PlayerData_.Avoid + "\n" +
            "暴擊率:" + PlayerDataOverView.Instance.PlayerData_.Crt + "\n" +
            "暴擊抵抗:" + PlayerDataOverView.Instance.PlayerData_.CrtResistance + "\n" +
            "暴擊傷害:" + PlayerDataOverView.Instance.PlayerData_.CrtDamage + "\n" +
            "移動速度:" + PlayerDataOverView.Instance.PlayerData_.Speed + "\n" +
            "力量:" + PlayerDataOverView.Instance.PlayerData_.STR + "\n" +
            "敏捷:" + PlayerDataOverView.Instance.PlayerData_.DEX + "\n" +
            "智慧:" + PlayerDataOverView.Instance.PlayerData_.INT + "\n" +
            "靈巧:" + PlayerDataOverView.Instance.PlayerData_.AGI + "\n" +
            "體力:" + PlayerDataOverView.Instance.PlayerData_.VIT + "\n" +
            "感知:" + PlayerDataOverView.Instance.PlayerData_.WIS + "\n" +
            "攻擊速度:" + PlayerDataOverView.Instance.PlayerData_.AS + "\n" +
            "詠唱速度:" + PlayerDataOverView.Instance.PlayerData_.CS + "\n" +
            "屬性傷害增幅:" + PlayerDataOverView.Instance.PlayerData_.ElementDamageIncrease + "\n" +
            "屬性傷害抗性:" + PlayerDataOverView.Instance.PlayerData_.ElementDamageReduction + "\n" +
            "異常狀態抗性:" + PlayerDataOverView.Instance.PlayerData_.DisorderResistance + "\n" +
            "格檔率:" + PlayerDataOverView.Instance.PlayerData_.BlockRate + "\n" +
            "玩家金幣量:" + PlayerDataOverView.Instance.PlayerData_.Coin + "\n";
    }
}

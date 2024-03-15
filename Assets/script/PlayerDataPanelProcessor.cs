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
        playerID.text += PlayerData.PlayerName; 
        SetPlayerDataContent();
    }

    /// <summary>
    /// 設定基礎詳細屬性資料呈現
    /// </summary>
    public void SetPlayerDataContent()
    {
        playerDataContent.text =
            "玩家等級:" + PlayerData.Lv + "\n" +
            "種族:" + PlayerData.Race + "\n" +
            "職業:" + PlayerData.Job + "\n" +
            "最大血量:" + PlayerData.MaxHP + "\n" +
            "當前血量:" + PlayerData.HP + "\n" +
            "血量回復:" + PlayerData.HP_Recovery + "\n" +
            "最大魔力:" + PlayerData.MaxMP + "\n" +
            "當前魔力:" + PlayerData.MP + "\n" +
            "魔力回復:" + PlayerData.MP_Recovery + "\n" +
            "經驗量:" + PlayerData.Exp + "\n" +
            "最大經驗值:" + PlayerData.MaxExp + "\n" +
            "近距離攻擊力:" + PlayerData.MeleeATK + "\n" +
            "遠距離攻擊力:" + PlayerData.RemoteATK + "\n" +
            "魔法攻擊力:" + PlayerData.MageATK + "\n" +
            "近距離命中值:" + PlayerData.MeleeHit + "\n" +
            "遠距離命中值:" + PlayerData.RemoteHit + "\n" +
            "魔法命中值:" + PlayerData.MageHit + "\n" +
            "總防禦值:" + PlayerData.DEF + "\n" +
            "總魔法防禦值:" + PlayerData.MDEF + "\n" +
            "傷害減緩:" + PlayerData.DamageReduction + "\n" +
            "總迴避值:" + PlayerData.Avoid + "\n" +
            "暴擊率:" + PlayerData.Crt + "\n" +
            "暴擊抵抗:" + PlayerData.CrtResistance + "\n" +
            "暴擊傷害:" + PlayerData.CrtDamage + "\n" +
            "移動速度:" + PlayerData.Speed + "\n" +
            "力量:" + PlayerData.STR + "\n" +
            "敏捷:" + PlayerData.DEX + "\n" +
            "智慧:" + PlayerData.INT + "\n" +
            "靈巧:" + PlayerData.AGI + "\n" +
            "體力:" + PlayerData.VIT + "\n" +
            "感知:" + PlayerData.WIS + "\n" +
            "攻擊速度:" + PlayerData.AS + "\n" +
            "詠唱速度:" + PlayerData.CS + "\n" +
            "屬性傷害增幅:" + PlayerData.ElementDamageIncrease + "\n" +
            "屬性傷害抗性:" + PlayerData.ElementDamageReduction + "\n" +
            "異常狀態抗性:" + PlayerData.DisorderResistance + "\n" +
            "格檔率:" + PlayerData.BlockRate + "\n" +
            "玩家金幣量:" + PlayerData.Coin + "\n";
    }
}

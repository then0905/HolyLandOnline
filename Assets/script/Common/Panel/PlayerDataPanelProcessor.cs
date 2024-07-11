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
        var playerData = PlayerDataOverView.Instance.PlayerData_;
        playerDataContent.text =
            "玩家等級:" + playerData.Lv + "\n" +
            "種族:" + playerData.Race + "\n" +
            "職業:" + playerData.Job + "\n" +
            "最大血量:" + playerData.MaxHP + "\n" +
            "當前血量:" + playerData.HP + "\n" +
            "血量回復:" + playerData.HP_Recovery + "\n" +
            "最大魔力:" + playerData.MaxMP + "\n" +
            "當前魔力:" + playerData.MP + "\n" +
            "魔力回復:" + playerData.MP_Recovery + "\n" +
            "經驗量:" + playerData.Exp + "\n" +
            "最大經驗值:" + playerData.MaxExp + "\n" +
            "近距離攻擊力:" + playerData.MeleeATK + "\n" +
            "遠距離攻擊力:" + playerData.RemoteATK + "\n" +
            "魔法攻擊力:" + playerData.MageATK + "\n" +
            "近距離命中值:" + playerData.MeleeHit + "\n" +
            "遠距離命中值:" + playerData.RemoteHit + "\n" +
            "魔法命中值:" + playerData.MageHit + "\n" +
            "總防禦值:" + playerData.DEF + "\n" +
            "總魔法防禦值:" + playerData.MDEF + "\n" +
            "傷害減緩:" + playerData.DamageReduction + "\n" +
            "總迴避值:" + playerData.Avoid + "\n" +
            "暴擊率:" + playerData.Crt + "\n" +
            "暴擊抵抗:" + playerData.CrtResistance + "\n" +
            "暴擊傷害:" + playerData.CrtDamage + "\n" +
            "移動速度:" + playerData.Speed + "\n" +
            "力量:" + playerData.STR + "\n" +
            "敏捷:" + playerData.DEX + "\n" +
            "智慧:" + playerData.INT + "\n" +
            "靈巧:" + playerData.AGI + "\n" +
            "體力:" + playerData.VIT + "\n" +
            "感知:" + playerData.WIS + "\n" +
            "攻擊速度:" + playerData.AS + "\n" +
            "詠唱速度:" + playerData.CS + "\n" +
            "屬性傷害增幅:" + playerData.ElementDamageIncrease + "\n" +
            "屬性傷害抗性:" + playerData.ElementDamageReduction + "\n" +
            "異常狀態抗性:" + playerData.DisorderResistance + "\n" +
            "格檔率:" + playerData.BlockRate + "\n" +
            "玩家金幣量:" + playerData.Coin + "\n";
    }
}

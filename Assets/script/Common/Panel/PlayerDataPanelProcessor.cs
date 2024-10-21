using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;
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
    [Header("血條"), SerializeField] protected Slider hpSlider;
    [Header("魔力條"), SerializeField] protected Slider mpSlider;
    [Header("經驗條"), SerializeField] protected Slider expSlider;
    [Header("血條文字"), SerializeField] protected TMP_Text hptext;
    [Header("魔力文字"), SerializeField] protected TMP_Text mptext;
    [Header("等級文字"), SerializeField] protected TMP_Text lvtext;
    [Header("玩家名稱文字"), SerializeField] protected TMP_Text nametext;
    /// <summary>
    /// 初始化角色介面資料
    /// </summary>
    public void Init()
    {
        //設定玩家ID
        playerID.text += PlayerDataOverView.Instance.PlayerData_.PlayerName;

        //設定玩家屬性介面
        var playerDataOverView = PlayerDataOverView.Instance;

        playerDataOverView.HpSlider = hpSlider;
        playerDataOverView.MpSlider = mpSlider;
        playerDataOverView.ExpSlider = expSlider;
        playerDataOverView.HpText = hptext;
        playerDataOverView.MpText = mptext;
        playerDataOverView.LvText = lvtext;
        playerDataOverView.NameText = nametext;

        SetPlayerDataContent();
    }

    /// <summary>
    /// 設定基礎詳細屬性資料呈現
    /// </summary>
    public void SetPlayerDataContent()
    {
        var playerData = PlayerDataOverView.Instance.PlayerData_;
        playerDataContent.text =
            "TM_PlayerLv".GetText() + ": " + playerData.Lv + "\n" +
           "TM_Race".GetText() + ": " + ("TM_" + playerData.Race).GetText() + "\n" +
            "TM_Race".GetText() + ": " + ("TM_" + playerData.Job).GetText() + "\n" +
             "TM_MaxHP".GetText() + ": " + playerData.MaxHP + "\n" +
            "TM_HP".GetText() + ": " + playerData.HP + "\n" +
             "TM_HP_Recovery".GetText() + ": " + playerData.HP_Recovery + "\n" +
             "TM_MaxMP".GetText() + ": " + playerData.MaxMP + "\n" +
            "TM_MP".GetText() + ": " + playerData.MP + "\n" +
             "TM_MP_Recovery".GetText() + ": " + playerData.MP_Recovery + "\n" +
            "TM_Exp".GetText() + ": " + playerData.Exp + "\n" +
            "TM_MaxExp".GetText() + ": " + playerData.MaxExp + "\n" +
             "TM_MeleeATK".GetText() + ": " + playerData.MeleeATK + "\n" +
             "TM_RemoteATK".GetText() + ": " + playerData.RemoteATK + "\n" +
             "TM_MageATK".GetText() + ": " + playerData.MageATK + "\n" +
             "TM_MeleeHit".GetText() + ": " + playerData.MeleeHit + "\n" +
             "TM_RemoteHit".GetText() + ": " + playerData.RemoteHit + "\n" +
             "TM_MageHit".GetText() + ": " + playerData.MageHit + "\n" +
             "TM_DEF".GetText() + ": " + playerData.DEF + "\n" +
             "TM_MDEF".GetText() + ": " + playerData.MDEF + "\n" +
            "TM_DamageReduction".GetText() + ": " + playerData.DamageReduction + "\n" +
             "TM_Avoid".GetText() + ": " + playerData.Avoid + "\n" +
            "TM_Crt".GetText() + ": " + playerData.Crt + "\n" +
             "TM_CrtResistance".GetText() + ": " + playerData.CrtResistance + "\n" +
             "TM_CrtDamage".GetText() + ": " + playerData.CrtDamage + "\n" +
             "TM_Speed".GetText() + ": " + playerData.Speed + "\n" +
            "TM_STR".GetText() + ": " + playerData.STR + "\n" +
            "TM_DEX".GetText() + ": " + playerData.DEX + "\n" +
             "TM_INT".GetText() + ": " + playerData.INT + "\n" +
             "TM_AGI".GetText() + ": " + playerData.AGI + "\n" +
             "TM_VIT".GetText() + ": " + playerData.VIT + "\n" +
             "TM_WIS".GetText() + ": " + playerData.WIS + "\n" +
            "TM_AS".GetText() + ": " + playerData.AS + "\n" +
            "TM_CS".GetText() + ": " + playerData.CS + "\n" +
             "TM_ElementDamageIncrease".GetText() + ": " + playerData.ElementDamageIncrease + "\n" +
             "TM_ElementDamageReduction".GetText() + ": " + playerData.ElementDamageReduction + "\n" +
             "TM_DisorderResistance".GetText() + ": " + playerData.DisorderResistance + "\n" +
             "TM_BlockRate".GetText() + ": " + playerData.BlockRate + "\n" +
            "TM_PlayerCoin".GetText() + ": " + playerData.Coin + "\n";
    }
}

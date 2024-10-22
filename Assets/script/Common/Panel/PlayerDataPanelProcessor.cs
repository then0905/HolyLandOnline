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
        playerID.text = "TM_PlayerName".GetText(true) + PlayerDataOverView.Instance.PlayerData_.PlayerName;

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
            "TM_PlayerLv".GetText(true) + playerData.Lv + "\n" +
            "TM_Race".GetText(true) + ("TM_" + playerData.Race).GetText() + "\n" +
            "TM_Race".GetText(true) + ("TM_" + playerData.Job).GetText() + "\n" +
            "TM_MaxHP".GetText(true) + playerData.MaxHP + "\n" +
            "TM_HP".GetText(true) + playerData.HP + "\n" +
            "TM_HP_Recovery".GetText(true) + playerData.HP_Recovery + "\n" +
            "TM_MaxMP".GetText(true) + playerData.MaxMP + "\n" +
            "TM_MP".GetText(true) + playerData.MP + "\n" +
            "TM_MP_Recovery".GetText(true) + playerData.MP_Recovery + "\n" +
            "TM_Exp".GetText(true) + playerData.Exp + "\n" +
            "TM_MaxExp".GetText(true) + playerData.MaxExp + "\n" +
            "TM_MeleeATK".GetText(true) + playerData.MeleeATK + "\n" +
            "TM_RemoteATK".GetText(true) + playerData.RemoteATK + "\n" +
            "TM_MageATK".GetText(true) + playerData.MageATK + "\n" +
            "TM_MeleeHit".GetText(true) + playerData.MeleeHit + "\n" +
            "TM_RemoteHit".GetText(true) + playerData.RemoteHit + "\n" +
            "TM_MageHit".GetText(true) + playerData.MageHit + "\n" +
            "TM_DEF".GetText(true) + playerData.DEF + "\n" +
            "TM_MDEF".GetText(true) + playerData.MDEF + "\n" +
            "TM_DamageReduction".GetText(true) + playerData.DamageReduction + "\n" +
            "TM_Avoid".GetText(true) + playerData.Avoid + "\n" +
            "TM_Crt".GetText(true) + playerData.Crt + "\n" +
            "TM_CrtResistance".GetText(true) + playerData.CrtResistance + "\n" +
            "TM_CrtDamage".GetText(true) + playerData.CrtDamage + "\n" +
            "TM_Speed".GetText(true) + playerData.Speed + "\n" +
            "TM_STR".GetText(true) + playerData.STR + "\n" +
            "TM_DEX".GetText(true) + playerData.DEX + "\n" +
            "TM_INT".GetText(true) + playerData.INT + "\n" +
            "TM_AGI".GetText(true) + playerData.AGI + "\n" +
            "TM_VIT".GetText(true) + playerData.VIT + "\n" +
            "TM_WIS".GetText(true) + playerData.WIS + "\n" +
            "TM_AS".GetText(true) + playerData.AS + "\n" +
            "TM_CS".GetText(true) + playerData.CS + "\n" +
            "TM_ElementDamageIncrease".GetText(true) + playerData.ElementDamageIncrease + "\n" +
            "TM_ElementDamageReduction".GetText(true) + playerData.ElementDamageReduction + "\n" +
            "TM_DisorderResistance".GetText(true) + playerData.DisorderResistance + "\n" +
            "TM_BlockRate".GetText(true) + playerData.BlockRate + "\n" +
            "TM_PlayerCoin".GetText(true) + playerData.Coin + "\n";
    }
}

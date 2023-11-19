using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/18
//  創建用途:  角色UI面板物件計算(血量 魔力 經驗值 等級)
//==========================================
public class PlayerValueManager : MonoBehaviour
{
    #region 全域靜態變數

    public static PlayerValueManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerValueManager>();
            return instance;
        }
    }

    private static PlayerValueManager instance;

    #endregion

    #region 遊戲物件
    public Slider HPSlider;         //血條
    public Slider MPSlider;         //魔力條
    public Slider ExpSlider;        //經驗條
    public TMP_Text HPtext;         //血條文字
    public TMP_Text MPtext;         //魔力文字
    public TMP_Text LVtext;         //等級文字
    #endregion

    public static int FirstInGame = 1;         //第一次登入遊戲

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        RefreshExpAndLv();

        //呼叫自然恢復魔力與血量
        StartCoroutine(RecoveryHpCoroutine());
        StartCoroutine(RecoveryMpCoroutine());
    }

    /// <summary>
    /// 經驗值增加測試
    /// </summary>
    public void exptest()
    {
        PlayerData.Exp += 10;
    }

    /// <summary>
    /// 自然恢復生命計時
    /// </summary>
    /// <returns></returns>
    public IEnumerator RecoveryHpCoroutine()
    {
        float hpRecoverSec = GameData.GameSettingDic["HpRecoverySec"].GameSettingValue;
        while (hpRecoverSec > 0)
        {
            hpRecoverSec -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (PlayerData.HP + PlayerData.HP_Recovery > PlayerData.MaxHP)
            PlayerData.HP = PlayerData.MaxHP;
        else
            PlayerData.HP += PlayerData.HP_Recovery;

        RefreshExpAndLv();

        yield return StartCoroutine(RecoveryHpCoroutine());
    }

    /// <summary>
    /// 自然恢復魔力計時
    /// </summary>
    /// <returns></returns>
    public IEnumerator RecoveryMpCoroutine()
    {
        float hpRecoverSec = GameData.GameSettingDic["MpRecoverySec"].GameSettingValue;
        while (hpRecoverSec > 0)
        {
            hpRecoverSec -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (PlayerData.MP + PlayerData.MP_Recovery > PlayerData.MaxMP)
            PlayerData.MP = PlayerData.MaxMP;
        else
            PlayerData.MP += PlayerData.MP_Recovery;

        RefreshExpAndLv();

        yield return StartCoroutine(RecoveryMpCoroutine());
    }

    /// <summary>
    /// 設定UI資料 血量 魔力 經驗值?
    /// </summary>
    void RefreshExpAndLv()
    {
        //獲取等級
        LVtext.text = PlayerData.Lv.ToString();

        //獲取最大生命值與魔力
        HPSlider.maxValue = PlayerData.MaxHP;
        MPSlider.maxValue = PlayerData.MaxMP;

        //獲取當前生命值與魔力
        HPSlider.value = PlayerData.HP;
        MPSlider.value = PlayerData.MP;

        //帶入UI Slider
        HPtext.text = HPSlider.value + "/" + HPSlider.maxValue;
        MPtext.text = MPSlider.value + "/" + MPSlider.maxValue;

        LoadPlayerData.SaveUserData();
    }
    /// <summary>
    /// 經驗值更動時的處理
    /// </summary>
    public void ExpProcessor()
    {
        //若玩家經驗值>最大經驗值條 為 升級事件
        if (PlayerData.Exp >= ExpSlider.maxValue)
        {
            //呼叫刷新與升等
            LVup();
            //更新經驗值條(扣除當前最大經驗值)
            PlayerData.Exp -= int.Parse(ExpSlider.maxValue.ToString());
            PlayerData.Lv++;
        }

        //設定經驗值調資料
        ExpSlider.value = PlayerData.Exp;
        ExpSlider.maxValue = GameData.ExpAndLvDic.Where(x => x.Key.Contains(PlayerData.Lv.ToString())).Select(x => x.Value).FirstOrDefault().EXP;
    }

    /// <summary>
    /// 升級處理
    /// </summary>
    void LVup()
    {
        //回復玩家生命值與魔力
        PlayerData.HP = PlayerData.MaxHP;
        PlayerData.MP = PlayerData.MaxMP;
    }
}

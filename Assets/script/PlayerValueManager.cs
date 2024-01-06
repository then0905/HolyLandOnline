using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
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
    [Header("血條"), SerializeField] protected Slider hpSlider;
    [Header("魔力條"), SerializeField] protected Slider mpSlider;
    [Header("經驗條"), SerializeField] protected Slider expSlider;
    [Header("血條文字"), SerializeField] protected TMP_Text hptext;
    [Header("魔力文字"), SerializeField] protected TMP_Text mptext;
    [Header("等級文字"), SerializeField] protected TMP_Text lvtext;
    [Header("玩家名稱文字"), SerializeField] protected TMP_Text nametext;
    #endregion

    public static int FirstInGame = 1;         //第一次登入遊戲

    public Action UIrefresh;        //刷新玩家UI的事件
    public Action<int> ChangeHpEvent;        //刷新玩家HP的事件
    public Action<int> ChangeMpEvent;        //刷新玩家MP的事件

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        RefreshExpAndLv();

        //呼叫自然恢復魔力與血量
        StartCoroutine(RecoveryHpCoroutine());
        StartCoroutine(RecoveryMpCoroutine());

        UIrefresh += RefreshExpAndLv;
        UIrefresh += PlayerDataPanelProcessor.Instance.SetPlayerDataContent;
        ChangeHpEvent += ChangePlayerHp;
        ChangeMpEvent += ChangePlayerMp;
    }

    /// <summary>
    /// 經驗值增加測試
    /// </summary>
    public void exptest()
    {
        PlayerData.Exp += 10;
        ExpProcessor();
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

        ChangeHpEvent.Invoke(PlayerData.HP_Recovery);

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

        ChangeMpEvent.Invoke(PlayerData.MP_Recovery);

        yield return StartCoroutine(RecoveryMpCoroutine());
    }

    /// <summary>
    /// 設定UI資料 血量 魔力 經驗值?
    /// </summary>
    void RefreshExpAndLv()
    {
        //獲取等級
        lvtext.text = PlayerData.Lv.ToString();
        //獲取玩家名稱
        nametext.text = PlayerData.PlayerName;

        //獲取最大生命值與魔力
        hpSlider.maxValue = PlayerData.MaxHP;
        mpSlider.maxValue = PlayerData.MaxMP;

        //獲取當前生命值與魔力
        hpSlider.value = PlayerData.HP;
        mpSlider.value = PlayerData.MP;

        //帶入UI Slider
        hptext.text = hpSlider.value + "/" + hpSlider.maxValue;
        mptext.text = mpSlider.value + "/" + mpSlider.maxValue;

        LoadPlayerData.SaveUserData();
    }
    /// <summary>
    /// 經驗值更動時的處理
    /// </summary>
    public void ExpProcessor()
    {
        //若玩家經驗值>最大經驗值條 為 升級事件
        if (PlayerData.Exp >= expSlider.maxValue)
        {
            //更新經驗值條(扣除當前最大經驗值)
            PlayerData.Exp -= int.Parse(expSlider.maxValue.ToString());
            PlayerData.Lv++;
            //呼叫刷新與升等
            LVup();
        }

        //設定經驗值調資料
        expSlider.value = PlayerData.Exp;
        expSlider.maxValue = GameData.ExpAndLvDic.Where(x => x.Key.Contains(PlayerData.Lv.ToString())).Select(x => x.Value).FirstOrDefault().EXP;
    }

    /// <summary>
    /// 升級處理
    /// </summary>
    void LVup()
    {
        StatusOperation.Instance.StatusMethod();//使用者資料刷新
        RefreshExpAndLv();
        //回復玩家生命值與魔力
        PlayerData.HP = PlayerData.MaxHP;
        PlayerData.MP = PlayerData.MaxMP;
        //更新技能 暫時寫在這 以後不確定會不會再學技能這塊增加NPC或什麼道具學習
        ClassAndSkill.Instance.Init();
    }

    /// <summary>
    /// 更動玩家血量
    /// </summary>
    /// <param name="value">更動值</param>
    void ChangePlayerHp(int value)
    {
        //判斷增加的值是否超出血量Range範圍 設定滿血 或 死亡 或更動數值
        if (PlayerData.HP + value > PlayerData.MaxHP)
        {
            //滿血
            PlayerData.HP = PlayerData.MaxHP;
        }
        if (PlayerData.HP + value < 0)
        {
            //死亡 
            PlayerData.HP = 0;
            //呼叫死亡事件
        }
        else
            //正常更動
            PlayerData.HP += value;

        UIrefresh?.Invoke();
    }
    /// <summary>
    /// 更動玩家血量
    /// </summary>
    /// <param name="value">更動值</param>
    void ChangePlayerMp(int value)
    {
        //判斷增加的值是否超出血量Range範圍 設定滿血 或 死亡 或更動數值
        if (PlayerData.MP + value > PlayerData.MaxMP)
        {
            //滿血
            PlayerData.MP = PlayerData.MaxMP;
        }
        else
            //正常更動
            PlayerData.MP += value;

        UIrefresh?.Invoke();
    }
}

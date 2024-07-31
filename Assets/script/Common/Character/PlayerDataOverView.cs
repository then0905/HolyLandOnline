using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/04/21
//  創建用途: 玩家資料總覽
//==========================================
public class PlayerDataOverView : ActivityCharacterBase, ICombatant
{
    #region 全域靜態變數

    private static PlayerDataOverView instance;
    public static PlayerDataOverView Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerDataOverView>(true);
            }
            return instance;
        }
    }

    #endregion

    [Header("遊戲物件")]
    public Character_move CharacterMove;

    //血條
    [HideInInspector] public Slider HpSlider;
    //魔力條
    [HideInInspector] public Slider MpSlider;
    //經驗條
    [HideInInspector] public Slider ExpSlider;
    //血條文字
    [HideInInspector] public TMP_Text HpText;
    //魔力文字
    [HideInInspector] public TMP_Text MpText;
    //等級文字
    [HideInInspector] public TMP_Text LvText;
    //玩家名稱文字
    [HideInInspector] public TMP_Text NameText;

    [Header("遊戲資料")]
    public PlayerData PlayerData_ = new PlayerData();

    public string GetAttackMode { get; set; }
    public int HP
    {
        get => PlayerData_.HP;
        set
        {
            //判斷增加的值是否超出血量Range範圍 設定滿血 或 死亡 或更動數值
            if (value > PlayerData_.MaxHP)
            {
                //滿血
                value = PlayerData_.MaxHP;
            }
            if (value < 0)
            {
                //死亡 
                value = 0;
                //呼叫死亡事件
                IsDead = true;
            }
            PlayerData_.HP = value;
        }
    }
    public int MP
    {
        get => PlayerData_.MP; set
        {
            //判斷增加的值是否超出魔力Range範圍 設定滿魔 或更動數值
            if (value > PlayerData_.MaxMP)
            {
                //滿魔
                value = PlayerData_.MaxMP;
            }
            //正常更動
            PlayerData_.MP = value;
        }
    }
    public int ATK
    {
        get
        {
            return GetAttackMode switch
            {
                "RemoteATK" => PlayerData_.RemoteATK,
                "MageATK" => PlayerData_.MageATK,
                "MeleeATK" => PlayerData_.MeleeATK,
                _ => 0
            };
        }
    }
    public int Hit
    {
        get
        {
            return GetAttackMode switch
            {
                "RemoteATK" => PlayerData_.RemoteHit,
                "MageATK" => PlayerData_.MageHit,
                "MeleeATK" => PlayerData_.MeleeHit,
                _ => 0
            };
        }
    }
    public string Name { get => PlayerData_.PlayerName; }
    public int LV { get => PlayerData_.Lv; }
    public int Avoid { get => PlayerData_.Avoid; }
    public int DEF { get => PlayerData_.Avoid; }
    public int MDEF { get => PlayerData_.MDEF; }
    public int DamageReduction { get => PlayerData_.DamageReduction; }
    public float Crt { get => PlayerData_.Crt; }
    public int CrtDamage { get => PlayerData_.CrtDamage; }
    public float CrtResistance { get => PlayerData_.CrtResistance; }
    public float DisorderResistance { get => PlayerData_.DisorderResistance; }
    public float BlockRate { get => PlayerData_.BlockRate; }
    public float ElementDamageIncrease { get => PlayerData_.ElementDamageIncrease; }
    public float ElementDamageReduction { get => PlayerData_.ElementDamageReduction; }
    public GameObject Obj { get => gameObject; }
    public bool IsDead { get; set; }

    [Header("事件區")]
    public Action UIrefresh;        //刷新玩家UI的事件
    public Action<int> ChangeHpEvent;        //刷新玩家HP的事件
    public Action<int> ChangeMpEvent;        //刷新玩家MP的事件

    private void OnEnable()
    {
        CharacterMove.ControlCharacterEvent += NormalAttackSystem.Instance.StopNormalAttack;
        CharacterMove.ControlCharacterEvent += SkillDisplayAction.Instance.StopSkillChasingTarge;
        SelectTarget.Instance.CharacterCamera = CharacterMove.CharacterCamera;
    }
    private void OnDisable()
    {
        CharacterMove.ControlCharacterEvent -= NormalAttackSystem.Instance.StopNormalAttack;
        CharacterMove.ControlCharacterEvent -= SkillDisplayAction.Instance.StopSkillChasingTarge;
    }

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
        ExpProcessor();
    }

    /// <summary>
    /// 經驗值增加測試
    /// </summary>
    public void exptest()
    {
        PlayerData_.Exp += 10;
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

        ChangeHpEvent.Invoke(PlayerData_.HP_Recovery);

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

        ChangeMpEvent.Invoke(PlayerData_.MP_Recovery);

        yield return StartCoroutine(RecoveryMpCoroutine());
    }

    /// <summary>
    /// 設定UI資料 血量 魔力 經驗值?
    /// </summary>
    private void RefreshExpAndLv()
    {
        //獲取等級
        LvText.text = PlayerData_.Lv.ToString();
        //獲取玩家名稱
        NameText.text = PlayerData_.PlayerName;

        //獲取最大生命值與魔力
        HpSlider.maxValue = PlayerData_.MaxHP;
        MpSlider.maxValue = PlayerData_.MaxMP;

        //獲取當前生命值與魔力
        HpSlider.value = HP;
        MpSlider.value = MP;

        //帶入UI Slider
        HpText.text = HpSlider.value + "/" + HpSlider.maxValue;
        MpText.text = MpSlider.value + "/" + MpSlider.maxValue;

        LoadPlayerData.SaveUserData();
    }
    /// <summary>
    /// 經驗值更動時的處理
    /// </summary>
    public void ExpProcessor()
    {
        //設定經驗值條資料
        ExpSlider.value = PlayerData_.Exp;
        ExpSlider.maxValue = GameData.ExpAndLvDic.Where(x => x.Key.Contains(PlayerData_.Lv.ToString())).Select(x => x.Value).FirstOrDefault().EXP;
        //若玩家經驗值>最大經驗值條 為 升級事件
        if (PlayerData_.Exp >= ExpSlider.maxValue)
        {
            //更新經驗值條(扣除當前最大經驗值)
            PlayerData_.Exp -= int.Parse(ExpSlider.maxValue.ToString());
            PlayerData_.Lv++;
            //呼叫刷新與升等
            LVup();
        }
    }

    /// <summary>
    /// 升級處理
    /// </summary>
    private void LVup()
    {
        StatusOperation.Instance.StatusMethod();//使用者資料刷新
        RefreshExpAndLv();
        //回復玩家生命值與魔力
        PlayerData_.HP = PlayerData_.MaxHP;
        PlayerData_.MP = PlayerData_.MaxMP;
        //更新技能 暫時寫在這 以後不確定會不會再學技能這塊增加NPC或什麼道具學習
        Init();
    }

    /// <summary>
    /// 更動玩家血量
    /// </summary>
    /// <param name="value">更動值</param>
    private void ChangePlayerHp(int value)
    {
        HP = value + HP;
        UIrefresh?.Invoke();
    }
    /// <summary>
    /// 更動玩家魔力
    /// </summary>
    /// <param name="value">更動值</param>
    private void ChangePlayerMp(int value)
    {
        MP = value + MP;
        UIrefresh?.Invoke();
    }

    public void DealingWithInjuriesMethod(ICombatant attackerData, int damage)
    {
        ChangeHpEvent.Invoke(damage);
    }
}

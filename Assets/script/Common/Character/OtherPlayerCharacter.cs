using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:    家豪
//  創建日期:  2024/10/22
//  創建用途:  其他玩家的行為呈現設定
//==========================================
public class OtherPlayerCharacter : ActivityCharacterBase
{
    [Header("遊戲資料")]
    public PlayerData PlayerData_ = new PlayerData();

    public override string GetAttackMode { get; set; }
    public override int HP
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
    public override int MP
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
    public override int ATK
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
    public override int Hit
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
    public override string Name { get => PlayerData_.PlayerName; }
    public override int LV { get => PlayerData_.Lv; }
    public override int Avoid { get => PlayerData_.Avoid; }
    public override int DEF { get => PlayerData_.Avoid; }
    public override int MDEF { get => PlayerData_.MDEF; }
    public override int DamageReduction { get => PlayerData_.DamageReduction; }
    public override float Crt { get => PlayerData_.Crt; }
    public override int CrtDamage { get => PlayerData_.CrtDamage; }
    public override float CrtResistance { get => PlayerData_.CrtResistance; }
    public override float DisorderResistance { get => PlayerData_.DisorderResistance; }
    public override float BlockRate { get => PlayerData_.BlockRate; }
    public override float ElementDamageIncrease { get => PlayerData_.ElementDamageIncrease; }
    public override float ElementDamageReduction { get => PlayerData_.ElementDamageReduction; }
    public override GameObject Obj { get => gameObject; }

    /// <summary>
    /// 初始化設定
    /// </summary>
    public void Init()
    {

    }

    /// <summary>
    /// 設定服裝
    /// </summary>
    public void SetEquip()
    {

    }

    /// <summary>
    /// 角色移動及面想
    /// </summary>
    public void CharacterMoveAndFace()
    {

    }

    /// <summary>
    /// 角色獲得Buff
    /// </summary>
    /// <param name="skillData"></param>
    public void GetBuff(Skill_Base_Buff skillData)
    {

    }

    public override void DealingWithInjuriesMethod(ICombatant attackerData, int damage, bool animTrigger = true)
    {
        
    }

    protected override void CharacterIsDead()
    {

    }
}

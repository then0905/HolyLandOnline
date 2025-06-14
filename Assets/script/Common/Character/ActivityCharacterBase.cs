using System;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/03/14
//  創建用途: 活體人物基底
//==========================================
public abstract class ActivityCharacterBase : MonoBehaviour, ICombatant
{
    [Header("角色中心點"), SerializeField] protected Transform povit;
    public Transform Povit
    {
        get { return povit; }
    }
    [Header("角色頭部 傷害數字參考"), SerializeField] protected Transform headTrans;
    public Transform HeadTrans
    {
        get { return headTrans; }
    }
    [Header("可否被選取"), SerializeField]
    protected bool canBeChoose;
    public bool CanBeChoose => canBeChoose;

    public virtual string Name { get; }

    public virtual int HP { get; set; }
    public virtual int MP { get; set; }

    public virtual int LV { get; }

    public virtual int ATK { get; }

    private string attackMode;
    public virtual string GetAttackMode
    {
        get { return attackMode; }
        set { attackMode = value; }
    }

    public virtual int Hit { get; }

    public virtual int Avoid { get; }

    public virtual int DEF { get; }

    public virtual int MDEF { get; }

    public virtual int DamageReduction { get; }

    public virtual float Crt { get; }

    public virtual int CrtDamage { get; }

    public virtual float CrtResistance { get; }

    public virtual float DisorderResistance { get; }

    public virtual float BlockRate { get; }

    public virtual float ElementDamageIncrease { get; }

    public virtual float ElementDamageReduction { get; }

    public virtual GameObject Obj { get; }

    protected bool isDead;
    public bool IsDead
    {
        get
        {
            return isDead;
        }
        set
        {
            isDead = value;
            if (isDead)
                CharacterIsDead();
        }
    }
    public int MoveIsEnable { get; set; } = 0;

    public virtual void MoveEnable(bool enable)
    {
        //設計的想法是 =0才可以移動 帶入false會+1 不能動 等到狀態解除會-1 可能會被多個控制狀態堆疊所以用此設計計算
        int tempValue = (enable ? -1 : 1);
        MoveIsEnable = (MoveIsEnable + tempValue) < 0 ? MoveIsEnable : MoveIsEnable + tempValue;
    }
    public int SkillIsEnable { get; set; } = 0;
    public virtual void SkillEnable(bool enable)
    {
        //設計的想法是 =0才可以移動 帶入false會+1 不能動 等到狀態解除會-1 可能會被多個控制狀態堆疊所以用此設計計算
        int tempValue = (enable ? -1 : 1);
        SkillIsEnable = (SkillIsEnable + tempValue) < 0 ? SkillIsEnable : SkillIsEnable + tempValue;
    }
    public int AttackIsEnable { get; set; } = 0;
    public virtual void AttackEnable(bool enable)
    {
        //設計的想法是 =0才可以移動 帶入false會+1 不能動 等到狀態解除會-1 可能會被多個控制狀態堆疊所以用此設計計算
        int tempValue = (enable ? -1 : 1);
        AttackIsEnable = (AttackIsEnable + tempValue) < 0 ? AttackIsEnable : AttackIsEnable + tempValue;
    }

    /// <summary>
    /// 物件被選取處理
    /// </summary>
    public void BeenSelected()
    {
        if (!canBeChoose) return;
        MonsterBehaviour monsterBehaviour = this as MonsterBehaviour;
        NpcBehavior npcBehavior = this as NpcBehavior;
        if (monsterBehaviour != null)
        {
            SetTargetInformation(monsterBehaviour);
        }
        else if (npcBehavior != null)
        {
            SetTargetInformation(npcBehavior);
        }
    }

    /// <summary>
    /// 設定目標信息(血量 等級..)
    /// </summary>
    protected void SetTargetInformation(NpcBehavior npcBehavior)
    {
        if (this is MonoBehaviour)
        {
            SelectTarget.Instance.TargetInformation.SetActive(true);
            SelectTarget.Instance.TargetHP.maxValue = npcBehavior.HP;
            SelectTarget.Instance.TargetHP.value = npcBehavior.HP;
            SelectTarget.Instance.TargetName.text = npcBehavior.Name;
            SelectTarget.Instance.TargetLV.text = 1.ToString();
            SelectTarget.Instance.TargetClass.text = "NPC";
            SelectTarget.Instance.TargetHP_MinAndMax.text = SelectTarget.Instance.TargetHP.value.ToString() + "/" + SelectTarget.Instance.TargetHP.maxValue.ToString();
        }
    }

    /// <summary>
    /// 設定目標信息(血量 等級..)
    /// </summary>
    protected void SetTargetInformation(MonsterBehaviour monster)
    {
        if (this is MonoBehaviour)
        {
            SelectTarget.Instance.TargetInformation.SetActive(true);
            SelectTarget.Instance.TargetHP.maxValue = monster.MonsterValue.HP;
            SelectTarget.Instance.TargetHP.value = monster.HP;
            SelectTarget.Instance.TargetName.text = monster.MonsterValue.Name.GetText();
            SelectTarget.Instance.TargetLV.text = monster.MonsterValue.Lv.ToString();
            SelectTarget.Instance.TargetClass.text = monster.MonsterValue.Class;
            SelectTarget.Instance.TargetHP_MinAndMax.text = SelectTarget.Instance.TargetHP.value.ToString() + "/" + SelectTarget.Instance.TargetHP.maxValue.ToString();
            SelectTarget.Instance.DebuffIconSetting(monster.DebuffEffectBases);
        }
    }

    private void OnMouseDown()
    {
        if (SelectTarget.Instance.Targetgameobject == this)
        {
            if (this is NpcBehavior)
            {
                NpcBehavior behavior = (NpcBehavior)this;
                StartCoroutine(
                CommonFunction.DetectionRangeMethod(PlayerDataOverView.Instance.Povit.gameObject,
                   behavior.Povit.gameObject, 7.5f,
                    () =>
                    {
                        //開啟自動尋路
                        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = true;
                        float pursueDistance = Vector3.Distance(PlayerDataOverView.Instance.Povit.gameObject.transform.position, behavior.Povit.gameObject.transform.position);

                        if (pursueDistance > 7.5f)
                        {
                            // 取得角色面相目標的方向
                            Vector3 direction = SelectTarget.Instance.Targetgameobject.transform.position - PlayerDataOverView.Instance.CharacterMove.Character.transform.position;
                            // 鎖定y軸的旋轉 避免角色在x軸和z軸上傾斜
                            direction.y = 0;
                            // 如果 direction 的長度不為零，設定角色的朝向
                            if (direction != Vector3.zero)
                                PlayerDataOverView.Instance.CharacterMove.Character.transform.rotation = Quaternion.LookRotation(direction);

                            PlayerDataOverView.Instance.CharacterMove.RunAnimation(true);
                            PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position =
                                Vector3.MoveTowards(PlayerDataOverView.Instance.Povit.transform.position,
                                SelectTarget.Instance.Targetgameobject.Povit.position,
                                PlayerDataOverView.Instance.CharacterMove.MoveSpeed);
                        }
                    }
                    , () =>
                    {
                        PlayerDataOverView.Instance.CharacterMove.RunAnimation(false);
                        behavior.NpcInit();
                    }));
            }
            else if (this is MonsterBehaviour)
            {
                MonsterBehaviour monsterBehaviour = (MonsterBehaviour)this;

                NormalAttackSystem.Instance.StartNormalAttack(
                   PlayerDataOverView.Instance,
                   monsterBehaviour);
            }
        }
    }

    public virtual void DealingWithInjuriesMethod(ICombatant attackerData, int damage, bool animTrigger = true)
    {

    }

    public virtual void DealingWithHealMethod(ICombatant attackerData, int value, bool animTrigger = true)
    {

    }

    public virtual void DealingWithMageMethod(ICombatant attackerData, int value, bool animTrigger = true)
    {

    }

    public virtual void GetBuffEffect(ICombatant target, OperationData operationData)
    {

    }

    public virtual void RemoveBuffEffect(ICombatant target, OperationData operationData)
    {

    }

    public virtual void GetDebuff(DebuffEffectBase debuffEffectBase)
    {
        switch (debuffEffectBase.InfluenceStatus)
        {
            case "Stun":
                //禁止移動 禁止施放技能 禁止攻擊
                MoveEnable(false);
                SkillEnable(false);
                AttackEnable(false);
                break;
            default:
                break;
        }

    }

    public virtual void RemoveDebuff(DebuffEffectBase debuffEffectBase)
    {
        switch (debuffEffectBase.InfluenceStatus)
        {
            case "Stun":
                //恢復移動 恢復施放技能 恢復攻擊
                MoveEnable(true);
                SkillEnable(true);
                AttackEnable(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 人物死亡執行內容
    /// </summary>
    protected abstract void CharacterIsDead();
}
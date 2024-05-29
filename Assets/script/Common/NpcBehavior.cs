using UnityEngine;
using TMPro;

//==========================================
//  創建者:    家豪
//  翻修日期:  2024/05/13
//  創建用途:  NPC行為設定呈現
//==========================================

public class NpcBehavior : ActivityCharacterBase, ICombatant
{
    [Header("遊戲物件")]
    //Npc UI
    public Canvas NameCanvas;
    public GameObject NameText;

    [Header("遊戲資料")]
    private NpcSystem npcSystem;
    //Npc ID
    public string NpcID;
    //Npc資料
    private NpcDataModel npcDataModel;
    public NpcDataModel NpcDataModel
    {
        get
        {
            return npcDataModel;
        }
    }

    public string Name => npcDataModel.NpcName;

    public int HP { get { return npcDataModel.HP; } set { HP = value; } }
    public int MP { get { return npcDataModel.MP; } set { MP = value; } }

    public int LV => 1;

    public int ATK => npcDataModel.ATK;

    public int Hit => npcDataModel.Hit;

    public int Avoid => npcDataModel.Avoid;

    public int DEF => npcDataModel.DEF;

    public int MDEF => npcDataModel.MDEF;

    public int DamageReduction => npcDataModel.DamageReduction;

    public float Crt => npcDataModel.Crt;

    public int CrtDamage => npcDataModel.CrtDamage;

    public float CrtResistance => npcDataModel.CrtResistance;

    public float DisorderResistance => npcDataModel.DisorderResistance;

    public float BlockRate => npcDataModel.BlockRate;

    public float ElementDamageIncrease => npcDataModel.ElementDamageIncrease;

    public float ElementDamageReduction => npcDataModel.ElementDamageReduction;

    public GameObject Obj => gameObject;

    public bool IsDead { get; set; }
    private void Start()
    {
        npcSystem = NpcSystem.Instance;
        npcDataModel = GameData.NpcDataDic?[NpcID];
        NameText.GetComponent<TextMeshProUGUI>().text = npcDataModel.NpcName;
    }


    private void LateUpdate()
    {
        //每幀刷新 讓怪物身上的UI面對玩家
        NameText.transform.LookAt(NameText.transform.position + Camera.main.transform.rotation * Vector3.forward,
Camera.main.transform.rotation * Vector3.up);//讓傷害數字面對玩家
    }

    public void NpcInit()
    {
        npcSystem.InitNpcSystem(npcDataModel);
    }

    public void NpcWindowInit()
    {

    }

    public void DealingWithInjuriesMethod(ICombatant attackerData, int damage)
    {
    }
}

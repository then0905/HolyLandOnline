using UnityEngine;
using TMPro;

//==========================================
//  創建者:    家豪
//  翻修日期:  2024/05/13
//  創建用途:  NPC行為設定呈現
//==========================================

public class NpcBehavior : ActivityCharacterBase
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

    public override string Name => npcDataModel.NpcName.GetText();

    public override int HP { get { return npcDataModel.HP; } set { HP = value; } }
    public override int MP { get { return npcDataModel.MP; } set { MP = value; } }

    public override int LV => 1;

    public override int ATK => npcDataModel.ATK;

    public override string GetAttackMode { get; set; }

    public override int Hit => npcDataModel.Hit;

    public override int Avoid => npcDataModel.Avoid;

    public override int DEF => npcDataModel.DEF;

    public override int MDEF => npcDataModel.MDEF;

    public override int DamageReduction => npcDataModel.DamageReduction;

    public override float Crt => npcDataModel.Crt;

    public override int CrtDamage => npcDataModel.CrtDamage;

    public override float CrtResistance => npcDataModel.CrtResistance;

    public override float DisorderResistance => npcDataModel.DisorderResistance;

    public override float BlockRate => npcDataModel.BlockRate;

    public override float ElementDamageIncrease => npcDataModel.ElementDamageIncrease;

    public override float ElementDamageReduction => npcDataModel.ElementDamageReduction;

    public override GameObject Obj => gameObject;

    private void Start()
    {
        //獲得NPC系統實例
        npcSystem = NpcSystem.Instance;
        //獲得此NPC資料
        npcDataModel = GameData.NpcDataDic?[NpcID];
        //設定此NPC名稱
        NameText.GetComponent<TextMeshProUGUI>().text = npcDataModel.NpcName.GetText();
        //將名稱文字的父級 轉移到 文字畫布
        NameText.transform.SetParent(MapManager.Instance.CanvasMapText.transform);
    }


    private void Update()
    {
        //獲取世界轉換成螢幕的座標
        Vector3 screenPosition = PlayerDataOverView.Instance.CharacterMove.CharacterCamera.WorldToScreenPoint(HeadTrans.position);
        //計算怪物物件與攝影機的距離
        float distance = Vector3.Distance(HeadTrans.position, PlayerDataOverView.Instance.CharacterMove.CharacterCamera.transform.position);

        // 將屏幕坐標轉換為Canvas中的本地坐標
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            MapManager.Instance.CanvasMapText.GetComponent<RectTransform>(),
            screenPosition,
            MapManager.Instance.CanvasMapText.worldCamera,
            out Vector2 localPoint);

        //計算縮放距離
        float scale = Mathf.Clamp(1.0f - (distance * 0.02f), 0.5f, 2.0f);
        //若在玩家身後則不顯示
        NameText.gameObject.SetActive(screenPosition.z > 0);
        //設定文字座標
        NameText.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(localPoint.x, localPoint.y, 0);
        // 設定文字大小
        NameText.transform.localScale = new Vector3(scale, scale, scale);
    }


    public void NpcInit()
    {
        npcSystem.InitNpcSystem(npcDataModel);
    }

    public void NpcWindowInit()
    {

    }

    public override void DealingWithInjuriesMethod(ICombatant attackerData, int damage)
    {
    }
}

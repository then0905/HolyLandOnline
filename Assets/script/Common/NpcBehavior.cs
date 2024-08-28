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
    //頭頂位置參考
    public Transform HeadTrans;
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
        //獲得NPC系統實例
        npcSystem = NpcSystem.Instance;
        //獲得此NPC資料
        npcDataModel = GameData.NpcDataDic?[NpcID];
        //設定此NPC名稱
        NameText.GetComponent<TextMeshProUGUI>().text = npcDataModel.NpcName;
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

    public void DealingWithInjuriesMethod(ICombatant attackerData, int damage)
    {
    }
}

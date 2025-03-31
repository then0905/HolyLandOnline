using HLO_Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/08/01
//  創建用途: 角色創建、選擇 場景
//==========================================
public class CharacterScene : MonoBehaviour
{
    [Header("創造角色")]
    [SerializeField] private GameObject createCharacterWindow;
    [SerializeField] private TMP_InputField createCharacterName;
    [SerializeField] private Transform characterTransOffset;        //生成角色物件參考

    [Header("選擇角色")]
    [SerializeField] private GameObject chooseCharacterWindow;
    [SerializeField] private CharacterChooseItem characterChooseItem;
    [SerializeField] private Transform characterChooseSpawnTrans;
    private List<CharacterChooseItem> characterChooseItemList = new List<CharacterChooseItem>();

    [SerializeField] private TextMeshProUGUI characterName;   //角色名稱
    [SerializeField] private Slider characterHpSlider;       //角色血量bar
    [SerializeField] private TextMeshProUGUI characterHpText;       //角色血量文字
    [SerializeField] private Slider characterMpTextSlider;       //角色魔力Bar
    [SerializeField] private TextMeshProUGUI characterMpText;       //角色魔力文字
    [SerializeField] private TextMeshProUGUI characterRace;       //角色種族
    [SerializeField] private TextMeshProUGUI characterJob;        //角色職業
    [SerializeField] private TextMeshProUGUI characterCreateTime;     //角色創建時間
    [SerializeField] private TextMeshProUGUI characterLoginTime;      //角色最後登入時間

    [Header("共用確認視窗")]
    [SerializeField] private GameObject commonCheckPanelObj;
    [SerializeField] private TextMeshProUGUI commonCheckPanelContent;

    //帳號與角色資料
    public static LoginAccountResponse UserAccountResponseData = null;
    //暫存 選擇角色的資料
    private CharacterDataDTO tempChooseCharacterData = null;

    public string RaceData { get; set; }        //種族
    public string JobData { get; set; }       //職業
    public string CharacterName { get; set; }         //角色名稱

    private void Start()
    {
        //InitSequenceManager.Instance.Init();
        //StartCoroutine(InitSequenceManager.Instance.Init());
        //StartCoroutine(Init());

        GameData.Init();            //GameData初始化 因為創角需要計算HP MP資料

        if (UserAccountResponseData.CharacterList.CheckAnyData())
            SetChooseCharacterWindow();
        else
            SetCreateCharacterWindow();
    }

    /// <summary>
    /// 呼叫初始化
    /// </summary>
    /// <returns></returns>
    public IEnumerator Init()
    {
        yield return new WaitForSeconds(2);
        MapManager.Instance.NextMap("DevelopmentTest");
    }

    /// <summary>
    /// 設定創造角色畫面
    /// </summary>
    private void SetCreateCharacterWindow()
    {
        print("呼叫創角");
        createCharacterWindow.SetActive(true);
        chooseCharacterWindow.SetActive(false);
        //生成角色物件
        GameObject character = CommonFunction.LoadObject<GameObject>(GameConfig.ChooseSceneCharacterPrefab, "Character");
        Instantiate(character, characterTransOffset);
    }

    /// <summary>
    /// 設定選擇角色畫面
    /// </summary>
    private void SetChooseCharacterWindow()
    {
        print("呼叫選角");
        createCharacterWindow.SetActive(false);
        chooseCharacterWindow.SetActive(true);

        foreach (var characterData in UserAccountResponseData.CharacterList)
        {
            var temp = Instantiate(characterChooseItem, characterChooseSpawnTrans);
            temp.gameObject.SetActive(true);
            temp.Init(characterData, CharacterBeChoosed);
            characterChooseItemList.Add(temp);
        }
    }

    /// <summary>
    /// 角色被選擇事件
    /// </summary>
    /// <param name="data"></param>
    private void CharacterBeChoosed(CharacterDataDTO data)
    {
        tempChooseCharacterData = data;
        characterName.text = data.CharacterName;
        characterRace.text = data.Race;
        characterJob.text = data.Job;
        characterHpSlider.maxValue = data.MaxHP;
        characterHpSlider.value = data.HP;
        characterHpText.text = $"{data.HP}/{data.MaxHP}";
        characterMpTextSlider.maxValue = data.MaxMP;
        characterMpTextSlider.value = data.MP;
        characterMpText.text = $"{data.MP}/{data.MaxMP}";
        characterCreateTime.text = $"創建日期：{data.CharacterCreateTime.ToString("yyyy/MM/dd HH:mm:ss")}";
        characterLoginTime.text = $"最後登入日期：{(data.LastLogintTime == null ? "--" : data.LastLogintTime.Value.ToString("yyyy / MM / dd HH: mm: ss"))}";
    }

    #region 按鈕方法

    /// <summary>
    /// 確認創建角色視窗設定 (掛在創建角色的第一個確認按鈕)
    /// </summary>
    public void CreateCharacterCheck()
    {
        commonCheckPanelObj.SetActive(true);
        commonCheckPanelContent.text = $"名稱:{CharacterName + "\n"}種族：{RaceData + "\n"}職業：{JobData + "\n"} 是否確定創建此角色?";
    }

    /// <summary>
    /// 創造角色API (掛在創建角色確認視窗的按鈕)
    /// </summary>
    public async void CreateCharacterAPI()
    {
        //呼叫 創造角色API
        await ApiManager.ApiPostFunc<CreateCharacterRequest, CreateCharacterResponse>(
           new CreateCharacterRequest()
           {
               Account = UserAccountResponseData.Account,
               CharacterData = new CharacterDataDTO()
               {
                   CharacterName = CharacterName,
                   Job = JobData,
                   Race = RaceData,
                   MaxHP = StatusOperation.CreateCharacterOperation("HP", JobData, RaceData),
                   MaxMP = StatusOperation.CreateCharacterOperation("MP", JobData, RaceData)
               }
           },
            (response) =>
            {
                //刷新帳戶的角色資料
                UserAccountResponseData = new LoginAccountResponse()
                {
                    Account = response.Account,
                    CharacterList = response.CharacterList
                };
                commonCheckPanelObj.SetActive(false);
                SetChooseCharacterWindow();
            });
    }

    /// <summary>
    /// 登入角色API
    /// </summary>
    public async void LoginCharacterAPI()
    {
        //呼叫 登入角色API
        await ApiManager.ApiGetFunc<LoginCharacterRequest, LoginCharacterResponse>(
           new LoginCharacterRequest()
           {
               Account = UserAccountResponseData.Account,
               CharacterData = tempChooseCharacterData
           },
            (response) =>
            {
                //tempChooseCharacterData = response.CharacterData;
            });
    }

    #endregion
}


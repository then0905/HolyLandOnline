using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2025/03/16
//  創建用途: 選擇角色視窗 角色清單物件
//==========================================
public class CharacterChooseItem : MonoBehaviour
{
    [SerializeField] private Button characterButton;   //按鈕事件
    [SerializeField] private TextMeshProUGUI characterName;   //角色名稱
    [SerializeField] private TextMeshProUGUI characterRace;       //角色種族
    [SerializeField] private TextMeshProUGUI characterJob;        //角色職業
    [SerializeField] private TextMeshProUGUI characterCreateTime;     //角色創建時間
    [SerializeField] private TextMeshProUGUI characterLoginTime;      //角色最後登入時間

    //暫存資料
    public CharacterDataModel characterData { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(CharacterDataModel data, Action<CharacterDataModel> beChooseEvent)
    {
        characterData = data;
        characterName.text = $"角色名稱：{data.CharacterName}";
        characterRace.text = $"所屬種族：{data.Race}";
        characterJob.text = $"職業：{data.Job}";
        characterCreateTime.text = $"創建日期：{data.CharacterCreateTime.ToString("yyyy/MM/dd HH:mm:ss")}";
        characterLoginTime.text = $"最後登入日期：{data.LastLogintTime.ToString("yyyy/MM/dd HH:mm:ss")}";
        characterButton.onClick.AddListener(() => { beChooseEvent?.Invoke(characterData); });
    }
}

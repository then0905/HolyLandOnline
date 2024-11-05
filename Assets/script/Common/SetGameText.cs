using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/11/05
//  創建用途: 掛上腳本 預設就從GameText設定好文字內容
//  掛上此腳本的物件名稱需用GameText的ID
//==========================================
public class SetGameText : MonoBehaviour
{
    void Start()
    {
        TextMeshProUGUI text =  GetComponent<TextMeshProUGUI>();
        text.text = gameObject.name.GetText();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2025/03/16
//  創建用途: 創建角色的需求結構
//==========================================
public class CreateCharacterRequest : HLO_ClientRequest
{
    public CreateCharacterRequest()
    {
        Path = "/Character/CreateCharacter";
    }

    /// <summary>
    /// 帳號
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 此次創建的角色資料
    /// </summary>
    public CharacterDataModel CharacterData { get; set; }
}
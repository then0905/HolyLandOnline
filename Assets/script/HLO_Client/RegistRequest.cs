using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2025/03/16
//  創建用途: 註冊帳號的需求結構
//==========================================
public class RegistRequest : HLO_ClientRequest
{
    public RegistRequest()
    {
        Path = "/User/Regist";
    }

    /// <summary>
    /// 帳號
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; }
}

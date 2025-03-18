//==========================================
//  創建者:家豪
//  創建日期:2025/03/14
//  創建用途: 輸入帳號後的需求結構
//==========================================
public class LoginAccountRequest : HLO_ClientRequest
{
    public LoginAccountRequest()
    {
        Path = "/User/Login";
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

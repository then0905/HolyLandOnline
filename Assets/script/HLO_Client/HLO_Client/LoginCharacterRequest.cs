//==========================================
//  創建者:家豪
//  創建日期:2025/03/14
//  創建用途: 登入角色後的需求結構
//==========================================
namespace HLO_Client
{
    public class LoginCharacterRequest : HLO_ClientRequest
    {
        public LoginCharacterRequest()
        {
            Path = "/Character/LoginCharacter";
        }

        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 角色資料
        /// </summary>
        public CharacterDataDTO CharacterData { get; set; }
    }
}

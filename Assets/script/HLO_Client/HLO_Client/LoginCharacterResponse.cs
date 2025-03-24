//==========================================
//  創建者:家豪
//  創建日期:2025/03/14
//  創建用途: 登入角色的回應結構
//==========================================
namespace HLO_Client
{
    public class LoginCharacterResponse : HLO_ClientResponse
    {
        /// <summary>
        /// 角色資料
        /// </summary>
        public CharacterDataDTO CharacterData { get; set; }
    }
}

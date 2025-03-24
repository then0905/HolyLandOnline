//==========================================
//  創建者:家豪
//  創建日期:2025/03/16
//  創建用途: 創建角色的回應結構
//==========================================
using System.Collections.Generic;

namespace HLO_Client
{
    public class CreateCharacterResponse : HLO_ClientResponse
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 角色資料
        /// </summary>
        public List<CharacterDataDTO>? CharacterList { get; set; }
    }
}
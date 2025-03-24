//==========================================
//  創建者:家豪
//  創建日期:2025/03/16
//  創建用途: 創建角色的回應結構
//==========================================
using System.Collections.Generic;

namespace HLO_Client {
    public class UpdateCharacterResponse : HLO_ClientResponse
    {
        /// <summary>
        /// 此次傳送的角色資料
        /// </summary>
        public CharacterDataDTO CharacterData { get; set; }
    }
}
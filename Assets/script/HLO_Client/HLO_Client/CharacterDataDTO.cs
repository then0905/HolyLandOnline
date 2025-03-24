//==========================================
//  創建者:家豪
//  創建日期:2025/03/16
//  創建用途: 角色的資料結構(傳輸層用)
//==========================================

using System;
using System.Collections.Generic;

namespace HLO_Client
{
    public class CharacterDataDTO
    {
        /// <summary>
        /// 角色名稱
        /// </summary>
        public string CharacterName { get; set; }

        /// <summary>
        /// 等級
        /// </summary>
        public int Lv { get; set; }

        /// <summary>
        /// 最大血量
        /// </summary>
        public int MaxHP { get; set; }

        /// <summary>
        /// 當前血量
        /// </summary>
        public int HP { get; set; }

        /// <summary>
        /// 最大魔力
        /// </summary>
        public int MaxMP { get; set; }

        /// <summary>
        /// 當前魔力
        /// </summary>
        public int MP { get; set; }

        /// <summary>
        /// 當前經驗值
        /// </summary>
        public int EXP { get; set; }

        /// <summary>
        /// 職業
        /// </summary>
        public string Job { get; set; }

        /// <summary>
        /// 所屬種族
        /// </summary>
        public string Race { get; set; }

        /// <summary>
        /// 擁有的金錢
        /// </summary>
        public int Coin { get; set; }

        /// <summary>
        /// 角色創立日期
        /// </summary>
        public DateTime CharacterCreateTime { get; set; }

        /// <summary>
        /// 最後登入日期
        /// </summary>
        public DateTime? LastLogintTime { get; set; }

        /// <summary>
        /// 最後遊玩的地圖
        /// </summary>
        public string? LastMapID { get; set; }

        /// <summary>最後存在的座標 X</summary>
        public float? LastPosX { get; set; }
        /// <summary>最後存在的座標 Y</summary>
        public float? LastPosY { get; set; }
        /// <summary>最後存在的座標 Z</summary>
        public float? LastPosZ { get; set; }

        /// <summary>
        /// 背包格數
        /// </summary>
        public int BagCount { get; set; }

        /// <summary>
        /// 背包資料(含裝備欄資料)
        /// </summary>
        public List<InventoryDataDTO>? BagDataList { get; set; } = new List<InventoryDataDTO>();

        /// <summary>
        /// 角色是否正在線上
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// 樂觀鎖
        /// </summary>
        public DateTime RowVersion { get; set; }
    }
}

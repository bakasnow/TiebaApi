using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiebaApi.TiebaJieGou
{
    /// <summary>
    /// 贴吧用户搜索结构
    /// </summary>
    public class TiebaYongHuSouSuoJieGou
    {
        /// <summary>
        /// 获取成功
        /// </summary>
        public bool HuoQuChengGong;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg;

        /// <summary>
        /// Uid
        /// </summary>
        public long Uid;

        /// <summary>
        /// 贴吧数字ID
        /// </summary>
        public long TiebaUid;

        /// <summary>
        /// 用户名
        /// </summary>
        public string YongHuMing;

        /// <summary>
        /// 昵称
        /// </summary>
        public string NiCheng;

        /// <summary>
        /// 覆盖名
        /// </summary>
        public string FuGaiMing;

        /// <summary>
        /// 个人简介
        /// </summary>
        public string JianJie;

        /// <summary>
        /// 头像
        /// </summary>
        public string TouXiangID;

        /// <summary>
        /// 粉丝数
        /// </summary>
        public int FenSiShu;
    }
}

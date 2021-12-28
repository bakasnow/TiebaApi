using System;
using System.Collections.Generic;

namespace TiebaApi.TiebaJieGou
{
    /// <summary>
    /// 贴吧吧务后台结构
    /// </summary>
    public static class TiebaBaWuHouTaiJieGou
    {
        /// <summary>
        /// 帖子管理日志结构
        /// </summary>
        public class TieZiGuanLiRiZhiJieGou
        {
            public long Tid;
            public long Pid;

            public string BiaoTi;
            public string NeiRong;
            public List<string> TuPian;

            public string YongHuMing;
            public string NiCheng;
            public string TouXiang;
            public string FaTieShiJian;

            public string CaoZuoLeiXing;
            public string CaoZuoRen;
            public DateTime CaoZuoShiJian;
        }
    }
}

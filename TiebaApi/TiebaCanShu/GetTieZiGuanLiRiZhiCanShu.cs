using System;
using System.Windows.Forms;
using TiebaApi.TiebaLeiXing;

namespace TiebaApi.TiebaCanShu
{
    /// <summary>
    /// 获取帖子管理日志参数
    /// </summary>
    public class GetTieZiGuanLiRiZhiCanShu
    {
        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie;

        /// <summary>
        /// 贴吧名
        /// </summary>
        public string TiebaName;

        /// <summary>
        /// 查询类型
        /// </summary>
        public TiebaBaWuHouTaiChaXunLeiXing ChaXunLeiXing;

        /// <summary>
        /// 用户名
        /// </summary>
        public string YongHuMing;

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime KaiShiRiQi;

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime JieShuRiQi;

        /// <summary>
        /// 操作类型
        /// </summary>
        public TieZiGuanLiRiZhiCaoZuoLeiXing CaoZuoLeiXing;

        /// <summary>
        /// 页数
        /// </summary>
        public int Pn;
    }
}

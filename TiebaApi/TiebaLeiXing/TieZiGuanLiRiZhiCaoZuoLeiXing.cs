
namespace TiebaApi.TiebaLeiXing
{
    /// <summary>
    /// 帖子管理日志操作类型
    /// </summary>
    public enum TieZiGuanLiRiZhiCaoZuoLeiXing
    {
        /// <summary>
        /// 全部操作
        /// </summary>
        QuanBuCaoZuo = 0,

        /// <summary>
        /// 屏蔽
        /// </summary>
        PingBi = 10032,

        /// <summary>
        /// 解除屏蔽
        /// </summary>
        JieChuPingBi = 10033,

        /// <summary>
        /// 删帖
        /// </summary>
        ShanTie = 12,

        /// <summary>
        /// 恢复删帖
        /// </summary>
        HuiFuShanTie = 13,

        /// <summary>
        /// 加精
        /// </summary>
        JiaJing = 17,

        /// <summary>
        /// 取消加精
        /// </summary>
        QuXiaoJiaJing = 18,

        /// <summary>
        /// 置顶
        /// </summary>
        ZhiDing = 25,

        /// <summary>
        /// 取消置顶
        /// </summary>
        QuXiaoZhiDing = 26
    }
}

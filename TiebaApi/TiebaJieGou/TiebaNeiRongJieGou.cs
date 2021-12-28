using TiebaApi.TiebaLeiXing;

namespace TiebaApi.TiebaJieGou
{
    public class TiebaNeiRongJieGou
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int SuoYin;

        /// <summary>
        /// 类型
        /// </summary>
        public TiebaNeiRongLeiXing LeiXing;

        /// <summary>
        /// 文本
        /// </summary>
        public string WenBen;
    }

    /// <summary>
    /// 文本结构
    /// </summary>
    public class WenBenJieGou : TiebaNeiRongJieGou
    {

    }

    /// <summary>
    /// 链接结构
    /// </summary>
    public class LianJieJieGou : TiebaNeiRongJieGou
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string LianJie;
    }

    /// <summary>
    /// 表情结构
    /// </summary>
    public class BiaoQingJieGou : TiebaNeiRongJieGou
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string DaiMa;
    }

    /// <summary>
    /// 图片结构
    /// </summary>
    public class TuPianJieGou : TiebaNeiRongJieGou
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string LianJie;

        /// <summary>
        /// 宽度
        /// </summary>
        public int KuanDu;

        /// <summary>
        /// 高度
        /// </summary>
        public int GaoDu;

        /// <summary>
        /// 大小
        /// </summary>
        public int DaXiao;

        /// <summary>
        /// 是否长图片
        /// </summary>
        public bool IsChangTuPian;

        /// <summary>
        /// 是否动图
        /// </summary>
        public bool IsDongTu;
    }

    /// <summary>
    /// 艾特结构
    /// </summary>
    public class AtJieGou : TiebaNeiRongJieGou
    {
        /// <summary>
        /// Uid
        /// </summary>
        public long Uid;
    }

    /// <summary>
    /// 视频结构
    /// </summary>
    public class ShiPinJieGou : TiebaNeiRongJieGou
    {

    }

    /// <summary>
    /// 电话号码结构
    /// </summary>
    public class DianHuaHaoMaJieGou : TiebaNeiRongJieGou
    {

    }

    /// <summary>
    /// 语音结构
    /// </summary>
    public class YuYinJieGou : TiebaNeiRongJieGou
    {

    }

    /// <summary>
    /// 活动结构
    /// </summary>
    public class HuoDongJieGou : TiebaNeiRongJieGou
    {

    }

    /// <summary>
    /// 话题结构
    /// </summary>
    public class HuaTiJieGou : TiebaNeiRongJieGou
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string LianJie;
    }

    /// <summary>
    /// 其他结构
    /// </summary>
    public class QiTaJieGou : TiebaNeiRongJieGou
    {

    }
}

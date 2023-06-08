using TiebaApi.TiebaLeiXing;

namespace TiebaApi.TiebaJieGou
{
    /// <summary>
    /// 贴吧Web内容结构
    /// </summary>
    public class TiebaWebNeiRongJieGou
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int SuoYin;

        /// <summary>
        /// 类型
        /// </summary>
        public TiebaWebNeiRongLeiXing LeiXing;

        /// <summary>
        /// 贴吧Web文本结构
        /// </summary>
        public TiebaWebWenBenJieGou WenBen;

        /// <summary>
        /// 贴吧Web链接结构
        /// </summary>
        public TiebaWebLianJieJieGou LianJie;

        /// <summary>
        /// 贴吧Web表情结构
        /// </summary>
        public TiebaWebBiaoQingJieGou BiaoQing;

        /// <summary>
        /// 贴吧Web图片结构
        /// </summary>
        public TiebaWebTuPianJieGou TuPian;

        /// <summary>
        /// 贴吧Web艾特结构
        /// </summary>
        public TiebaWebAtJieGou At;

        /// <summary>
        /// 贴吧Web视频结构
        /// </summary>
        public TiebaWebShiPinJieGou ShiPin;

        /// <summary>
        /// 贴吧Web电话号码结构
        /// </summary>
        public TiebaWebDianHuaHaoMaJieGou DianHuaHaoMa;

        /// <summary>
        /// 贴吧Web语音结构
        /// </summary>
        public TiebaWebYuYinJieGou YuYin;

        /// <summary>
        /// 贴吧Web活动结构
        /// </summary>
        public TiebaWebHuoDongJieGou HuoDong;

        /// <summary>
        /// 贴吧Web话题结构
        /// </summary>
        public TiebaWebHuaTiJieGou HuaTi;

        /// <summary>
        /// 贴吧Web其他结构
        /// </summary>
        public TiebaWebQiTaJieGou QiTa;
    }

    /// <summary>
    /// 贴吧Web文本结构
    /// </summary>
    public class TiebaWebWenBenJieGou
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string WenBen;
    }

    /// <summary>
    /// 贴吧Web链接结构
    /// </summary>
    public class TiebaWebLianJieJieGou
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string WenBen;

        /// <summary>
        /// 链接
        /// </summary>
        public string LianJie;
    }

    /// <summary>
    /// 贴吧Web表情结构
    /// </summary>
    public class TiebaWebBiaoQingJieGou
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string WenBen;

        /// <summary>
        /// 代码
        /// </summary>
        public string DaiMa;
    }

    /// <summary>
    /// 贴吧Web图片结构
    /// </summary>
    public class TiebaWebTuPianJieGou
    {
        /// <summary>
        /// 原图链接
        /// </summary>
        public string YuanTuLianJie;

        /// <summary>
        /// 缩略图链接
        /// </summary>
        public string SuoLueTuLianJie;

        /// <summary>
        /// 宽度
        /// </summary>
        public int KuanDu;

        /// <summary>
        /// 高度
        /// </summary>
        public int GaoDu;

        /// <summary>
        /// 图片大小
        /// </summary>
        public int DaXiao;

        /// <summary>
        /// 是否长图片
        /// </summary>
        public bool IsChangTuPian;
    }

    /// <summary>
    /// 贴吧Web艾特结构
    /// </summary>
    public class TiebaWebAtJieGou
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string WenBen;

        /// <summary>
        /// 用户名
        /// </summary>
        public string YongHuMing;

        /// <summary>
        /// 头像ID
        /// </summary>
        public string TouXiangID;

        /// <summary>
        /// Uid
        /// </summary>
        public long Uid;
    }

    /// <summary>
    /// 贴吧Web视频结构
    /// </summary>
    public class TiebaWebShiPinJieGou
    {
        /// <summary>
        /// 缩略图链接
        /// </summary>
        public string SuoLueTuLianJie;

        /// <summary>
        /// 视频链接1
        /// </summary>
        public string ShiPinLianJie1;

        /// <summary>
        /// 视频链接2
        /// </summary>
        public string ShiPinLianJie2;

        /// <summary>
        /// 宽度
        /// </summary>
        public int KuanDu;

        /// <summary>
        /// 高度
        /// </summary>
        public int GaoDu;

        /// <summary>
        /// 时长
        /// </summary>
        public int ShiChang;
    }

    /// <summary>
    /// 贴吧Web电话号码结构
    /// </summary>
    public class TiebaWebDianHuaHaoMaJieGou
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string WenBen;
    }

    /// <summary>
    /// 贴吧Web语音结构
    /// </summary>
    public class TiebaWebYuYinJieGou
    {
        /// <summary>
        /// 时长
        /// </summary>
        public int ShiChang;

        /// <summary>
        /// MD5
        /// </summary>
        public string MD5;
    }

    /// <summary>
    /// 贴吧Web活动结构
    /// </summary>
    public class TiebaWebHuoDongJieGou
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string WenBen;
    }

    /// <summary>
    /// 贴吧Web话题结构
    /// </summary>
    public class TiebaWebHuaTiJieGou
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string WenBen;

        /// <summary>
        /// 链接
        /// </summary>
        public string LianJie;
    }

    /// <summary>
    /// 贴吧Web其他结构
    /// </summary>
    public class TiebaWebQiTaJieGou
    {
        public string JSON;
    }
}

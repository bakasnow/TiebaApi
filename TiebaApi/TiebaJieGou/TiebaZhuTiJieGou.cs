using System;
using System.Collections.Generic;
namespace TiebaApi.TiebaJieGou
{
    public class TiebaZhuTiJieGou : TiebaUidJieGou
    {
        public int LeiXing;//类型
        public long Tid;//Tid

        public string BiaoTi;//标题
        public int DianJiLiang;//点击量
        public int HuiFuShu;//回复数

        public int ZanTongShu;//赞同数
        public int FanDuiShu;//反对数
        public int ZhuanFaShu;//转发量

        public DateTime FaTieShiJian;//发帖时间
        public long FaTieShiJianChuo;//发帖时间戳
        public DateTime ZuiHouHuiFuShiJian;//最后回复时间
        public long ZuiHouHuiFuShiJianChuo;//最后回复时间戳

        public bool IsZhiDing;//是否置顶
        public bool IsJingPin;//是否精品
        public bool IsHuiYuanZhiDing;//是否会员置顶
    }
}

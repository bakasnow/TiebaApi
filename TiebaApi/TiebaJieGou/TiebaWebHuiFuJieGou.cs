using System;
using TiebaApi.TiebaWebApi;

namespace TiebaApi.TiebaJieGou
{
    public class TiebaWebHuiFuJieGou : TiebaWebUidJieGou
    {
        public string BiaoTi;//标题

        public long Tid;
        public long Pid;

        public int LouCeng;//楼层
        public DateTime HuiFuShiJian;//回复时间
        public long HuiFuShiJianChuo;//回复时间戳
        public int ZanTongShu;//赞同数
        public int FanDuiShu;//反对数
        public int LzlHuiFuShu;//楼中楼回复数

        public TiebaWebNeiRong NeiRong;//内容
    }
}

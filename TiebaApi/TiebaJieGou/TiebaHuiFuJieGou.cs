using System;
using TiebaApi.TiebaAppApi;

namespace TiebaApi.TiebaJieGou
{
    public class TiebaHuiFuJieGou : TiebaUidJieGou
    {
        public string BiaoTi;//标题

        public long Tid;
        public long Pid;
        //public long Spid;

        public int LouCeng;//楼层
        public DateTime FaTieShiJian;//发帖时间
        public long FaTieShiJianChuo;//发帖时间戳
        public TiebaNeiRong NeiRong;//内容
        public int ZanTongShu;//赞同数
        public int FanDuiShu;//反对数

        public int LzlHuiFuShu;//楼中楼回复数
    }
}

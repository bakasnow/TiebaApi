using BakaSnowTool;
using BakaSnowTool.Http;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TiebaApi.TiebaTools;

namespace TiebaApi.TiebaWebApi
{
    public class TiebaGaoJiSouSuo
    {
        public string Html { get; private set; }

        public string Cookie;
        public string TiebaName;
        public string GuanJianCi;
        public string YongHuMing;
        public int PaiXuFangShi = 排序方式.按时间倒叙;
        public int XianShiTiaoShu = 显示条数.每页显示10条;
        public bool ZhiKanZhuTiTie = false;
        public int Pn { get; private set; }


        /// <summary>
        /// 获取网页源码
        /// </summary>
        public void GetHtml()
        {
            Html = TiebaHttpHelper.Get($"https://tieba.baidu.com/f/search/ures?kw={Http.UrlEncode(TiebaName)}&qw={Http.UrlEncode(GuanJianCi)}&rn={XianShiTiaoShu}&un={Http.UrlEncode(YongHuMing)}&only_thread={(ZhiKanZhuTiTie ? "1" : "")}&sm={PaiXuFangShi}&sd=&ed=&pn={Pn}", Cookie);
            //Console.WriteLine(Html);
        }

        /// <summary>
        /// 获取PostList网页源码
        /// </summary>
        /// <returns></returns>
        public string GetPostListHtml()
        {
            string html = BST.JieQuWenBen(Html, "<div class=\"s_post_list\">", "<div class=\"s_aside\">");
            return Http.GuoLvWangYeYuanMa(html);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public JieGuoJieGou Get(int pn)
        {
            Pn = pn;

            JieGuoJieGou jieGuoJieGou = new JieGuoJieGou();

            //获取网页源码
            GetHtml();
            if (string.IsNullOrEmpty(Html))
            {
                return jieGuoJieGou;
            }

            //获取尾页
            int.TryParse(BST.JianYiZhengZe(Html, "&pn=([0-9]*)\" class=\"last\">尾页</a>"), out jieGuoJieGou.MaxPn);

            //过滤网页源码
            string html = GetPostListHtml();

            MatchCollection mc = new Regex("<div class=\"s_post\">.*?</font></div>").Matches(html);
            jieGuoJieGou.JieGuoLieBiao = new List<JieGuoLieBiaoJieGou>();
            for (int i = 0; i < mc.Count; i++)
            {
                JieGuoLieBiaoJieGou jieGuoLieBiaoJieGou = new JieGuoLieBiaoJieGou();
                Regex headRegex = new Regex("<span class=\"p_title\"><a data-tid=\"[0-9]*\" data-fid=\"[0-9]*\" class=\"bluelink\" href=\"(.*?)\" class=\"bluelink\" target=\"_blank\" >(.*?)</a></span>");
                Match headMatch = headRegex.Match(mc[i].Value);
                jieGuoLieBiaoJieGou.Tid = BST.JieQuWenBen(headMatch.Groups[1].Value, "/p/", "?");
                jieGuoLieBiaoJieGou.Pid = BST.JieQuWenBen(headMatch.Groups[1].Value, "pid=", "&");
                jieGuoLieBiaoJieGou.BiaoTi = headMatch.Groups[2].Value.Replace("<em>", "").Replace("</em>", "");
                jieGuoLieBiaoJieGou.NeiRong = BST.JianYiZhengZe(mc[i].Value, "<div class=\"p_content\">(.*?)</div>\\s*贴吧：<a data-fid=").Replace("<em>", "").Replace("</em>", "");
                jieGuoLieBiaoJieGou.NeiRong = Http.HtmlDecode(jieGuoLieBiaoJieGou.NeiRong);
                jieGuoLieBiaoJieGou.TiebaName = BST.JianYiZhengZe(mc[i].Value, "贴吧：<a data-fid=\"[0-9]*\" class=\"p_forum\" href=\".*?\" target=\"_blank\"><font class=\"p_violet\">(.*?)</font>");
                jieGuoLieBiaoJieGou.YongHuMing = BST.JianYiZhengZe(mc[i].Value, "作者：<a href=\".*?\" target=_blank><font class=\"p_violet\">(.*?)</font></a>");
                jieGuoLieBiaoJieGou.ShiJian = Convert.ToDateTime(BST.JianYiZhengZe(mc[i].Value, "<font class=\"p_green p_date\">(.*?)</font></div>"));

                jieGuoJieGou.JieGuoLieBiao.Add(jieGuoLieBiaoJieGou);
            }

            jieGuoJieGou.HuoQuChengGong = true;
            return jieGuoJieGou;
        }

        /// <summary>
        /// 结果结构
        /// </summary>
        public class JieGuoJieGou
        {
            public bool HuoQuChengGong;
            public List<JieGuoLieBiaoJieGou> JieGuoLieBiao;
            public int MaxPn;
        }

        /// <summary>
        /// 结果列表结构
        /// </summary>
        public class JieGuoLieBiaoJieGou
        {
            public string Tid;
            public string Pid;
            public string BiaoTi;
            public string NeiRong;
            public string TiebaName;
            public string YongHuMing;
            public DateTime ShiJian;
        }

        /// <summary>
        /// 排序方式
        /// </summary>
        public class 排序方式
        {
            public const int 按时间倒叙 = 1;
            public const int 按时间顺序 = 0;
            public const int 按相关性排序 = 2;
        }

        /// <summary>
        /// 显示条数
        /// </summary>
        public class 显示条数
        {
            public const int 每页显示10条 = 10;
            public const int 每页显示20条 = 20;
            public const int 每页显示30条 = 30;
        }
    }
}

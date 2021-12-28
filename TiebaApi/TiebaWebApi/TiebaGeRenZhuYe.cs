using BakaSnowTool;
using BakaSnowTool.Http;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TiebaApi.TiebaTools;

namespace TiebaApi.TiebaWebApi
{
    public class TiebaGeRenZhuYe
    {
        private readonly string YongHuMing;

        /// <summary>
        /// 贴吧个人主页
        /// </summary>
        /// <param name="yongHuMing"></param>
        public TiebaGeRenZhuYe(string yongHuMing)
        {
            YongHuMing = yongHuMing;
        }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
        {
            string html = TiebaHttpHelper.Get($"http://tieba.baidu.com/home/main?un={Http.UrlEncode(YongHuMing)}&fr=home");
            html = Http.GuoLvWangYeYuanMa(html);
            return html;
        }

        /// <summary>
        /// 获取主题网页源码
        /// </summary>
        /// <returns></returns>
        public string GetZhuTiHtml()
        {
            string html = GetHtml();
            html = BST.JieQuWenBen(html, "<ul class=\"new_list clearfix\">", "</ul><div class=\"data_loading\">");
            return html;
        }

        /// <summary>
        /// 获取主题列表
        /// </summary>
        /// <returns></returns>
        public List<ZhuYeJieGou> GetZhuTiList()
        {
            List<ZhuYeJieGou> zhuTiLieBiao = new List<ZhuYeJieGou>();

            //获取主题部分的源码
            string html = GetZhuTiHtml();

            //删除图片部分的源码
            Regex tuPianRegex = new Regex("<ul class=\"n_media clearfix\".*?</ul>");
            if (tuPianRegex.IsMatch(html))
            {
                html = tuPianRegex.Replace(html, "");
            }

            //遍历主题列表
            Regex zhuTiRegex = new Regex("<div class=\"n_right( | n_right_first )clearfix\">(.*?)</div></div></div>");
            MatchCollection zhuTiMatch = zhuTiRegex.Matches(html);
            for (int zhuTiJiShu = 0; zhuTiJiShu < zhuTiMatch.Count; zhuTiJiShu++)
            {
                string duanLuo = zhuTiMatch[zhuTiJiShu].Value;
                //Console.WriteLine(duanLuo);

                ZhuYeJieGou zhuTiJieGou = new ZhuYeJieGou
                {
                    TiebaName = BST.JianYiZhengZe(duanLuo, "class=\"n_name\" title=\"(.*?)\">")
                };
                long.TryParse(BST.JianYiZhengZe(duanLuo, "<a href=\"/p/([0-9]*)\\?"), out zhuTiJieGou.Tid);
                zhuTiJieGou.BiaoTi = BST.JianYiZhengZe(duanLuo, "class=\"title\" locate=\".*?\" title=\"(.*?)\">");
                //zhuTiJieGou.NeiRongYuLan = BST.JianYiZhengZe(duanLuo, "<div class=\"n_txt\">\\s*(.*?)\\s*</div>");
                zhuTiJieGou.NeiRongYuLan = BST.JieQuWenBen(duanLuo, "<div class=\"n_txt\">", "</div>");
                zhuTiJieGou.YongHuMing = YongHuMing;
                DateTime.TryParse(BST.JianYiZhengZe(duanLuo, "<div class=\"n_post_time\">(.*?)</div>"), out zhuTiJieGou.FaTieShiJian);

                zhuTiLieBiao.Add(zhuTiJieGou);
            }

            return zhuTiLieBiao;
        }

        /// <summary>
        /// 主页结构
        /// </summary>
        public class ZhuYeJieGou
        {
            public string TiebaName;
            public long Tid;
            public string BiaoTi;
            public string NeiRongYuLan;
            public string YongHuMing;
            public DateTime FaTieShiJian;
        }
    }
}

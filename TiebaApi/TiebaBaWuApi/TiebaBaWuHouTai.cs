using BakaSnowTool;
using BakaSnowTool.Http;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using TiebaApi.TiebaCanShu;
using TiebaApi.TiebaLeiXing;
using TiebaApi.TiebaTools;
using TiebaApi.TiebaWebApi;
using static TiebaApi.TiebaJieGou.TiebaBaWuHouTaiJieGou;

namespace TiebaApi.TiebaBaWuApi
{
    public static class TiebaBaWuHouTai
    {
        /// <summary>
        /// 获取贴子管理日志
        /// </summary>
        /// <param name="canShu"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<TieZiGuanLiRiZhiJieGou> GetTieZiGuanLiRiZhi(GetTieZiGuanLiRiZhiCanShu canShu, out string msg)
        {
            List<TieZiGuanLiRiZhiJieGou> tieZiGuanLiRiZhi = new List<TieZiGuanLiRiZhiJieGou>();

            long kaiShiShiJianChuo = BST.QuShiJianChuo($"{canShu.KaiShiRiQi:yyyy-MM-dd} 00:00:00", "1970-01-01 08:00:00");
            long jieShuShiJianChuo = BST.QuShiJianChuo($"{canShu.JieShuRiQi:yyyy-MM-dd} 23:59:59", "1970-01-01 08:00:00");

            string url = $"https://tieba.baidu.com/bawu2/platform/listPostLog?" +
                $"word={Http.UrlEncode(canShu.TiebaName)}" +
                $"&op_type={(canShu.CaoZuoLeiXing == TieZiGuanLiRiZhiCaoZuoLeiXing.QuanBuCaoZuo ? "" : $"{(int)canShu.CaoZuoLeiXing}")}" +
                $"&stype={(canShu.ChaXunLeiXing == 0 ? "post_uname" : "op_uname")}" +
                $"&date_type=on" +
                $"&startTime={canShu.KaiShiRiQi:yyyy-MM-dd}" +
                $"&begin={kaiShiShiJianChuo}" +
                $"&endTime={canShu.JieShuRiQi:yyyy-MM-dd}" +
                $"&end={jieShuShiJianChuo}" +
                $"&svalue={Http.UrlEncodeUtf8(Http.UrlEncodeUtf8(canShu.YongHuMing))}";

            string html = TiebaHttpHelper.Get(url, canShu.Cookie);

            File.WriteAllText(@"C:\Users\bakas\Desktop\PS4吧吧务后台源码.html", html);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection tr_list = doc.DocumentNode?.SelectNodes("html/body/div[contains(@class,\"container\")]/div[contains(@class,\"main_content\")]/div[contains(@class,\"bawu-post-log\")]/table/tbody/tr");
            if (tr_list == null)
            {
                msg = "tr获取失败";
                return tieZiGuanLiRiZhi;
            }

            foreach (var tr in tr_list)
            {
                TieZiGuanLiRiZhiJieGou tieZiGuanLiJieGou = new TieZiGuanLiRiZhiJieGou();

                //链接
                string lianJie = tr?.SelectSingleNode("td[@class='left_cell']/article/div[@class='post_content']/h1/a")?.Attributes["href"]?.Value;
                long.TryParse(BST.JieQuWenBen(lianJie, "/p/", "?"), out tieZiGuanLiJieGou.Tid);
                long.TryParse(BST.JieQuWenBen(lianJie, "&pid=", "#"), out tieZiGuanLiJieGou.Pid);

                //标题
                tieZiGuanLiJieGou.BiaoTi = tr?.SelectSingleNode("td[@class='left_cell']/article/div[@class='post_content']/h1/a")?.InnerText;

                //内容
                tieZiGuanLiJieGou.NeiRong = tr?.SelectSingleNode("td[@class='left_cell']/article/div[@class='post_content']/div[@class='post_text']")?.InnerText.Trim();

                //图片
                tieZiGuanLiJieGou.TuPian = new List<string>();
                HtmlNodeCollection post_media_ul_li_list = tr?.SelectNodes("td[@class='left_cell']/article/div[@class='post_content']/div[@class='post_media']/ul/li");
                if (post_media_ul_li_list != null)
                {
                    foreach (var li in post_media_ul_li_list)
                    {
                        tieZiGuanLiJieGou.TuPian.Add(li.SelectSingleNode("a")?.Attributes["href"]?.Value);
                    }
                }

                //用户名
                string yongHuMing = tr?.SelectSingleNode("td[@class='left_cell']/article/div[@class='post_meta']/div[1]/a")?.InnerText;
                tieZiGuanLiJieGou.YongHuMing = yongHuMing.Replace("用户名: ", string.Empty);

                //昵称
                string niCheng = tr?.SelectSingleNode("td[@class='left_cell']/article/div[@class='post_meta']/div[2]/a")?.InnerText;
                tieZiGuanLiJieGou.NiCheng = niCheng.Replace("昵称: ", string.Empty);
                if (tieZiGuanLiJieGou.NiCheng == "--")
                {
                    tieZiGuanLiJieGou.NiCheng = string.Empty;
                }

                //头像
                string touXiang = tr?.SelectSingleNode("td[@class='left_cell']/article/div[@class='post_meta']/div[2]/a")?.Attributes["href"]?.Value;
                tieZiGuanLiJieGou.TouXiangID = Tieba.GuoLvTouXiangID(BST.JieQuWenBen(touXiang, "id=", "&"));

                //发帖时间
                tieZiGuanLiJieGou.FaTieShiJian = tr?.SelectSingleNode("td[@class='left_cell']/article/div[@class='post_meta']/time[@class='ui_text_desc']")?.InnerText;

                //操作类型
                tieZiGuanLiJieGou.CaoZuoLeiXing = tr?.SelectSingleNode("td/span")?.InnerText;

                //操作人
                tieZiGuanLiJieGou.CaoZuoRen = tr?.SelectSingleNode("td/a[@class='ui_text_normal']")?.InnerText;

                //操作时间
                string caoZuoShiJian = tr?.SelectSingleNode("td[4]")?.InnerHtml;
                caoZuoShiJian = caoZuoShiJian.Replace("<br>", " ");
                DateTime.TryParse(caoZuoShiJian, out tieZiGuanLiJieGou.CaoZuoShiJian);

                tieZiGuanLiRiZhi.Add(tieZiGuanLiJieGou);
            }

            msg = "获取成功";
            return tieZiGuanLiRiZhi;
        }
    }
}

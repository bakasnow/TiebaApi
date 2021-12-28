using BakaSnowTool.Http;
using CsharpHttpHelper;
using CsharpHttpHelper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TiebaApi.TiebaWebApi
{
    public class TiebaGeRenZhuYeChuPingBan
    {
        private const string RegexText = "<div class=\"tab tab_holo home_tab j_home_tab\"><a href=\"#\" class=\"j_home_tab_item tab_item home_tab_item active\"><span class=\"home_tab_item_num\">([0-9]*)</span><span class=\"home_tab_item_title\">贴子</span></a><a href=\"#\" class=\"j_home_tab_item tab_item home_tab_item\"><span class=\"home_tab_item_num\">([0-9]*)</span><span class=\"home_tab_item_title\">贴吧</span></a><a href=\"#\" class=\"j_home_tab_item tab_item home_tab_item\"><span class=\"home_tab_item_num\">([0-9]*)</span><span class=\"home_tab_item_title\">关注</span></a><a href=\"#\" class=\"j_home_tab_item tab_item home_tab_item\"><span class=\"home_tab_item_num\">([0-9]*)</span><span class=\"home_tab_item_title\">粉丝</span></a></div>";

        private static string Cookie;

        private static string ID;

        public int TieZiShu = 0;

        public int TiebaShu = 0;

        public int GuanZhuShu = 0;

        public int FenSiShu = 0;

        public bool Get(string cookie, string id)
        {
            Cookie = cookie;
            ID = id;

            string html = GetHtml();
            MatchCollection match = new Regex(RegexText).Matches(html);
            if (match.Count > 0)
            {
                int.TryParse(match[0].Groups[1].Value, out TieZiShu);
                int.TryParse(match[0].Groups[2].Value, out TiebaShu);
                int.TryParse(match[0].Groups[3].Value, out GuanZhuShu);
                int.TryParse(match[0].Groups[4].Value, out FenSiShu);

                return true;
            }

            return false;
        }

        public string GetHtml()
        {
            string url = "https://tieba.baidu.com/home/main?";
            if (ID.Length >= 36)
            {
                url += "id=" + ID;
            }
            else
            {
                url += "un=" + Http.UrlEncode(ID);
            }

            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项
                Method = "GET",//URL     可选项 默认为Get
                Timeout = 100000,//连接超时时间     可选项默认为100000
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                Cookie = Cookie,//字符串Cookie     可选项
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Mobile Safari/537.36",//用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                ContentType = "text/html",//返回类型    可选项有默认值
                Referer = "http://tieba.baidu.com/",//来源URL     可选项
                Allowautoredirect = false,//是否根据３０１跳转     可选项
                AutoRedirectCookie = false,//是否自动处理Cookie     可选项
                Postdata = "",//Post数据     可选项GET时不需要写
                ResultType = ResultType.String,//返回数据类型，是Byte还是String
            };
            HttpResult result = http.GetHtml(item);
            string html = result.Html;
            //string cookie = result.Cookie;

            return html;
        }
    }
}

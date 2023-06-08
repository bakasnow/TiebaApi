using BakaSnowTool;
using CsharpHttpHelper.Enum;
using CsharpHttpHelper;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TiebaApi.TiebaJieGou;
using System.Xml.Linq;

namespace TiebaApi.TiebaWebApi
{
    /// <summary>
    /// 贴吧Web回复
    /// </summary>
    public class TiebaWebHuiFu
    {
        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie = string.Empty;

        /// <summary>
        /// 贴吧名
        /// </summary>
        public string TiebaName { private set; get; }

        /// <summary>
        /// Fid
        /// </summary>
        public long Fid { private set; get; }

        /// <summary>
        /// 帖号
        /// </summary>
        public long Tid = 0;

        /// <summary>
        /// 标题
        /// </summary>
        public string BiaoTi { private set; get; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int Pn { private set; get; }

        /// <summary>
        /// 获取条数
        /// </summary>
        public int Rn { private set; get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string CuoWuXinXi { private set; get; }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = $"https://tieba.baidu.com/mg/p/getPbData?kz={Tid}&obj_param2=safari&format=json&eqid=&refer=tieba.baidu.com&prev=frs&source=a0-bindex-c-d-e0&pn={Pn}&rn={Rn}",
                Method = "GET",//URL     可选项 默认为Get
                Timeout = 100000,//连接超时时间     可选项默认为100000
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                Cookie = Cookie,//字符串Cookie     可选项
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/110.0.0.0",//用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                ContentType = "application/json; charset=utf-8",//返回类型    可选项有默认值
                Referer = "",//来源URL     可选项
                Allowautoredirect = false,//是否根据３０１跳转     可选项
                AutoRedirectCookie = false,//是否自动处理Cookie     可选项
                Postdata = "",//Post数据     可选项GET时不需要写
                ResultType = ResultType.String,//返回数据类型，是Byte还是String
            };
            HttpResult result = http.GetHtml(item);

            return result.Html;
        }

        /// <summary>
        /// 获取回复列表
        /// </summary>
        /// <param name="pn">当前页数</param>
        /// <returns></returns>
        public List<TiebaWebHuiFuJieGou> Get(int pn = 1, int rn = 10)
        {
            List<TiebaWebHuiFuJieGou> huiFuLieBiao = new List<TiebaWebHuiFuJieGou>();

            //当前页数
            Pn = pn;

            //获取条数
            Rn = rn;

            //获取网页源码
            string html = GetHtml();

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                CuoWuXinXi = "HTML获取失败";
                return huiFuLieBiao;
            }

            //解析
            JObject huiFuJsonData;
            try
            {
                huiFuJsonData = JObject.Parse(html);
            }
            catch
            {
                CuoWuXinXi = "JSON解析失败1";
                return huiFuLieBiao;
            }

            //访问失败
            if (huiFuJsonData["errno"]?.ToString() != "0")
            {
                CuoWuXinXi = huiFuJsonData["errmsg"]?.ToString();
                return huiFuLieBiao;
            }

            //贴吧信息
            var forum = huiFuJsonData["data"]?["forum"];
            if (forum == null)
            {
                TiebaName = forum["name"]?.ToString();
                if (long.TryParse(forum["id"]?.ToString(), out long fid))
                {
                    Fid = fid;
                }
            }

            var post_list = huiFuJsonData["data"]?["post_list"];
            if (post_list == null)
            {
                CuoWuXinXi = "JSON解析失败2";
                return huiFuLieBiao;
            }

            foreach (var post in post_list)
            {
                TiebaWebHuiFuJieGou huiFuJieGou = new TiebaWebHuiFuJieGou();

                if (string.IsNullOrEmpty(BiaoTi))
                {
                    huiFuJieGou.BiaoTi = post["title"]?.ToString();
                    if (huiFuJieGou.BiaoTi.StartsWith("回复："))
                    {
                        huiFuJieGou.BiaoTi = huiFuJieGou.BiaoTi.Substring(3);
                    }

                    BiaoTi = huiFuJieGou.BiaoTi;
                }
                else
                {
                    huiFuJieGou.BiaoTi = BiaoTi;
                }

                huiFuJieGou.Tid = Tid;
                long.TryParse(post["id"]?.ToString(), out huiFuJieGou.Pid);
                int.TryParse(post["floor"]?.ToString(), out huiFuJieGou.LouCeng);
                long.TryParse(post["time"]?.ToString(), out huiFuJieGou.HuiFuShiJianChuo);
                huiFuJieGou.HuiFuShiJian = BST.ShiJianChuoDaoShiJian(huiFuJieGou.HuiFuShiJianChuo * 1000);
                int.TryParse(post["sub_post_number"]?.ToString(), out huiFuJieGou.LzlHuiFuShu);
                int.TryParse(post["agree"]?["agree_num"]?.ToString(), out huiFuJieGou.ZanTongShu);
                int.TryParse(post["agree"]?["disagree_num"]?.ToString(), out huiFuJieGou.FanDuiShu);

                //回复内容
                huiFuJieGou.NeiRong = new TiebaWebNeiRong(post["content"]);

                //用户信息
                long.TryParse(post["author"]?["id"]?.ToString(), out huiFuJieGou.Uid);
                huiFuJieGou.YongHuMing = post["author"]?["name"]?.ToString();
                huiFuJieGou.NiCheng = post["author"]?["name_show"]?.ToString();
                huiFuJieGou.FuGaiMing = post["author"]?["show_nickname"]?.ToString();
                huiFuJieGou.TouXiangID = Tieba.GuoLvTouXiangID(post["author"]?["portrait"]?.ToString());

                huiFuLieBiao.Add(huiFuJieGou);
            }

            return huiFuLieBiao;
        }
    }
}
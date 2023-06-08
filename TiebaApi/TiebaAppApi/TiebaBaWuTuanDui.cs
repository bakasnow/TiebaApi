using BakaSnowTool.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaLeiXing;
using TiebaApi.TiebaTools;
using TiebaApi.TiebaWebApi;

namespace TiebaApi.TiebaAppApi
{
    public class TiebaBaWuTuanDui
    {
        /// <summary>
        /// 贴吧吧务团队
        /// </summary>
        /// <param name="tiebaName"></param>
        public TiebaBaWuTuanDui(string tiebaName)
        {
            TiebaName = tiebaName;
        }

        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie = string.Empty;

        /// <summary>
        /// 贴吧名
        /// </summary>
        public string TiebaName { private set; get; }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
        {
            string url = "http://c.tieba.baidu.com/c/f/forum/getBawuInfo";
            string postStr
                = Cookie
                + $"&_client_id={Tieba.GetAndroidStamp()}"
                + "&_client_type=2"
                + "&_client_version=12.6.0.1"
                + $"&forum_id={TiebaWeb.GetTiebaFid(TiebaName)}"
                + $"&kw={Http.UrlEncodeUtf8(TiebaName)}";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            return TiebaHttpHelper.Post(url, postStr);
        }

        /// <summary>
        /// 获取吧务团队列表
        /// </summary>
        /// <param name="pn">当前页数</param>
        /// <returns></returns>
        public List<TiebaBaWuTuanDuiJieGou> Get()
        {
            //主题列表
            List<TiebaBaWuTuanDuiJieGou> baWuTuanDuiLieBiao = new List<TiebaBaWuTuanDuiJieGou>();

            //获取网页源码
            string html = GetHtml();

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                return baWuTuanDuiLieBiao;
            }

            //解析
            JObject baWuTuanDuiJsonData;
            try
            {
                baWuTuanDuiJsonData = JObject.Parse(html);
            }
            catch
            {
                return baWuTuanDuiLieBiao;
            }

            //访问失败
            if (baWuTuanDuiJsonData["error_code"]?.ToString() != "0")
            {
                return baWuTuanDuiLieBiao;
            }

            var bawu_team_list = baWuTuanDuiJsonData["bawu_team_info"]?["bawu_team_list"];
            for (int x = 0; x < bawu_team_list.Count(); x++)
            {
                string role_name = (string)bawu_team_list[x]?["role_name"];
                TiebaBaWuLeiXing zhiWu = TiebaLeiXingZhuanHuan.TiebaBaWuWenBenZhuanLeiXing(role_name);

                var role_info = bawu_team_list[x]?["role_info"];
                for (int y = 0; y < role_info.Count(); y++)
                {
                    TiebaBaWuTuanDuiJieGou tiebaBaWuTuanDuiJieGou = new TiebaBaWuTuanDuiJieGou
                    {
                        Uid = (long)role_info[y]?["user_id"],
                        YongHuMing = (string)role_info[y]?["user_name"],
                        NiCheng = (string)role_info[y]?["name_show"],
                        TouXiangID = (string)role_info[y]?["portrait"],
                        DengJi = (int)role_info[y]?["user_level"],
                        ZhiWu = zhiWu
                    };

                    baWuTuanDuiLieBiao.Add(tiebaBaWuTuanDuiJieGou);
                }
            }

            return baWuTuanDuiLieBiao;
        }
    }
}

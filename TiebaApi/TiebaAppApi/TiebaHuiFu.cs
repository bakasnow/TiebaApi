using BakaSnowTool;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaTools;
using TiebaApi.TiebaWebApi;

namespace TiebaApi.TiebaAppApi
{
    public class TiebaHuiFu
    {
        /// <summary>
        /// 贴吧回复
        /// </summary>
        /// <param name="tiebaName"></param>
        public TiebaHuiFu(string tiebaName)
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
        /// Tid
        /// </summary>
        public long Tid = 0;

        /// <summary>
        /// 当前页数
        /// </summary>
        public int Pn { private set; get; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int ZongYeShu { private set; get; }

        /// <summary>
        /// 是否倒叙
        /// </summary>
        public bool IsDaoXu { set; private get; }

        /// <summary>
        /// 回帖权限
        /// </summary>
        public int HuiTieQuanXian { private set; get; }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        public string GetHtml()
        {
            string url = "http://c.tieba.baidu.com/c/f/pb/page";
            string postStr
                = Cookie
                + $"&_client_id={Tieba.GetAndroidStamp()}"
                + "&_client_type=2"
                + "&_client_version=12.6.0.1"
                + $"&kz={Tid}";

            if (IsDaoXu)
                postStr += "&last=1&r=1";
            else
                postStr += "&pn=" + Pn.ToString();

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            return TiebaHttpHelper.Post_App(url, postStr);
        }

        /// <summary>
        /// 获取回复列表
        /// </summary>
        /// <param name="pn">当前页数</param>
        /// <returns></returns>
        public List<TiebaHuiFuJieGou> Get(int pn)
        {
            //回复列表
            List<TiebaHuiFuJieGou> huiFuLieBiao = new List<TiebaHuiFuJieGou>();

            //当前页数
            Pn = pn;

            //获取网页源码
            string html = GetHtml();

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
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
                return huiFuLieBiao;
            }

            //访问失败
            if (huiFuJsonData["error_code"]?.ToString() != "0")
            {
                return huiFuLieBiao;
            }

            //总页数
            try
            {
                ZongYeShu = Convert.ToInt32(huiFuJsonData["page"]?["total_page"]);
            }
            catch
            {
                ZongYeShu = -1;
            }

            var post_list = huiFuJsonData["post_list"];
            var user_list = huiFuJsonData["user_list"];

            //回帖权限
            try
            {
                HuiTieQuanXian = Convert.ToInt32(huiFuJsonData["thread"]?["author"]?["priv_sets"]?["reply"]);
            }
            catch
            {
                HuiTieQuanXian = 0;
            }

            foreach (var post in post_list)
            {
                //是否被折叠
                if (post["id"]?.ToString() == "0")
                {
                    continue;
                }

                //帖子参数
                TiebaHuiFuJieGou huiFuJieGou = new TiebaHuiFuJieGou
                {
                    BiaoTi = post["thread"]?["title"]?.ToString(),
                    Tid = Tid
                };
                long.TryParse(post["id"]?.ToString(), out huiFuJieGou.Pid);
                //huiFuJieGou.Spid = -1;
                int.TryParse(post["floor"]?.ToString(), out huiFuJieGou.LouCeng);
                long.TryParse(post["time"]?.ToString(), out huiFuJieGou.FaTieShiJianChuo);
                huiFuJieGou.FaTieShiJian = BST.ShiJianChuoDaoShiJian(huiFuJieGou.FaTieShiJianChuo * 1000);
                int.TryParse(post["sub_post_number"]?.ToString(), out huiFuJieGou.LzlHuiFuShu);
                int.TryParse(post["agree"]?["agree_num"]?.ToString(), out huiFuJieGou.ZanTongShu);
                int.TryParse(post["agree"]?["disagree_num"]?.ToString(), out huiFuJieGou.FanDuiShu);

                //层主信息
                foreach (var user in user_list)
                {
                    if (user["id"]?.ToString() == post["author_id"]?.ToString())
                    {
                        long.TryParse(user["id"]?.ToString(), out huiFuJieGou.Uid);
                        huiFuJieGou.YongHuMing = user["name"]?.ToString();
                        huiFuJieGou.NiCheng = user["name_show"]?.ToString();
                        huiFuJieGou.TouXiang = Tieba.GuoLvTouXiangID(user["portrait"]?.ToString());
                        int.TryParse(user["level_id"]?.ToString(), out huiFuJieGou.DengJi);
                        huiFuJieGou.IsBaWu = user["is_bawu"]?.ToString() == "1";
                        huiFuJieGou.YinJi = new TiebaYinJi(user["iconinfo"]);
                        break;
                    }
                }

                //帖子内容
                huiFuJieGou.NeiRong = new TiebaNeiRong(post["content"]);

                huiFuLieBiao.Add(huiFuJieGou);
            }

            return huiFuLieBiao;
        }
    }
}

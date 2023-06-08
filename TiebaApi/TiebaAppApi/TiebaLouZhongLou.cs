using BakaSnowTool;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaTools;
using TiebaApi.TiebaWebApi;

namespace TiebaApi.TiebaAppApi
{
    public class TiebaLouZhongLou
    {
        /// <summary>
        /// 贴吧楼中楼
        /// </summary>
        /// <param name="tiebaName"></param>
        public TiebaLouZhongLou(string tiebaName)
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
        /// Pid
        /// </summary>
        public long Pid = 0;

        /// <summary>
        /// 当前页数
        /// </summary>
        public int Pn { private set; get; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int ZongYeShu { private set; get; }

        ///// <summary>
        ///// 是否倒叙
        ///// </summary>
        //public bool IsDaoXu { set; private get; }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        public string GetHtml()
        {
            string url = "http://c.tieba.baidu.com/c/f/pb/floor";
            string postStr
                = Cookie
                + $"&_client_id={Tieba.GetAndroidStamp()}"
                + "&_client_type=2"
                + "&_client_version=12.6.0.1"
                + $"&kz={Tid}"
                + $"&pid={Pid}"
                + $"&pn={Pn}";

            //if (IsDaoXu)
            //    postStr += "&last=1&r=1";
            //else
            //    postStr += "&pn=" + Pn.ToString();

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            return TiebaHttpHelper.Post(url, postStr);
        }

        /// <summary>
        /// 获取楼中楼列表
        /// </summary>
        /// <param name="pn">当前页数</param>
        /// <returns></returns>
        public List<TiebaLouZhongLouJieGou> Get(int pn)
        {
            //当前页数
            Pn = pn;

            //获取网页源码
            string html = GetHtml();

            //楼中楼列表
            List<TiebaLouZhongLouJieGou> louZhongLouLieBiao = new List<TiebaLouZhongLouJieGou>();

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                return louZhongLouLieBiao;
            }

            //解析
            JObject huiFuJsonData;
            try
            {
                huiFuJsonData = JObject.Parse(html);
            }
            catch
            {
                return louZhongLouLieBiao;
            }

            //访问失败
            if (huiFuJsonData["error_code"]?.ToString() != "0")
            {
                return louZhongLouLieBiao;
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

            var subpost_list = huiFuJsonData["subpost_list"];

            foreach (var subpost in subpost_list)
            {
                //是否被折叠
                if (subpost["id"]?.ToString() == "0")
                {
                    continue;
                }

                //楼中楼参数
                TiebaLouZhongLouJieGou louZhongLouJieGou = new TiebaLouZhongLouJieGou
                {
                    Tid = Tid,
                    Pid = Pid
                };
                long.TryParse(subpost["id"]?.ToString(), out louZhongLouJieGou.Spid);
                int.TryParse(subpost["floor"]?.ToString(), out louZhongLouJieGou.LouCeng);//这个好像一直都是0
                long.TryParse(subpost["time"]?.ToString(), out louZhongLouJieGou.FaTieShiJianChuo);
                louZhongLouJieGou.FaTieShiJian = BST.ShiJianChuoDaoShiJian(louZhongLouJieGou.FaTieShiJianChuo * 1000);
                int.TryParse(subpost["agree"]?["agree_num"]?.ToString(), out louZhongLouJieGou.ZanTongShu);
                int.TryParse(subpost["agree"]?["disagree_num"]?.ToString(), out louZhongLouJieGou.FanDuiShu);

                //层主信息
                long.TryParse(subpost["author"]?["id"]?.ToString(), out louZhongLouJieGou.Uid);
                louZhongLouJieGou.YongHuMing = subpost["author"]?["name"]?.ToString();
                louZhongLouJieGou.NiCheng = subpost["author"]?["name_show"]?.ToString();
                louZhongLouJieGou.TouXiangID = Tieba.GuoLvTouXiangID(subpost["author"]?["portrait"]?.ToString());
                int.TryParse(subpost["author"]?["level_id"]?.ToString(), out louZhongLouJieGou.DengJi);
                louZhongLouJieGou.IsBaWu = subpost["author"]?["is_bawu"]?.ToString() == "1";
                louZhongLouJieGou.YinJi = new TiebaYinJi(subpost["author"]?["iconinfo"]);

                //帖子内容
                louZhongLouJieGou.NeiRong = new TiebaNeiRong(subpost["content"]);

                louZhongLouLieBiao.Add(louZhongLouJieGou);
            }

            return louZhongLouLieBiao;
        }
    }
}

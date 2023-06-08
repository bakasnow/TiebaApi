using BakaSnowTool;
using CsharpHttpHelper.Enum;
using CsharpHttpHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using TiebaApi.TiebaJieGou;
using System.Diagnostics;
using BakaSnowTool.Http;

namespace TiebaApi.TiebaWebApi
{
    public class TiebaWebZhuTi
    {
        /// <summary>
        /// 贴吧Web主题
        /// </summary>
        /// <param name="tiebaName"></param>
        public TiebaWebZhuTi(string tiebaName)
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
        /// 当前页数
        /// </summary>
        public int Pn { private set; get; }

        /// <summary>
        /// 获取条数
        /// </summary>
        public int Rn { private set; get; }

        /// <summary>
        /// 是否精品区
        /// </summary>
        public bool IsJingPin { private set; get; }

        /// <summary>
        /// 是否发布排序
        /// </summary>
        public bool IsFaBuPaiXu { private set; get; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int ZongYeShu { private set; get; }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = $"https://tieba.baidu.com/mg/f/getFrsData?kw={Http.UrlDecode(TiebaName)}&rn={Rn}&pn={Pn}&is_good={(IsJingPin ? "1" : "0")}&cid=0&sort_type={(IsFaBuPaiXu ? "1" : "0")}&fr=search&default_pro=0&only_thread_list=0&eqid=&refer=tieba.baidu.com",//URL     必需项
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
        /// 获取主题列表
        /// </summary>
        /// <param name="pn">当前页数</param>
        /// <returns></returns>
        public List<TiebaWebZhuTiJieGou> Get(int pn = 1, int rn = 30, bool isFaBuPaiXu = false, bool isJingPing = false)
        {
            //主题列表
            List<TiebaWebZhuTiJieGou> zhuTiLieBiao = new List<TiebaWebZhuTiJieGou>();

            //当前页数
            Pn = pn;

            //获取条数
            Rn = rn;

            //是否发布排序
            IsFaBuPaiXu = isFaBuPaiXu;

            //是否精品区
            IsJingPin = isJingPing;

            //获取网页源码
            string html = GetHtml();

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                return zhuTiLieBiao;
            }

            //解析
            JObject zhuTiJsonData;
            try
            {
                zhuTiJsonData = JObject.Parse(html);
            }
            catch
            {
                return zhuTiLieBiao;
            }

            //访问失败
            if (zhuTiJsonData["errno"]?.ToString() != "0")
            {
                return zhuTiLieBiao;
            }

            //总页数
            try
            {
                ZongYeShu = Convert.ToInt32(zhuTiJsonData["page"]?["total_page"]);
            }
            catch
            {
                ZongYeShu = -1;
            }

            var forum = zhuTiJsonData["data"]?["forum"];
            if (forum == null)
            {
                return zhuTiLieBiao;
            }

            var thread_list = zhuTiJsonData["data"]?["thread_list"];
            if (thread_list == null)
            {
                return zhuTiLieBiao;
            }

            #region "主题参数处理"
            foreach (var thread in thread_list)
            {
                TiebaWebZhuTiJieGou zhuTiJieGou = new TiebaWebZhuTiJieGou();

                //主题参数
                long.TryParse(forum["id"]?.ToString(), out zhuTiJieGou.Fid);
                zhuTiJieGou.TiebaName = forum["name"]?.ToString();

                int.TryParse(thread["thread_types"]?.ToString(), out zhuTiJieGou.LeiXing);
                long.TryParse(thread["tid"]?.ToString(), out zhuTiJieGou.Tid);

                zhuTiJieGou.BiaoTi = thread["title"]?.ToString().Replace("\r", "").Replace("\n", "");
                zhuTiJieGou.NeiRongYuLan = thread["abstract"]?[0]?.ToString().Replace("\r", "").Replace("\n", "");

                //图片列表
                zhuTiJieGou.TuPianLieBiao = new List<TiebaWebTuPianJieGou>();
                zhuTiJieGou.ShiPinLieBiao = new List<TiebaWebShiPinJieGou>();
                var media_list = thread["media"];
                if (media_list != null)
                {
                    foreach (var media in media_list)
                    {
                        if (media["type"]?.ToString() == "pic")
                        {
                            zhuTiJieGou.TuPianLieBiao.Add(new TiebaWebTuPianJieGou
                            {
                                YuanTuLianJie = media["big_pic"]?.ToString(),
                                SuoLueTuLianJie = media["small_pic"]?.ToString(),
                                KuanDu = Convert.ToInt32(media["width"]?.ToString()),
                                GaoDu = Convert.ToInt32(media["height"]?.ToString()),
                                IsChangTuPian = media["is_long_pic"]?.ToString() == "1"

                            });
                        }
                        else if (media["type"]?.ToString() == "flash")
                        {
                            zhuTiJieGou.ShiPinLieBiao.Add(new TiebaWebShiPinJieGou
                            {
                                SuoLueTuLianJie = media["vpic"]?.ToString(),
                                ShiPinLianJie1 = media["vhsrc"]?.ToString(),
                                ShiPinLianJie2 = media["vsrc"]?.ToString()
                            });
                        }
                        else
                        {
                            Debug.WriteLine(media.ToString());
                        }
                    }
                }

                //语音列表
                zhuTiJieGou.YuYinLieBiao = new List<TiebaWebYuYinJieGou>();
                var voice_info_list = thread["voice_info"];
                if (voice_info_list != null)
                {
                    foreach (var yuYin in thread["voice_info"])
                    {
                        zhuTiJieGou.YuYinLieBiao.Add(
                            new TiebaWebYuYinJieGou
                            {
                                ShiChang = Convert.ToInt32(yuYin["during_time"]?.ToString()),
                                MD5 = yuYin["voice_md5"]?.ToString()
                            });
                    }
                }

                //回复数、赞同数、反对数
                int.TryParse(thread["reply_num"]?.ToString(), out zhuTiJieGou.HuiFuShu);
                int.TryParse(thread["agree"]?["agree_num"]?.ToString(), out zhuTiJieGou.ZanTongShu);
                int.TryParse(thread["agree"]?["disagree_num"]?.ToString(), out zhuTiJieGou.FanDuiShu);

                //时间参数
                long.TryParse(thread["create_time"]?.ToString(), out zhuTiJieGou.FaTieShiJianChuo);
                zhuTiJieGou.FaTieShiJian = BST.ShiJianChuoDaoShiJian(zhuTiJieGou.FaTieShiJianChuo * 1000);
                long.TryParse(thread["last_time_int"]?.ToString(), out zhuTiJieGou.ZuiHouHuiFuShiJianChuo);
                zhuTiJieGou.ZuiHouHuiFuShiJian = BST.ShiJianChuoDaoShiJian(zhuTiJieGou.ZuiHouHuiFuShiJianChuo * 1000);

                //通用参数
                zhuTiJieGou.IsZhiDing = thread["is_top"]?.ToString() == "1";
                zhuTiJieGou.IsHuiYuanZhiDing = thread["is_membertop"]?.ToString() == "1";
                zhuTiJieGou.IsHuaTi = thread["live_ad_type_show"]?.ToString() == "话题";

                //楼主信息
                long.TryParse(thread["author"]?["id"]?.ToString(), out zhuTiJieGou.Uid);
                zhuTiJieGou.YongHuMing = thread["author"]?["name"]?.ToString();
                zhuTiJieGou.NiCheng = thread["author"]?["name_show"]?.ToString();
                zhuTiJieGou.FuGaiMing = thread["author"]?["show_nickname"]?.ToString();
                zhuTiJieGou.TouXiangID = Tieba.GuoLvTouXiangID(thread["author"]?["portrait"]?.ToString());

                zhuTiLieBiao.Add(zhuTiJieGou);
            }
            #endregion

            return zhuTiLieBiao;
        }
    }
}

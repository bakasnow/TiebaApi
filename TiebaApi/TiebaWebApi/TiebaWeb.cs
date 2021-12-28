using BakaSnowTool;
using BakaSnowTool.Http;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TiebaApi.TiebaForms;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaTools;

namespace TiebaApi.TiebaWebApi
{
    public static class TiebaWeb
    {
        /// <summary>
        /// 取百度Tbs
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string GetBaiduTbs(string cookie)
        {
            string html = TiebaHttpHelper.Get("http://tieba.baidu.com/dc/common/tbs", cookie);
            return BST.JieQuWenBen(html, "\"tbs\":\"", "\"");
        }

        /// <summary>
        /// 获取百度账号在线状态
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static bool GetBaiduZhangHaoIsZaiXian(string cookie)
        {
            string html = TiebaHttpHelper.Get("http://tieba.baidu.com/dc/common/tbs", cookie);
            return BST.JieQuWenBen(html, "\"is_login\":", "}") == "1";
        }

        /// <summary>
        /// 取贴吧Fid
        /// </summary>
        /// <param name="tiebaName">贴吧名</param>
        /// <returns></returns>
        public static long GetTiebaFid(string tiebaName)
        {
            string html = TiebaHttpHelper.Get($"http://tieba.baidu.com/f/commit/share/fnameShareApi?fname={Http.UrlEncodeUtf8(tiebaName)}&ie=utf-8");
            if (long.TryParse(BST.JieQuWenBen(html, "\"fid\":", ","), out long fid))
            {
                return fid;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取贴吧账号信息
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static TiebaZhangHaoXinXiJieGou GetTiebaZhangHaoXinXi(string cookie)
        {
            TiebaZhangHaoXinXiJieGou tiebaZhangHaoXinXiJieGou = new TiebaZhangHaoXinXiJieGou();

            string url = $"http://tieba.baidu.com/f/user/json_userinfo";
            //Console.WriteLine(url);

            string html = TiebaHttpHelper.Get(url, cookie);
            //Console.WriteLine(html);

            if (string.IsNullOrEmpty(html))
            {
                tiebaZhangHaoXinXiJieGou.Msg = "网络异常";
                return tiebaZhangHaoXinXiJieGou;
            }

            if (html == "null")
            {
                tiebaZhangHaoXinXiJieGou.Msg = "Cookie无效";
                return tiebaZhangHaoXinXiJieGou;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                tiebaZhangHaoXinXiJieGou.Msg = "Json解析失败";
                return tiebaZhangHaoXinXiJieGou;
            }

            tiebaZhangHaoXinXiJieGou.Msg = jObject["error"]?.ToString();
            if (jObject["no"]?.ToString() != "0")
            {
                return tiebaZhangHaoXinXiJieGou;
            }

            long.TryParse(jObject["data"]?["open_uid"]?.ToString(), out tiebaZhangHaoXinXiJieGou.Uid);
            tiebaZhangHaoXinXiJieGou.TouXiang = Tieba.GuoLvTouXiangID(jObject["data"]?["user_portrait"]?.ToString());
            tiebaZhangHaoXinXiJieGou.YongHuMing = jObject["data"]?["user_name_weak"]?.ToString();
            tiebaZhangHaoXinXiJieGou.NiCheng = jObject["data"]?["user_name_show"]?.ToString();
            tiebaZhangHaoXinXiJieGou.FuGaiMing = jObject["data"]?["show_nickname"]?.ToString();

            tiebaZhangHaoXinXiJieGou.HuoQuChengGong = true;
            return tiebaZhangHaoXinXiJieGou;
        }

        /// <summary>
        /// 获取吧务团队
        /// </summary>
        /// <param name="tiebaName"></param>
        /// <param name="quChongFu"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<BaWuJieGou> GetBaWuTuanDui(string tiebaName, out string msg)
        {
            List<BaWuJieGou> baWuTuanDuiLieBiao = new List<BaWuJieGou>();

            string html = TiebaHttpHelper.Get("http://tieba.baidu.com/bawu2/platform/listBawuTeamInfo?word=" + Http.UrlEncode(tiebaName));
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return baWuTuanDuiLieBiao;
            }

            const string wenBenTou = "<div class=\"bawu_team_wrap\">";
            const string wenBenWei = "<div id=\"footer\" class=\"footer\">";

            //将吧务团队列表的源码过滤出来
            string baWuTuanDuiHtml = BST.JieQuWenBen(html, wenBenTou, wenBenWei);
            baWuTuanDuiHtml = wenBenTou + baWuTuanDuiHtml + wenBenWei;
            //File.WriteAllText(@"C:\Users\bakas\Desktop\test1.html", baWuTuanDuiHtml);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(baWuTuanDuiHtml);

            HtmlNodeCollection bawu_team_wrap_list = doc.DocumentNode?.SelectNodes("div[1]/div");
            if (bawu_team_wrap_list == null)
            {
                msg = "Html解析失败";
                return baWuTuanDuiLieBiao;
            }

            foreach (var bawu_single_type in bawu_team_wrap_list)
            {
                //职务标题
                HtmlNode bawu_single_type_title = bawu_single_type?.SelectSingleNode("div[1]");
                if (bawu_single_type_title == null) continue;

                //职务
                string zhiWu = bawu_single_type_title.InnerText;

                //职务列表
                HtmlNodeCollection member_first_row_list = bawu_single_type?.SelectNodes("div[2]/span");
                if (member_first_row_list == null) continue;

                foreach (var member_first_row in member_first_row_list)
                {
                    //头像
                    string touXiang = member_first_row?.SelectSingleNode("a[@class='avatar']")?.SelectSingleNode("img")?.Attributes["src"]?.Value;

                    //过滤头像链接
                    if (touXiang.Contains("/"))
                    {
                        string[] touXiangList = touXiang.Split('/');
                        touXiang = touXiangList[touXiangList.Length - 1];
                    }

                    touXiang = Tieba.GuoLvTouXiangID(touXiang);

                    //用户名
                    string yongHuMing = member_first_row?.SelectSingleNode("a[@class='user_name']")?.Attributes["title"]?.Value;

                    baWuTuanDuiLieBiao.Add(new BaWuJieGou
                    {
                        ZhiWu = zhiWu,
                        YongHuMing = yongHuMing,
                        TouXiang = touXiang
                    });
                }
            }

            msg = "获取成功";
            return baWuTuanDuiLieBiao;
        }

        /// <summary>
        /// 获取贴吧关注列表
        /// </summary>
        /// <param name="yonghuming">用户名</param>
        /// <returns></returns>
        public static List<TiebaGuanZhuJieGou> GetTiebaGuanZhuLieBiao(string cookie, string yongHuMing, int page = 1)
        {
            List<TiebaGuanZhuJieGou> tiebaGuanZhuLieBiao = new List<TiebaGuanZhuJieGou>();

            long uid = GetTiebaMingPian(yongHuMing).Uid;
            //string url = "http://c.tieba.baidu.com/c/f/forum/like";
            //string postStr
            //    = cookie
            //    + "&_client_id=" + Tieba.GetAndroidStamp()
            //    + "&_client_type=2"
            //    + "&_client_version=9.9.8.32"
            //    + "&friend_uid=" + uid
            //    + "&uid=" + uid;

            string url = "http://c.tieba.baidu.com/c/f/forum/like";
            string postStr
                = cookie
                + "&_client_id=" + Tieba.GetAndroidStamp()
                + "&_client_type=2"
                + "&_client_version=9.9.8.32"
                + "&_phone_imei=450456461928196"
                + "&cuid=00DC23509DCDF63928D194FD8D41703A%7CVRO6PAJEL"
                + "&cuid_galaxy2=00DC23509DCDF63928D194FD8D41703A%7CVRO6PAJEL"
                + "&cuid_gid="
                + $"&friend_uid={uid}"
                + "&from=1019960r"
                + "&is_guest=1"
                + "&model=MI+6"
                + "&net_type=1"
                + "&oaid=%7B%22sc%22%3A0%2C%22sup%22%3A0%2C%22tl%22%3A0%7D"
                + "&page_no=" + page
                + "&page_size=50"
                + "&stErrorNums=1"
                + "&stMethod=1"
                + "&stMode=1"
                + "&stSize=5061"
                + "&stTime=667"
                + "&stTimesNum=1"
                + "&timestamp=1584512309167"
                + "&uid=0";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            string html = TiebaHttpHelper.Post(url, postStr, cookie);
            if (string.IsNullOrEmpty(html))
            {
                return tiebaGuanZhuLieBiao;
            }

            //解析
            JObject htmlJson;
            try
            {
                htmlJson = JObject.Parse(html);
            }
            catch
            {
                return tiebaGuanZhuLieBiao;
            }

            if (htmlJson["error_code"]?.ToString() != "0")
            {
                return tiebaGuanZhuLieBiao;
            }

            //{"server_time":"50714","time":1584513751,"ctime":0,"logid":2551816537,"error_code":"0"}

            if (!htmlJson.ContainsKey("forum_list") || htmlJson["forum_list"]?.Count() == 0)
            {
                return tiebaGuanZhuLieBiao;
            }

            var non_gconforum = htmlJson["forum_list"]?["non-gconforum"];
            foreach (var ng in non_gconforum)
            {
                TiebaGuanZhuJieGou guanZhuJieGou = new TiebaGuanZhuJieGou
                {
                    TiebaName = ng["name"]?.ToString()
                };
                long.TryParse(ng["id"]?.ToString(), out guanZhuJieGou.Fid);
                int.TryParse(ng["level_id"]?.ToString(), out guanZhuJieGou.DengJi);
                int.TryParse(ng["cur_score"]?.ToString(), out guanZhuJieGou.JingYanZhi);

                tiebaGuanZhuLieBiao.Add(guanZhuJieGou);
            }

            return tiebaGuanZhuLieBiao;
        }

        /// <summary>
        /// 获取用户贴吧等级
        /// </summary>
        /// <param name="yongHuMing">用户名</param>
        /// <param name="tiebaName">贴吧名</param>
        /// <returns></returns>
        public static int GetYongHuTiebaDengJi(string cookie, string yongHuMing, string tiebaName)
        {
            List<TiebaGuanZhuJieGou> liebiao = GetTiebaGuanZhuLieBiao(cookie, yongHuMing);

            List<TiebaGuanZhuJieGou> jieGuo = liebiao.Where(x => (x.TiebaName == tiebaName)).ToList();
            if (jieGuo.Count > 0)
            {
                return jieGuo[0].DengJi;
            }

            return -1;
        }

        /// <summary>
        /// 获取贴吧名片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TiebaMingPianJieGou GetTiebaMingPian(string id)
        {
            //Console.WriteLine(id);

            TiebaMingPianJieGou mingPianJieGou = new TiebaMingPianJieGou();

            string canShu = $"&un={Http.UrlEncodeUtf8(id)}";
            if (id.Length >= 20)
            {
                canShu = $"&id={id}";
            }

            string url = $"https://tieba.baidu.com/home/get/panel?ie=utf-8{canShu}";
            //Console.WriteLine(url);

            string html = TiebaHttpHelper.Get(url);
            //Console.WriteLine(html);

            if (string.IsNullOrEmpty(html))
            {
                mingPianJieGou.Msg = "网络异常";
                return mingPianJieGou;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                mingPianJieGou.Msg = "Json解析失败";
                return mingPianJieGou;
            }

            mingPianJieGou.Msg = jObject["error"]?.ToString();
            if (jObject["no"]?.ToString() != "0")
            {
                return mingPianJieGou;
            }

            long.TryParse(jObject["data"]?["id"]?.ToString(), out mingPianJieGou.Uid);
            mingPianJieGou.TouXiang = Tieba.GuoLvTouXiangID(jObject["data"]?["portrait"]?.ToString());
            mingPianJieGou.YongHuMing = jObject["data"]?["name"]?.ToString();
            mingPianJieGou.NiCheng = jObject["data"]?["name_show"]?.ToString();
            mingPianJieGou.FuGaiMing = jObject["data"]?["show_nickname"]?.ToString();

            mingPianJieGou.HuoQuChengGong = true;
            return mingPianJieGou;
        }

        /// <summary>
        /// 删自己的帖子
        /// </summary>
        /// <param name="cookie">Cookie</param>
        /// <param name="tiebaName">贴吧名</param>
        /// <param name="fid">Fid</param>
        /// <param name="tid">Tid</param>
        /// <param name="pid">Pid</param>
        /// <param name="isLzl">是否楼中楼</param>
        /// <returns></returns>
        public static bool ShanTie(string cookie, string tiebaName, long fid, long tid, long pid, out string msg)
        {
            //自动获取Fid
            if (fid < 1)
            {
                fid = GetTiebaFid(tiebaName);
            }

            const string url = "http://tieba.baidu.com/f/commit/post/delete";
            string postData = $"ie=utf-8&tbs={GetBaiduTbs(cookie)}&kw={Http.UrlEncodeUtf8(tiebaName)}&fid={fid}&tid={tid}&user_name=&delete_my_post=1&delete_my_thread=0&is_vipdel=0&pid={pid}&is_finf=false"; //&is_finf={(isLzl ? "1" : "false")}

            //到达上限
            //{"no":34,"err_code":220034,"error":null,"data":{"reason":30}}

            //没有权限
            //{"no":308,"err_code":230308,"error":"\u7528\u6237\u6ca1\u6709\u6743\u9650","data":null}

            //需要验证码
            //{"no":4011,"err_code":224011,"error":null,"data":{"autoMsg":"","fid":2432903,"fname":"minecraft","tid":1388096876,"is_login":1,"content":"","access_state":null,"vcode":{"need_vcode":1,"str_reason":"\u8bf7\u70b9\u51fb\u9a8c\u8bc1\u7801\u5b8c\u6210\u53d1\u8d34","captcha_vcode_str":"captchaservice37363031574342557a386237516e57355348546553535a42385065716c5a387644754d764b644676314a5348526b337330386747654a58593238664d69527461594f6a623249465138796569666a4e5a544c67645647626c4759432b656257526a466572384a2f356644396c51344c58386c4d59476261544e415538564e3354524d4a7a6f764c2b574c764b51376975456d775242317a37467a7942754569657649366167544f786c753342304664514369674f37726765546d32762f6c58474d6d646154496b645159684a6141396e64526b66476d575959554c554d5a566d534a387133433871617769557a764b30726865666a6e595a555165534473484a76316d6d55472f7a73672f674c35354a774b5269457a48312b6551643172377a582f6148585a4c54437954436e6d314c6141","captcha_code_type":4,"userstatevcode":0},"second_class_id":""}}

            string html = TiebaHttpHelper.Post(url, postData, cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return false;
            }

            //解析
            JObject json;
            try
            {
                json = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //访问失败
            string err_code = json["err_code"]?.ToString();
            if (err_code == "224011")
            {
                //需要验证码(错误代码)|验证类型|验证代码
                msg = $"需要验证码({err_code})|{json["data"]?["vcode"]?["captcha_code_type"]?.ToString()}|{json["data"]?["vcode"]?["captcha_vcode_str"]?.ToString()}";
                return false;
            }

            switch (err_code)
            {
                case "0":
                    msg = "删帖成功";
                    return true;

                case "220034":
                    msg = $"删帖数量已到达上限({err_code})";
                    return false;

                case "230308":
                    msg = $"权限不足({err_code})";
                    return false;

                //case "224011":
                //    msg = $"需要验证码({err_code})";
                //    return false;

                default:
                    msg = $"未知错误({err_code})";
                    return false;
            }
        }

        /// <summary>
        /// 删帖验证码
        /// </summary>
        /// <param name="cookie">Cookie</param>
        /// <param name="captcha_vcode_str">验证代码</param>
        /// <param name="captcha_code_type">验证类型</param>
        /// <param name="captcha_input_str">验证结果</param>
        /// <param name="fid">Fid</param>
        /// <returns></returns>
        public static bool ShanTieYanZhengMa(string cookie, string captcha_vcode_str, string captcha_code_type, string captcha_input_str, long fid)
        {
            string url = "http://tieba.baidu.com/f/commit/commonapi/checkVcode";
            string postData = $"captcha_vcode_str={captcha_vcode_str}&captcha_code_type={captcha_code_type}&captcha_input_str={captcha_input_str}&fid={fid}";
            string html = TiebaHttpHelper.Post(url, postData, cookie);

            //{"anti_valve_err_no":40}
            string code = BST.JieQuWenBen(html, "{\"anti_valve_err_no\":", "}");
            if (code == "0")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 客户端签到
        /// </summary>
        /// <param name="cookie">Cookie</param>
        /// <param name="tiebaName">贴吧名</param>
        /// <param name="fid">Fid</param>
        /// <param name="msg">签到结果</param>
        /// <returns></returns>
        public static bool KeHuDuanQianDao(string cookie, string tiebaName, string fid, out string msg)
        {
            string url = "http://c.tieba.baidu.com/c/c/forum/sign";
            string postStr
                = cookie
                + "&_client_id=" + Tieba.GetAndroidStamp()
                + "&_client_type=2"
                + "&_client_version=9.9.8.32"
                + $"&fid={fid}"
                + $"&kw={Http.UrlEncodeUtf8(tiebaName)}"
                + "&net_type=3"
                + $"&tbs={GetBaiduTbs(cookie)}";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            string html = TiebaHttpHelper.Post(url, postStr, cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return false;
            }

            //解析
            JObject htmlJson;
            try
            {
                htmlJson = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            msg = BST.DeUnicode(Convert.ToString(htmlJson["error_msg"]));
            string error_code = Convert.ToString(htmlJson["error_code"]);
            if (error_code != "0")
            {
                msg += $"({error_code})";
                return false;
            }
            else
            {
                msg = "签到成功";
            }

            return true;
        }

        /// <summary>
        /// 获取我的帖子
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="isZhuTi"></param>
        /// <param name="uid"></param>
        /// <param name="pn"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<TiebaWoDeTieZiJieGou> GetWoDeTieZi(string cookie, bool isZhuTi, long uid, int pn, out string msg)
        {
            List<TiebaWoDeTieZiJieGou> lieBiao = new List<TiebaWoDeTieZiJieGou>();

            string url = "http://c.tieba.baidu.com/c/u/feed/userpost";
            string postStr
                = cookie
                + "&_client_id=" + Tieba.GetAndroidStamp()
                + "&_client_type=2"
                + "&_client_version=12.5.1.0"
                //+ $"&_phone_imei=000000000000000"
                //+ $"&c3_aid=A00-KVSOM4UW7FB7WIHJH5UVTKTFOD2QOTER-SC27XGGW"
                //+ $"&cuid=34DDE2B2B66E6C36D513959AA37AEAC1%7CO"
                //+ $"&cuid_galaxy2=34DDE2B2B66E6C36D513959AA37AEAC1%7CO"
                //+ $"&cuid_gid="
                //+ $"&flutter_net=1"
                //+ $"&framework_ver=3290033"
                //+ $"&from=1024328t"
                + $"&is_thread={(isZhuTi ? 1 : 0)}"
                + $"&is_view_card=1"
                //+ $"&model=Redmi+K20+Pro"
                + $"&need_content=1"
                + $"&net_type=1"
                + $"&pn={pn}"
                //+ $"&q_type=80"
                + $"&rn=50"
                //+ $"&scr_dip=2.64"
                //+ $"&scr_h=886.3636363636364"
                //+ $"&scr_w=409.09090909090907"
                //+ $"&sdk_ver=2.29.0"
                //+ $"&stErrorNums=0"
                //+ $"&stoken=f4ae22f91c43ac787c315ca746d9656cbab80f0bf8d8b487460a3cf825f297f6"
                //+ $"&swan_game_ver=1033000"
                + $"&tbs={GetBaiduTbs(cookie)}"
                //+ $"&timestamp=1621483249220"
                + $"&uid={uid}";
            //+ $"&z_id=afOHNtWGDdgwVx3MWR41zc2E9IOQgI-2LioQkYTw6lO8qo9FueU0ibDW0I39q8zToGzDWQ_uWbmIKo-UoVOpyRg";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            string html = TiebaHttpHelper.Post(url, postStr, cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return lieBiao;
            }

            //Console.WriteLine(html);

            //解析
            JObject htmlJson;
            try
            {
                htmlJson = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return lieBiao;
            }

            msg = BST.DeUnicode(Convert.ToString(htmlJson["error_msg"]));
            string error_code = Convert.ToString(htmlJson["error_code"]);
            if (error_code != "0")
            {
                msg += $"({error_code})";
                return lieBiao;
            }

            var post_list = htmlJson["post_list"];

            foreach (var post in post_list)
            {
                lieBiao.Add(new TiebaWoDeTieZiJieGou
                {
                    IsZhuTi = post["is_thread"]?.ToString() == "1",
                    TiebaName = post["forum_name"]?.ToString(),
                    Fid = Convert.ToInt64(post["forum_id"]?.ToString()),
                    Tid = Convert.ToInt64(post["thread_id"]?.ToString()),
                    Pid = Convert.ToInt64(post["post_id"]?.ToString()),
                    BiaoTi = post["title"]?.ToString(),
                    Uid = Convert.ToInt64(post["user_id"]?.ToString()),
                    NiCheng = post["name_show"]?.ToString(),
                    TouXiang = Tieba.GuoLvTouXiangID(post["user_portrait"]?.ToString()),
                    FaTieShiJian = BST.ShiJianChuoDaoShiJian(Convert.ToInt64(post["create_time"]?.ToString()) * 1000),
                    FaTieShiJianChuo = Convert.ToInt64(post["create_time"]?.ToString())
                });
            }

            msg = "获取成功";
            return lieBiao;
        }

        /// <summary>
        /// 客户端签到
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="tiebaName"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool KeHuDuanQianDao(string cookie, string tiebaName, out string msg)
        {
            string url = "http://c.tieba.baidu.com/c/c/forum/sign";
            string postStr
                = cookie
                + "&_client_id=" + Tieba.GetAndroidStamp()
                + "&_client_type=2"
                + "&_client_version=5.3.1"
                //+ "&_phone_imei=042791438690445"
                //+ "&cuid=DCE2BCBBC5F4307F7457E2463A14F382%7C544096834197240"
                //+ "&from=baidu_appstore"
                + "&kw=" + Http.UrlEncodeUtf8(tiebaName)
                //+ "&model=GT-I9100"
                //+ "&stErrorNums=0"
                //+ "&stMethod=1"
                //+ "&stMode=1"
                //+ "&stSize=64923"
                //+ "&stTime=780"
                //+ "&stTimesNum=0"
                + "&tbs=" + GetBaiduTbs(cookie);
            //+ "&timestamp=1388306178920";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            string html = TiebaHttpHelper.Post(url, postStr, cookie);

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return false;
            }

            //解析
            JObject huiFuJsonData;
            try
            {
                huiFuJsonData = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //访问失败
            msg = huiFuJsonData["error_msg"]?.ToString();
            if (huiFuJsonData["error_code"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 一键签到
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool YiJianQianDao(string cookie, out string msg)
        {
            string url = "http://c.tieba.baidu.com/c/c/forum/msign";
            string postStr
                = cookie
                + "&_client_id=" + Tieba.GetAndroidStamp()
                + "&_client_type=2"
                + "&_client_version=5.3.1"
                //+ "&_phone_imei=042791438690445"
                //+ "&cuid=DCE2BCBBC5F4307F7457E2463A14F382%7C544096834197240"
                //+ "&forum_ids=1938496%2C1929829"
                //+ "&from=baidu_appstore"
                //+ "&model=GT-I9100"
                //+ "&stErrorNums=0"
                //+ "&stMethod=1"
                //+ "&stMode=1"
                //+ "&stSize=138"
                //+ "&stTime=120"
                //+ "&stTimesNum=0"
                + "&tbs=" + GetBaiduTbs(cookie);
            //+ "&timestamp=1388304097180"
            //+ "&user_id=16303";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            string html = TiebaHttpHelper.Post(url, postStr, cookie);

            Console.WriteLine(BST.DeUnicode(html));

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return false;
            }

            //解析
            JObject huiFuJsonData;
            try
            {
                huiFuJsonData = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //访问失败
            msg = huiFuJsonData["error_msg"]?.ToString();
            if (huiFuJsonData["error_code"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }
    }
}

using BakaSnowTool;
using BakaSnowTool.Http;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

            string url = $"https://tieba.baidu.com/f/user/json_userinfo";
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
            tiebaZhangHaoXinXiJieGou.TouXiangID = Tieba.GuoLvTouXiangID(jObject["data"]?["user_portrait"]?.ToString());
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
                        TouXiangID = touXiang
                    });
                }
            }

            msg = "获取成功";
            return baWuTuanDuiLieBiao;
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
            Console.WriteLine(url);

            string html = TiebaHttpHelper.Get(url);
            Console.WriteLine(html);

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
            mingPianJieGou.TouXiangID = Tieba.GuoLvTouXiangID(jObject["data"]?["portrait"]?.ToString());
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

        
    }
}

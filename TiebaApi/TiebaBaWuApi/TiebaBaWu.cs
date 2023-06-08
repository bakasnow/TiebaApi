using BakaSnowTool;
using BakaSnowTool.Http;
using Newtonsoft.Json.Linq;
using System;
using TiebaApi.TiebaCanShu;
using TiebaApi.TiebaForms;
using TiebaApi.TiebaLeiXing;
using TiebaApi.TiebaTools;
using TiebaApi.TiebaWebApi;

namespace TiebaApi.TiebaBaWuApi
{
    public static class TiebaBaWu
    {
        /// <summary>
        /// 网页端删主题
        /// </summary>
        /// <param name="canShu"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool ShanZhuTi(TiebaShanZhuTiCanShu canShu, out string msg)
        {
            string url = "https://tieba.baidu.com/f/commit/thread/delete";
            string postStr =
                $"commit_fr=pb" +
                $"&ie=utf-8" +
                $"&tbs={TiebaWeb.GetBaiduTbs(canShu.Cookie)}" +
                $"&kw={Http.UrlEncodeUtf8(canShu.TiebaName)}" +
                $"&fid={canShu.Fid}" +
                $"&tid={canShu.Tid}" +
                $"&is_frs_mask=0" +
                $"&reason=5" +
                $"{(string.IsNullOrEmpty(canShu.VCode) ? string.Empty : $"&vcode={canShu.VCode}")}" +
                $"{(string.IsNullOrEmpty(canShu.VCode_md5) ? string.Empty : $"&vcode_md5={canShu.VCode_md5}")}";

            string html = TiebaHttpHelper.Post(url, postStr, canShu.Cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络错误";
                return false;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //访问失败
            string err_code = jObject["err_code"]?.ToString();
            if (err_code == "224011")
            {
                string captcha_code_type = jObject["data"]?["vcode"]?["captcha_code_type"]?.ToString();
                string captcha_vcode_str = jObject["data"]?["vcode"]?["captcha_vcode_str"]?.ToString();

                //需要验证码
                TiebaHanZiYanZhengMaForm hanZiYanZhengMaForm = new TiebaHanZiYanZhengMaForm
                {
                    captcha_vcode_str = captcha_vcode_str
                };
                hanZiYanZhengMaForm.ShowDialog();

                string captcha_input_str = hanZiYanZhengMaForm.captcha_input_str;

                if (TiebaWeb.ShanTieYanZhengMa(canShu.Cookie, captcha_vcode_str, captcha_code_type, captcha_input_str, canShu.Fid))
                {
                    canShu.VCode = captcha_input_str;
                    canShu.VCode_md5 = captcha_vcode_str;

                    if (ShanZhuTi(canShu, out string msg2))
                    {
                        msg = "删帖成功：验证码正确";
                        return true;
                    }
                    else
                    {
                        msg = msg2;
                    }
                }
                else
                {
                    msg = "删帖失败：验证码错误";
                }

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
        /// 网页端删回复
        /// </summary>
        /// <param name="canShu"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool ShanHuiFu(TiebaShanHuiFuCanShu canShu, out string msg)
        {
            string url = "http://tieba.baidu.com/f/commit/post/delete";
            string postData = $"commit_fr=pb&ie=utf-8" +
                $"&tbs={TiebaWeb.GetBaiduTbs(canShu.Cookie)}" +
                $"&kw={Http.UrlEncodeUtf8(canShu.TiebaName)}" +
                $"&fid={canShu.Fid}" +
                $"&tid={canShu.Tid}" +
                $"&is_vipdel=0" +
                $"&pid={canShu.Pid}" +
                $"&is_finf={(canShu.IsFinf ? "1" : "false")}" +
                $"{(string.IsNullOrEmpty(canShu.VCode) ? string.Empty : $"&vcode={canShu.VCode}")}" +
                $"{(string.IsNullOrEmpty(canShu.VCode_md5) ? string.Empty : $"&vcode_md5={canShu.VCode_md5}")}";

            string html = TiebaHttpHelper.Post(url, postData, canShu.Cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络错误";
                return false;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //{"no":0,"err_code":0,"error":null,"data":{"mute_text":null}}

            //访问失败
            string err_code = jObject["err_code"]?.ToString();
            if (err_code == "224011")
            {
                string captcha_code_type = jObject["data"]?["vcode"]?["captcha_code_type"]?.ToString();
                string captcha_vcode_str = jObject["data"]?["vcode"]?["captcha_vcode_str"]?.ToString();

                //需要验证码
                TiebaHanZiYanZhengMaForm hanZiYanZhengMaForm = new TiebaHanZiYanZhengMaForm
                {
                    captcha_vcode_str = captcha_vcode_str
                };
                hanZiYanZhengMaForm.ShowDialog();

                string captcha_input_str = hanZiYanZhengMaForm.captcha_input_str;

                if (TiebaWeb.ShanTieYanZhengMa(canShu.Cookie, captcha_vcode_str, captcha_code_type, captcha_input_str, canShu.Fid))
                {
                    canShu.VCode = captcha_input_str;
                    canShu.VCode_md5 = captcha_vcode_str;

                    if (ShanHuiFu(canShu, out string msg2))
                    {
                        msg = "删帖成功：验证码正确";
                        return true;
                    }
                    else
                    {
                        msg = msg2;
                    }
                }
                else
                {
                    msg = "删帖失败：验证码错误";
                }

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
        /// 操作量查询
        /// </summary>
        /// <param name="canShu"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int GetCaoZuoLiang(GetTieZiGuanLiRiZhiCanShu canShu, out string msg)
        {
            long kaiShiShiJianChuo = BST.QuShiJianChuo($"{canShu.KaiShiRiQi:yyyy-MM-dd} 00:00:00", "1970-01-01 08:00:00");
            long jieShuShiJianChuo = BST.QuShiJianChuo($"{canShu.JieShuRiQi:yyyy-MM-dd} 23:59:59", "1970-01-01 08:00:00");

            string url = $"https://tieba.baidu.com/bawu2/platform/listPostLog?" +
                $"stype=op_uname" +
                $"&svalue={Http.UrlEncode(canShu.YongHuMing)}" +
                $"&begin={kaiShiShiJianChuo}" +
                $"&end={jieShuShiJianChuo}" +
                $"&op_type={(canShu.CaoZuoLeiXing == TieZiGuanLiRiZhiCaoZuoLeiXing.QuanBuCaoZuo ? "" : $"{(int)canShu.CaoZuoLeiXing}")}" +
                $"&word={Http.UrlEncode(canShu.TiebaName)}" +
                $"&pn={canShu.Pn}";

            string html = TiebaHttpHelper.Get(url, canShu.Cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "Html获取失败";
                return -1;
            }

            if (int.TryParse(BST.JianYiZhengZe(html, "<div class=\"breadcrumbs\">共<em>([0-9]*)</em>条记录</div>"), out int caoZuoLiang))
            {
                msg = "获取成功";
                return caoZuoLiang;
            }
            else
            {
                msg = "获取失败";
                return -1;
            }
        }

        /// <summary>
        /// 网页端封禁
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="fid"></param>
        /// <param name="yongHuMing"></param>
        /// <param name="niCheng"></param>
        /// <param name="touXiangID"></param>
        /// <param name="day"></param>
        /// <param name="liYou"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool FengJin(string cookie, long fid, string yongHuMing, string niCheng, string touXiangID, int day, string liYou, out string msg)
        {
            string url = "http://tieba.baidu.com/pmc/blockid";
            string postStr = $"day={day}&fid={fid}&tbs={TiebaWeb.GetBaiduTbs(cookie)}&ie=gbk&user_name%5B%5D={Http.UrlEncodeUtf8(yongHuMing)}&nick_name%5B%5D={Http.UrlEncodeUtf8(niCheng)}&portrait%5B%5D={touXiangID}&reason={Http.UrlEncodeUtf8(liYou)}";
            string html = TiebaHttpHelper.Post(url, postStr, cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return false;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //{"errno":0,"errmsg":"成功"}
            //{"errno":110002,"errmsg":"请使用POST方式提交"}

            msg = BST.DeUnicode(jObject["errmsg"]?.ToString());
            if (jObject["errno"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 解封
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="tiebaName"></param>
        /// <param name="yongHuMing"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool JieFeng(string cookie, string tiebaName, string yongHuMing, out string msg)
        {
            string url = "http://tieba.baidu.com/bawu2/platform/cancelFilter";
            string postStr = $"word={Http.UrlEncodeUtf8(tiebaName)}&tbs={TiebaWeb.GetBaiduTbs(cookie)}&ie=utf-8&type=0&list%5B0%5D%5Buser_id%5D={TiebaWeb.GetTiebaMingPian(yongHuMing).Uid}&list%5B0%5D%5Buser_name%5D={Http.UrlEncodeUtf8(yongHuMing)}";
            string html = TiebaHttpHelper.Post(url, postStr, cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return false;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //{"cupid":{"83":"\u53f3\u4fa7\u8d44\u6e90\u533a\u5347\u7ea7","208":"\u8fde\u7eed\u7b7e\u5230\u6a59\u540d\u5c0f\u6d41\u91cf","210":"\u8fde\u7eed\u7b7e\u5230\u6a59\u540d\u5c0f\u6d41\u91cf\u4e4bfrsui","288":"\u8db3\u5f69\u76ee\u5f55\u767d\u540d\u5355","289":"\u8d34\u5427\u5f69\u7968\u767d\u540d\u5355","310":"\u53cc\u8272\u7403","318":"\u667a\u80fd\u7248\u6e38\u620f","1387960368":"\u5e7f\u544a\u4efb\u52a1","339":"\u5e7f\u544a\u6295\u653e\u7684\u5427","340":"\u5e7f\u544a\u6295\u653e\u7684\u4e00\u7ea7\u76ee\u5f55","341":"\u5e7f\u544a\u6295\u653e\u7684\u4e8c\u7ea7\u76ee\u5f55","342":"PS\u56de\u6d41\u5c0f\u6d41\u91cf\u5427","349":"pb\u63a8\u8350\u5185\u5bb9\u4e00\u7ea7\u76ee\u5f55\u5c0f\u6d41\u661f"},"errno":0,"errmsg":"success"}

            msg = BST.DeUnicode(jObject["errmsg"]?.ToString());
            if (jObject["errno"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 操作量查询
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="tiebaName"></param>
        /// <param name="yongHuMing"></param>
        /// <param name="kaiShiShiJian"></param>
        /// <param name="jieShuShiJian"></param>
        /// <returns></returns>
        public static int GetCaoZuoLiang(string cookie, string tiebaName, string yongHuMing, string kaiShiShiJian, string jieShuShiJian)
        {
            string url = $"http://tieba.baidu.com/bawu2/platform/listPostLog?stype=op_uname&svalue={Http.UrlEncode(yongHuMing)}&begin={BST.QuShiJianChuo(kaiShiShiJian, "1970/01/01 08:00:00")}&end={BST.QuShiJianChuo(jieShuShiJian, "1970/01/01 08:00:00")}&op_type=&word={Http.UrlEncode(tiebaName)}&pn=1";
            string html = TiebaHttpHelper.Get(url, cookie);
            if (string.IsNullOrEmpty(html))
            {
                return -1;
            }

            if (int.TryParse(BST.JieQuWenBen(html, "<div class=\"breadcrumbs\">共<em>", "</em>条记录</div>"), out int caoZuoLiang))
            {
                return caoZuoLiang;
            }

            return -1;
        }

        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="yongHuMing"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool JiaRuHeiMingDan(string cookie, string yongHuMing, out string msg)
        {
            long uid = TiebaWeb.GetTiebaMingPian(yongHuMing).Uid;
            if (uid <= 0)
            {
                msg = "Uid获取失败";
                return false;
            }

            return JiaRuHeiMingDan(cookie, yongHuMing, uid, out msg);
        }

        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="tiebaName"></param>
        /// <param name="uid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool JiaRuHeiMingDan(string cookie, string tiebaName, long uid, out string msg)
        {
            string url = "http://tieba.baidu.com/bawu2/platform/addBlack";
            string postData = $"tbs={TiebaWeb.GetBaiduTbs(cookie)}&user_id={uid}&word={Http.UrlEncodeUtf8(tiebaName)}&ie=utf-8";
            string html = TiebaHttpHelper.Post(url, postData, cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络错误";
                return false;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            msg = jObject["errmsg"]?.ToString();
            if (jObject["errno"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 移除黑名单
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="tiebaName"></param>
        /// <param name="yongHuMing"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool YiChuHeiMingDan(string cookie, string tiebaName, string yongHuMing, out string msg)
        {
            long uid = TiebaWeb.GetTiebaMingPian(yongHuMing).Uid;
            if (uid <= 0)
            {
                msg = "Uid获取失败";
                return false;
            }

            return YiChuHeiMingDan(cookie, tiebaName, uid, out msg);
        }

        /// <summary>
        /// 移除黑名单
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="tiebaName"></param>
        /// <param name="uid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool YiChuHeiMingDan(string cookie, string tiebaName, long uid, out string msg)
        {
            string url = "http://tieba.baidu.com/bawu2/platform/cancelBlack";
            string postData = $"word={Http.UrlEncodeUtf8(tiebaName)}&tbs={TiebaWeb.GetBaiduTbs(cookie)}&list%5B%5D={uid}&ie=utf-8";
            string html = TiebaHttpHelper.Post(url, postData, cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络错误";
                return false;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            msg = jObject["errmsg"]?.ToString();
            if (jObject["errno"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }

        ///// <summary>
        ///// 客户端封禁
        ///// </summary>
        ///// <param name="day">天数</param>
        ///// <param name="liYou">理由</param>
        ///// <param name="msg">消息</param>
        ///// <returns></returns>
        //public bool FengJin2(int day, string liyou, out string msg)
        //{
        //    string url = "http://c.tieba.baidu.com/c/c/bawu/commitprison";
        //    string postData
        //        = Cookie
        //        + "&_client_id=" + Tieba.GetAndroidStamp()
        //        + "&_client_type=2"
        //        + "&_client_version=9.9.8.32"
        //        + "&day=" + day.ToString()
        //        + "&fid=" + Fid
        //        + "&ntn=banid"
        //        + "&reason=" + Http.UrlEncodeUtf8(liyou)
        //        + "&tbs=" + Tieba.GetBaiduTbs(Cookie)
        //        + "&un=" + Http.UrlEncodeUtf8(YongHuMing)
        //        + "&word=" + Http.UrlEncodeUtf8(TiebaName)
        //        + "&z=5908352401";

        //    postData += "&sign=" + Tieba.GetTiebaSign(postData);

        //    string html = Http.Post(url, postData, Cookie);
        //    string code = BST.JieQuWenBen(html, "\"error_code\":\"", "\"");
        //    msg = BST.DeUnicode(BST.JieQuWenBen(html, "\"error_msg\":\"", "\""));
        //    if (code == "0")
        //        return true;
        //    else
        //        return false;
        //}
    }
}

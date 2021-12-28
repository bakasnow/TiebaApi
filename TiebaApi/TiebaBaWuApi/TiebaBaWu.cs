using BakaSnowTool;
using BakaSnowTool.Http;
using Newtonsoft.Json.Linq;
using TiebaApi.TiebaForms;
using TiebaApi.TiebaTools;
using TiebaApi.TiebaWebApi;

namespace TiebaApi.TiebaBaWuApi
{
    public class TiebaBaWu
    {
        public string Cookie = string.Empty;
        public long Uid = 0;
        public string TiebaName = string.Empty;
        public string YongHuMing = string.Empty;
        public string NiCheng = string.Empty;
        public string TouXiang = string.Empty;
        public long Fid = 0;
        public long Tid = 0;
        public long Pid = 0;

        /// <summary>
        /// 网页端删主题
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public bool ShanZhuTi(out string msg, string vcode = "", string vcode_md5 = "")
        {
            string url = "http://tieba.baidu.com/f/commit/thread/delete";
            string postStr = $"commit_fr=pb" +
                $"&fid={Fid}" +
                $"&ie=utf-8" +
                $"&kw={Http.UrlEncodeUtf8(TiebaName)}" +
                $"&tbs={TiebaWeb.GetBaiduTbs(Cookie)}" +
                $"&tid={Tid}" +
                $"{(string.IsNullOrEmpty(vcode) ? string.Empty : $"&vcode={vcode}")}" +
                $"{(string.IsNullOrEmpty(vcode_md5) ? string.Empty : $"&vcode_md5={vcode_md5}")}";

            string html = TiebaHttpHelper.Post(url, postStr, Cookie);
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
            //{"no":4009,"err_code":224009,"error":null,"data":null}

            msg = jObject["err_code"]?.ToString();
            if (jObject["no"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 网页端删回复
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="isFinf">是否楼中楼</param>
        /// <returns></returns>
        public bool ShanHuiFu(out string msg, bool isFinf = false, string vcode = "", string vcode_md5 = "")
        {
            string url = "http://tieba.baidu.com/f/commit/post/delete";
            string postData = $"commit_fr=pb&ie=utf-8" +
                $"&tbs={TiebaWeb.GetBaiduTbs(Cookie)}" +
                $"&kw={Http.UrlEncodeUtf8(TiebaName)}" +
                $"&fid={Fid}" +
                $"&tid={Tid}" +
                $"&is_vipdel=0" +
                $"&pid={Pid}" +
                $"&is_finf={(isFinf ? "1" : "false")}" +
                $"{(string.IsNullOrEmpty(vcode) ? string.Empty : $"&vcode={vcode}")}" +
                $"{(string.IsNullOrEmpty(vcode_md5) ? string.Empty : $"&vcode_md5={vcode_md5}")}";

            string html = TiebaHttpHelper.Post(url, postData, Cookie);
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

                if (TiebaWeb.ShanTieYanZhengMa(Cookie, captcha_vcode_str, captcha_code_type, captcha_input_str, Fid))
                {
                    if (ShanHuiFu(out string msg2, isFinf, captcha_input_str, captcha_vcode_str))
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
        /// <param name="caoZuoRen">操作人</param>
        /// <param name="kaiShiRiQi">开始日期</param>
        /// <param name="jieShuRiQi">结束日期</param>
        /// <returns></returns>
        public int CaoZuoLiangChaXun(string caoZuoRen, string kaiShiRiQi, string jieShuRiQi)
        {
            long kaiShiShiJianChuo = BST.QuShiJianChuo(kaiShiRiQi + " 00:00:00", "1970-01-01 08:00:00");
            long jieShuShiJianChuo = BST.QuShiJianChuo(jieShuRiQi + " 23:59:59", "1970-01-01 08:00:00");

            string url = $"http://tieba.baidu.com/bawu2/platform/listPostLog?word={Http.UrlEncode(TiebaName)}&op_type=&stype=op_uname&svalue={Http.UrlEncode(caoZuoRen)}&date_type=on&startTime={kaiShiRiQi}&begin={kaiShiShiJianChuo}&endTime={jieShuRiQi}&end={jieShuShiJianChuo}";
            string html = TiebaHttpHelper.Get(url, Cookie);
            if (string.IsNullOrEmpty(html))
            {
                return -1;
            }

            if (int.TryParse(BST.JianYiZhengZe(html, "<div class=\"breadcrumbs\">共<em>([0-9]*)</em>条记录</div>"), out int caoZuoLiang))
            {
                return caoZuoLiang;
            }
            else
            {
                return -1;
            }

        }

        /// <summary>
        /// 网页端封禁
        /// </summary>
        /// <param name="day">天数</param>
        /// <param name="liYou">理由</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public bool FengJin(int day, string liYou, out string msg)
        {
            string url = "http://tieba.baidu.com/pmc/blockid";
            string postStr = $"day={day}&fid={Fid}&tbs={TiebaWeb.GetBaiduTbs(Cookie)}&ie=gbk&user_name%5B%5D={Http.UrlEncodeUtf8(YongHuMing)}&nick_name%5B%5D={Http.UrlEncodeUtf8(NiCheng)}&portrait%5B%5D={TouXiang}&reason={Http.UrlEncodeUtf8(liYou)}";
            string html = TiebaHttpHelper.Post(url, postStr, Cookie);
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
        /// <param name="msg">返回信息</param>
        /// <returns></returns>
        public bool JieFeng(out string msg)
        {
            string url = "http://tieba.baidu.com/bawu2/platform/cancelFilter";
            string postStr = $"word={Http.UrlEncodeUtf8(TiebaName)}&tbs={TiebaWeb.GetBaiduTbs(Cookie)}&ie=utf-8&type=0&list%5B0%5D%5Buser_id%5D={TiebaWeb.GetTiebaMingPian(YongHuMing).Uid}&list%5B0%5D%5Buser_name%5D={Http.UrlEncodeUtf8(YongHuMing)}";
            string html = TiebaHttpHelper.Post(url, postStr, Cookie);
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
        /// <param name="kaiShiShiJian">开始时间</param>
        /// <param name="jieShuShiJian">结束时间</param>
        /// <returns></returns>
        public int GetCaoZuoLiang(string kaiShiShiJian, string jieShuShiJian)
        {
            string url = $"http://tieba.baidu.com/bawu2/platform/listPostLog?stype=op_uname&svalue={Http.UrlEncode(YongHuMing)}&begin={BST.QuShiJianChuo(kaiShiShiJian, "1970/01/01 08:00:00")}&end={BST.QuShiJianChuo(jieShuShiJian, "1970/01/01 08:00:00")}&op_type=&word={Http.UrlEncode(TiebaName)}&pn=1";
            string html = TiebaHttpHelper.Get(url, Cookie);
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
        /// <returns></returns>
        public bool JiaRuHeiMingDan(out string msg)
        {
            long uid = TiebaWeb.GetTiebaMingPian(YongHuMing).Uid;
            if (uid <= 0)
            {
                msg = "Uid获取失败";
                return false;
            }

            return JiaRuHeiMingDan(uid, out msg);
        }

        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool JiaRuHeiMingDan(long uid, out string msg)
        {
            string url = "http://tieba.baidu.com/bawu2/platform/addBlack";
            string postData = $"tbs={TiebaWeb.GetBaiduTbs(Cookie)}&user_id={uid}&word={Http.UrlEncodeUtf8(TiebaName)}&ie=utf-8";
            string html = TiebaHttpHelper.Post(url, postData, Cookie);
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
        /// <param name="msg"></param>
        /// <param name="yongHuMing"></param>
        /// <returns></returns>
        public bool YiChuHeiMingDan(out string msg)
        {
            long uid = TiebaWeb.GetTiebaMingPian(YongHuMing).Uid;
            if (uid <= 0)
            {
                msg = "Uid获取失败";
                return false;
            }

            return YiChuHeiMingDan(uid, out msg);
        }

        /// <summary>
        /// 移除黑名单
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool YiChuHeiMingDan(long uid, out string msg)
        {
            string url = "http://tieba.baidu.com/bawu2/platform/cancelBlack";
            string postData = $"word={Http.UrlEncodeUtf8(TiebaName)}&tbs={TiebaWeb.GetBaiduTbs(Cookie)}&list%5B%5D={uid}&ie=utf-8";
            string html = TiebaHttpHelper.Post(url, postData, Cookie);
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

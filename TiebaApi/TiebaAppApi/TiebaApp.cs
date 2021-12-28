using CsharpHttpHelper;
using CsharpHttpHelper.Enum;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaTools;
using TiebaApi.TiebaWebApi;

namespace TiebaApi.TiebaAppApi
{
    public static class TiebaApp
    {
        /// <summary>
        /// 推荐上首页
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="tid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool TuiJianShangShouYe(string cookie, long fid, long tid, out string msg)
        {
            string url = "http://c.tieba.baidu.com/c/c/bawu/pushRecomToPersonalized";
            string postStr =
                cookie +
                //"BDUSS=EJkTURPTERRbWJUMnFJQ1drMlBnbEcyUER5a0wyVXZiYm84S2t2U0I5RG5oZnBnSVFBQUFBJCQAAAAAAAAAAAEAAADiglQb0f3Osqmv0rbJ2QAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOf40mDn-NJga" +
                "&_client_id=wappc_1623401357946_579" +
                "&_client_type=2" +
                "&_client_version=12.6.1.0" +
                //"&_phone_imei=000000000000000" +
                //"&active_timestamp=1620228883015" +
                //"&c3_aid=A00-KVSOM4UW7FB7WIHJH5UVTKTFOD2QOTER-SC27XGGW" +
                //"&cmode=1" +
                //"&cuid=34DDE2B2B66E6C36D513959AA37AEAC1%7CO" +
                //"&cuid_galaxy2=34DDE2B2B66E6C36D513959AA37AEAC1%7CO" +
                //"&cuid_gid=" +
                //"&event_day=2021623" +
                //"&first_install_time=1620228865152" +
                $"&forum_id={fid}" +
                //"&framework_ver=3290033" +
                //"&from=1024328t" +
                //"&last_update_time=1623393444130" +
                //"&model=Redmi+K20+Pro" +
                //"&net_type=1" +
                //"&oaid=A10-ME2DKOJQGNTDEMJXGU4DIOLFHE-QNKOJZML" +
                //"&sample_id=1051_4-1075_2-1097_2-1098_2-1113_1-1127_1-15016_2-15025_2-553_6-840_1" +
                //"&sdk_ver=2.29.0" +
                //"&stErrorNums=1" +
                //"&stMethod=1" +
                //"&stMode=1" +
                //"&stSize=886" +
                //"&stTime=163" +
                //"&stTimesNum=1" +
                //"&stoken=6fcc4177fa12e18992d7f6e77d15384b9125ed43c8944af1b920fa421c57f3bd" +
                //"&swan_game_ver=1033000" +
                $"&tbs={TiebaWeb.GetBaiduTbs(cookie)}" +
                $"&thread_id={tid}";
            //"&timestamp=1624439115331";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);
            string html = TiebaHttpHelper.Post_App(url, postStr, cookie);

            //{"error_code":"1","error_msg":"用户未登录或登录失败，请更换账号或重试","info":{"checkUserLogin":"1","needlogin":"1"},"server_time":"2945","time":1623999343,"ctime":0,"logid":3343017988}
            //{"error_code":"0","error_msg":"success","data":{"is_push_success":"-4","msg":"三天前发布的贴子不支持推荐上首页"},"server_time":"64697","time":1623999491,"ctime":0,"logid":3491250127}

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

            //账号掉线
            string error_code = jObject["error_code"]?.ToString();
            if (error_code != "0")
            {
                msg = jObject["error_msg"]?.ToString();
                return false;
            }

            //操作失败
            string is_push_success = jObject["data"]?["is_push_success"]?.ToString();
            if (is_push_success != "1")
            {
                msg = jObject["data"]?["msg"]?.ToString();
                return false;
            }

            msg = jObject["data"]?["msg"]?.ToString();
            return true;
        }

        /// <summary>
        /// 获取消息At我的
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="pn"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<TiebaXiaoXiAtWoDeJieGou> GetXiaoXiAtWoDe(string cookie, int pn, out string msg)
        {
            List<TiebaXiaoXiAtWoDeJieGou> tiebaXiaoXiAtWoDeLieBiao = new List<TiebaXiaoXiAtWoDeJieGou>();

            string url = "http://c.tieba.baidu.com/c/u/feed/atme";
            string postStr =
                cookie +
                $"&_client_id={Tieba.GetAndroidStamp()}" +
                "&_client_type=2" +
                "&_client_version=12.6.1.0" +
                //"&_phone_imei=000000000000000" +
                //"&active_timestamp=1620228883015" +
                //"&c3_aid=A00-KVSOM4UW7FB7WIHJH5UVTKTFOD2QOTER-SC27XGGW" +
                //"&cmode=1" +
                //"&cuid=34DDE2B2B66E6C36D513959AA37AEAC1%7CO" +
                //"&cuid_galaxy2=34DDE2B2B66E6C36D513959AA37AEAC1%7CO" +
                //"&cuid_gid=" +
                //"&event_day=2021621" +
                //"&first_install_time=1620228865152" +
                //"&from=1024328t" +
                //"&last_update_time=1623393444130" +
                //"&model=Redmi+K20+Pro" +
                //"&net_type=1" +
                $"&pn={pn}";
            //"&q_type=0" +
            //"&scr_dip=3.0" +
            //"&scr_h=2296" +
            //"&scr_w=1080" +
            //"&stErrorNums=1" +
            //"&stMethod=1" +
            //"&stMode=1" +
            //"&stSize=912" +
            //"&stTime=112" +
            //"&stTimesNum=1" +
            //"&timestamp=1624262244826" +
            //$"&uid=5422789035";
            //"&z_id=3ixbksHtfwooQvQ7_lq1d-FNsA2oGI8YINK5rGhccUF-fRoCd5IZ43AwXZU_OBGQ5d4RnOyrhCIQNbF_vNV31lw";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            string html = TiebaHttpHelper.Post_App(url, postStr, cookie);
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络错误";
                return tiebaXiaoXiAtWoDeLieBiao;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return tiebaXiaoXiAtWoDeLieBiao;
            }

            //{"no":0,"err_code":0,"error":null,"data":{"mute_text":null}}

            //访问失败
            string error_code = jObject["error_code"]?.ToString();
            if (error_code != "0")
            {
                msg = jObject["error_msg"]?.ToString();
                return tiebaXiaoXiAtWoDeLieBiao;
            }

            var at_list = jObject["at_list"];
            foreach (var at in at_list)
            {
                var quote_user = at["quote_user"];

                tiebaXiaoXiAtWoDeLieBiao.Add(new TiebaXiaoXiAtWoDeJieGou
                {
                    Uid = (long)quote_user["id"],
                    YongHuMing = quote_user["name"]?.ToString(),
                    NiCheng = quote_user["name_show"]?.ToString(),
                    TouXiang = Tieba.GuoLvTouXiangID(quote_user["portrait"]?.ToString()),
                    TiebaName = at["fname"].ToString(),
                    Tid = (long)at["thread_id"],
                    Pid = (long)at["post_id"],
                    NeiRong = at["content"].ToString()
                });
            }

            msg = "获取成功";
            return tiebaXiaoXiAtWoDeLieBiao;
        }

        /// <summary>
        /// 贴吧用户搜索
        /// </summary>
        /// <param name="tieba_uid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static TiebaYongHuSouSuoJieGou TiebaYongHuSouSuo(long tieba_uid)
        {
            TiebaYongHuSouSuoJieGou tiebaYongHuSouSuoJieGou = new TiebaYongHuSouSuoJieGou();

            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = $"https://tieba.baidu.com/mo/q/search/user?word={tieba_uid}",//URL     必需项
                Method = "GET",//URL     可选项 默认为Get
                Timeout = 100000,//连接超时时间     可选项默认为100000
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                Cookie = "",//字符串Cookie     可选项
                UserAgent = "tieba/12.11.1",//用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                ContentType = "text/html",//返回类型    可选项有默认值
                Referer = "",//来源URL     可选项
                Allowautoredirect = false,//是否根据３０１跳转     可选项
                AutoRedirectCookie = false,//是否自动处理Cookie     可选项
                Postdata = "",//Post数据     可选项GET时不需要写
                ResultType = ResultType.String,//返回数据类型，是Byte还是String
            };
            HttpResult result = http.GetHtml(item);
            string html = result.Html;
            System.Console.WriteLine(html);


            if (string.IsNullOrEmpty(html))
            {
                tiebaYongHuSouSuoJieGou.Msg = "网络错误";
                return tiebaYongHuSouSuoJieGou;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                tiebaYongHuSouSuoJieGou.Msg = "Json解析失败";
                return tiebaYongHuSouSuoJieGou;
            }

            //{"no":0,"error":"success","data":{"pn":1,"has_more":0,"exactMatch":{"id":250153151,"intro":"","user_nickname":"\u796d\u96ea\u590f\u708e","name":"\u796d\u96ea\u590f\u708e","show_nickname":"\u796d\u96ea\u590f\u708e","portrait":"https:\/\/gss0.bdstatic.com\/6LZ1dD3d1sgCo2Kml5_Y_D3\/sys\/portrait\/item\/tb.1.ebc2e776.N8YGCU10eZ_g3coX8NFY1w?t=1626146994.jpg","encry_uid":"tb.1.ebc2e776.N8YGCU10eZ_g3coX8NFY1w","fans_num":974,"is_bjh":0,"bjh_v_intro":"","business_account_info":null,"bazhu_grade":[],"display_auth_type":null,"work_creator_info":null,"is_god":0,"ala_info":{"anchor_live":"0","enter_live":"0","lat":"0.0000000","lng":"0.0000000","location":"","show_name":"\u796d\u96ea\u590f\u708e","is_yy":0},"tieba_uid":"40966187","has_concerned":0},"fuzzyMatch":[],"ubs_sample_ids":"","ubs_abtest_config":""}}

            //访问失败
            string error_code = jObject["no"]?.ToString();
            tiebaYongHuSouSuoJieGou.Msg = jObject["error"]?.ToString();
            if (error_code != "0")
            {
                return tiebaYongHuSouSuoJieGou;
            }

            var exactMatch = jObject["data"]?["exactMatch"];
            if (exactMatch.Count<object>() == 0)
            {
                return tiebaYongHuSouSuoJieGou;
            }

            long.TryParse(exactMatch["id"]?.ToString(), out tiebaYongHuSouSuoJieGou.Uid);
            long.TryParse(exactMatch["tieba_uid"]?.ToString(), out tiebaYongHuSouSuoJieGou.TiebaUid);
            tiebaYongHuSouSuoJieGou.YongHuMing = exactMatch["name"]?.ToString();
            tiebaYongHuSouSuoJieGou.NiCheng = exactMatch["user_nickname"]?.ToString();
            tiebaYongHuSouSuoJieGou.FuGaiMing = exactMatch["show_nickname"]?.ToString();
            tiebaYongHuSouSuoJieGou.JianJie = exactMatch["intro"]?.ToString();
            tiebaYongHuSouSuoJieGou.TouXiangID = Tieba.GuoLvTouXiangID(exactMatch["encry_uid"]?.ToString());
            int.TryParse(exactMatch["fans_num"]?.ToString(), out tiebaYongHuSouSuoJieGou.FenSiShu);

            tiebaYongHuSouSuoJieGou.HuoQuChengGong = true;
            return tiebaYongHuSouSuoJieGou;
        }
    }
}

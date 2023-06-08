using BakaSnowTool;
using BakaSnowTool.Http;
using CsharpHttpHelper;
using CsharpHttpHelper.Enum;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
					TouXiangID = Tieba.GuoLvTouXiangID(quote_user["portrait"]?.ToString()),
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

		public static string TiebaYongHuSouSuo2(long tieba_uid)
		{
			string cookie = "";

			string url = "http://c.tieba.baidu.com/c/u/user/getUserByTiebaUid";
			string postStr =
				cookie +
				$"&_client_id={Tieba.GetAndroidStamp()}" +
				"&_client_type=2" +
				"&_client_version=12.12.1.0" +
				$"&tieba_uid={tieba_uid}" +
				"&cmd=309702";

			postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

			string html = TiebaHttpHelper.Post_App(url, postStr, cookie);
			System.Console.WriteLine(html);




			string msg = "";
			if (string.IsNullOrEmpty(html))
			{
				msg = "网络错误";
				return msg;
			}

			JObject jObject;
			try
			{
				jObject = JObject.Parse(html);
			}
			catch
			{
				msg = "Json解析失败";
				return msg;
			}

			return msg;
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
				+ $"&tbs={TiebaWeb.GetBaiduTbs(cookie)}";

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
		/// 获取贴吧关注列表
		/// </summary>
		/// <param name="yonghuming">用户名</param>
		/// <returns></returns>
		public static List<TiebaGuanZhuJieGou> GetTiebaGuanZhuLieBiao(string cookie, string yongHuMing, int page = 1)
		{
			List<TiebaGuanZhuJieGou> tiebaGuanZhuLieBiao = new List<TiebaGuanZhuJieGou>();

			long uid = TiebaWeb.GetTiebaMingPian(yongHuMing).Uid;
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
				+ $"&tbs={TiebaWeb.GetBaiduTbs(cookie)}"
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
					TouXiangID = Tieba.GuoLvTouXiangID(post["user_portrait"]?.ToString()),
					FaTieShiJian = BST.ShiJianChuoDaoShiJian(Convert.ToInt64(post["create_time"]?.ToString()) * 1000),
					FaTieShiJianChuo = Convert.ToInt64(post["create_time"]?.ToString())
				});
			}

			msg = "获取成功";
			return lieBiao;
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
				+ "&tbs=" + TiebaWeb.GetBaiduTbs(cookie);
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

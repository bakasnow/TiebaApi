using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiebaApi.TiebaJieGou
{
	public class TiebaWebZhuTiJieGou : TiebaWebUidJieGou
	{
		public string TiebaName;//贴吧名
		public long Fid;//Fid
		public int LeiXing;//类型
		public long Tid;//Tid

		public string BiaoTi;//标题
		public string NeiRongYuLan;//内容预览
		public List<TiebaWebTuPianJieGou> TuPianLieBiao;//图片列表
		public List<TiebaWebShiPinJieGou> ShiPinLieBiao;//视频列表
		public List<TiebaWebYuYinJieGou> YuYinLieBiao;//语音列表

		public int HuiFuShu;//回复数
		public int ZanTongShu;//赞同数
		public int FanDuiShu;//反对数

		public DateTime FaTieShiJian;//发帖时间
		public long FaTieShiJianChuo;//发帖时间戳
		public DateTime ZuiHouHuiFuShiJian;//最后回复时间
		public long ZuiHouHuiFuShiJianChuo;//最后回复时间戳

		public bool IsZhiDing;//是否置顶
		public bool IsHuiYuanZhiDing;//是否会员置顶
		public bool IsHuaTi;//是否话题
	}
}

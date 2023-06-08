using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiebaApi.TiebaJieGou;

namespace TiebaApi.TiebaLeiXing
{
    public static class TiebaLeiXingZhuanHuan
    {
        /// <summary>
        /// 贴吧吧务类型转文本
        /// </summary>
        /// <param name="tiebaBaWuLeiXing"></param>
        /// <returns></returns>
        public static string TiebaBaWuLeiXingZhuanWenBen(TiebaBaWuLeiXing tiebaBaWuLeiXing)
        {
            switch (tiebaBaWuLeiXing)
            {
                case TiebaBaWuLeiXing.BaZhu:
                    return "吧主";

                case TiebaBaWuLeiXing.XiaoBaZhu:
                    return "小吧主";

                case TiebaBaWuLeiXing.YuYinXiaoBian:
                    return "语音小编";

                case TiebaBaWuLeiXing.TuPianXiaoBian:
                    return "图片小编";


                case TiebaBaWuLeiXing.ShiPinXiaoBian:
                    return "视频小编";


                case TiebaBaWuLeiXing.BaKanZhuBian:
                    return "吧刊主编";


                case TiebaBaWuLeiXing.BaKanXiaoBian:
                    return "吧刊小编";


                case TiebaBaWuLeiXing.GuangBoXiaoBian:
                    return "广播小编";

                case TiebaBaWuLeiXing.QiTa:
                    return "其他职务";

                default:
                    return "其他职务";
            }
        }

        /// <summary>
        /// 贴吧吧务文本转类型
        /// </summary>
        /// <param name="zhiWuMing"></param>
        /// <returns></returns>
        public static TiebaBaWuLeiXing TiebaBaWuWenBenZhuanLeiXing(string zhiWuMing)
        {
            switch (zhiWuMing)
            {
                case "吧主":
                    return TiebaBaWuLeiXing.BaZhu;

                case "小吧主":
                    return TiebaBaWuLeiXing.XiaoBaZhu;

                case "语音小编":
                    return TiebaBaWuLeiXing.YuYinXiaoBian;

                case "图片小编":
                    return TiebaBaWuLeiXing.TuPianXiaoBian;

                case "视频小编":
                    return TiebaBaWuLeiXing.ShiPinXiaoBian;

                case "吧刊主编":
                    return TiebaBaWuLeiXing.BaKanZhuBian;

                case "吧刊小编":
                    return TiebaBaWuLeiXing.BaKanXiaoBian;

                case "广播小编":
                    return TiebaBaWuLeiXing.GuangBoXiaoBian;

                default:
                    return TiebaBaWuLeiXing.QiTa;

            }
        }
    }
}

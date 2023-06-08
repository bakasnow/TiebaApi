using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaLeiXing;

namespace TiebaApi.TiebaWebApi
{
    public class TiebaWebNeiRong
    {
        /// <summary>
        /// 内容列表
        /// </summary>
        public List<TiebaWebNeiRongJieGou> LieBiao { get; private set; }

        /// <summary>
        /// 拼接文本
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// TiebaNeiRong
        /// </summary>
        /// <param name="jToken"></param>
        public TiebaWebNeiRong(JToken jToken)
        {
            //初始化
            LieBiao = new List<TiebaWebNeiRongJieGou>();
            Text = string.Empty;

            if (jToken == null) return;

            //索引
            int suoYin = 0;
            foreach (var content in jToken)
            {
                //Console.WriteLine(content);

                if (!int.TryParse(content["type"]?.ToString(), out int leiXing))
                {
                    continue;
                }

                switch (leiXing)
                {
                    case (int)TiebaWebNeiRongLeiXing.WenBen:
                        //文本
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.WenBen,
                            WenBen = new TiebaWebWenBenJieGou
                            {
                                WenBen = content["text"]?.ToString()
                            }
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case (int)TiebaWebNeiRongLeiXing.LianJie:
                        //链接
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.LianJie,
                            LianJie = new TiebaWebLianJieJieGou
                            {
                                WenBen = content["text"]?.ToString(),
                                LianJie = content["link"]?.ToString()
                            }
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case (int)TiebaWebNeiRongLeiXing.BiaoQing:
                        //表情
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.BiaoQing,
                            BiaoQing = new TiebaWebBiaoQingJieGou
                            {
                                WenBen = content["c"]?.ToString(),
                                DaiMa = content["text"]?.ToString()
                            }
                        });

                        //拼接文本
                        Text += $"#表情={content["c"]?.ToString()}#";
                        break;

                    case (int)TiebaWebNeiRongLeiXing.TuPian:
                        //图片
                        TiebaWebTuPianJieGou tuPianJieGou = new TiebaWebTuPianJieGou
                        {
                            KuanDu = 0,
                            GaoDu = 0,
                            IsChangTuPian = false
                        };

                        string[] bsize = content["bsize"]?.ToString().Split(',');
                        if (bsize.Length == 2)
                        {
                            int.TryParse(bsize[0], out tuPianJieGou.KuanDu);
                            int.TryParse(bsize[0], out tuPianJieGou.GaoDu);
                        }

                        tuPianJieGou.YuanTuLianJie = content["bsize"]?.ToString();
                        int.TryParse(content["size"]?.ToString(), out tuPianJieGou.DaXiao);

                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.TuPian,
                            TuPian = tuPianJieGou
                        });

                        //拼接文本
                        Text += $"#图片#";
                        break;

                    case (int)TiebaWebNeiRongLeiXing.At:
                        //艾特
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.At,
                            At = new TiebaWebAtJieGou
                            {
                                WenBen = content["text"]?.ToString(),
                                YongHuMing = content["un"]?.ToString(),
                                TouXiangID = content["portrait"]?.ToString(),
                                Uid = Convert.ToInt64(content["uid"])
                            }
                        });

                        //拼接文本
                        Text += $"#艾特={content["text"]?.ToString()}#";
                        break;

                    case (int)TiebaWebNeiRongLeiXing.ShiPin:
                        //视频
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.ShiPin,
                            ShiPin = new TiebaWebShiPinJieGou
                            {
                                SuoLueTuLianJie = content["src"]?.ToString(),
                                ShiPinLianJie1 = content["link"]?.ToString(),
                                ShiPinLianJie2 = content["text"]?.ToString(),
                                KuanDu = Convert.ToInt32(content["width"]),
                                GaoDu = Convert.ToInt32(content["height"]),
                                ShiChang = Convert.ToInt32(content["during_time"]),
                            }
                        });

                        //拼接文本
                        Text += $"#视频#";
                        break;

                    case (int)TiebaWebNeiRongLeiXing.DianHuaHaoMa:
                        //电话号码
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.DianHuaHaoMa,
                            DianHuaHaoMa = new TiebaWebDianHuaHaoMaJieGou()
                            {
                                WenBen = content["text"]?.ToString()
                            }
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case (int)TiebaWebNeiRongLeiXing.YuYin:
                        //语音
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.YuYin,
                            YuYin = new TiebaWebYuYinJieGou
                            {
                                ShiChang = Convert.ToInt32(content["during_time"]),
                                MD5 = content["voice_md5"]?.ToString()
                            }
                        });

                        //拼接文本
                        Text += $"#语音#";
                        break;

                    case (int)TiebaWebNeiRongLeiXing.HuoDong:
                        //活动
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.HuoDong,
                            HuoDong = new TiebaWebHuoDongJieGou
                            {
                                WenBen = content["album_name"]?.ToString()
                            }
                        });

                        //拼接文本
                        Text += $"#活动={content["album_name"]?.ToString()}#";
                        break;

                    case (int)TiebaWebNeiRongLeiXing.HuaTi:
                        //话题
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.HuaTi,
                            HuaTi = new TiebaWebHuaTiJieGou
                            {
                                WenBen = content["text"]?.ToString(),
                                LianJie = content["link"]?.ToString(),
                            }
                        });

                        //拼接文本
                        Text += $"#话题={content["text"]?.ToString()}#";
                        break;

                    default:
                        //其他
                        LieBiao.Add(new TiebaWebNeiRongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaWebNeiRongLeiXing.QiTa,
                            QiTa = new TiebaWebQiTaJieGou
                            {
                                JSON = content.ToString()
                            }
                        });
                        break;
                }
            }
        }
    }
}

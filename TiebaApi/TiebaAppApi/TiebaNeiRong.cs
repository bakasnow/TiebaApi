using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaLeiXing;

namespace TiebaApi.TiebaAppApi
{
    public class TiebaNeiRong
    {
        /// <summary>
        /// 内容列表
        /// </summary>
        public List<TiebaNeiRongJieGou> LieBiao { get; private set; }

        /// <summary>
        /// 拼接文本
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// TiebaNeiRong
        /// </summary>
        /// <param name="jToken"></param>
        public TiebaNeiRong(JToken jToken)
        {
            //初始化
            LieBiao = new List<TiebaNeiRongJieGou>();
            Text = string.Empty;

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
                    case (int)TiebaNeiRongLeiXing.WenBen:
                        //列表
                        LieBiao.Add(new WenBenJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.WenBen,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case (int)TiebaNeiRongLeiXing.LianJie:
                        //列表
                        LieBiao.Add(new LianJieJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.LianJie,
                            WenBen = content["text"]?.ToString(),
                            LianJie = content["link"]?.ToString()
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case (int)TiebaNeiRongLeiXing.BiaoQing:
                        //列表
                        LieBiao.Add(new BiaoQingJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.BiaoQing,
                            WenBen = content["c"]?.ToString(),
                            DaiMa = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#表情={content["c"]?.ToString()}#";
                        break;

                    case (int)TiebaNeiRongLeiXing.TuPian:
                        //列表
                        TuPianJieGou tuPianJieGou = new TuPianJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.TuPian,
                            WenBen = content["origin_src"]?.ToString(),
                            LianJie = content["origin_src"]?.ToString()
                        };

                        string[] bsize = content["bsize"]?.ToString().Split(',');
                        if (bsize.Length == 2)
                        {
                            int.TryParse(bsize[0], out tuPianJieGou.KuanDu);
                            int.TryParse(bsize[1], out tuPianJieGou.GaoDu);
                        }
                        else
                        {
                            tuPianJieGou.GaoDu = -1;
                            tuPianJieGou.KuanDu = -1;
                        }

                        int.TryParse(content["size"]?.ToString(), out tuPianJieGou.DaXiao);
                        tuPianJieGou.IsChangTuPian = content["is_long_pic"]?.ToString() == "1";
                        tuPianJieGou.IsDongTu = content["show_original_btn"]?.ToString() == "1";
                        LieBiao.Add(tuPianJieGou);

                        //拼接文本
                        Text += $"#图片#";
                        break;

                    case (int)TiebaNeiRongLeiXing.At:
                        //列表
                        LieBiao.Add(new AtJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.At,
                            WenBen = content["text"]?.ToString(),
                            Uid = Convert.ToInt64(content["uid"])
                        });

                        //拼接文本
                        Text += $"#艾特={content["text"]?.ToString()}#";
                        break;

                    case (int)TiebaNeiRongLeiXing.ShiPin:
                        //列表
                        LieBiao.Add(new ShiPinJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.ShiPin,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#视频#";
                        break;

                    case (int)TiebaNeiRongLeiXing.DianHuaHaoMa:
                        //列表
                        LieBiao.Add(new DianHuaHaoMaJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.DianHuaHaoMa,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case (int)TiebaNeiRongLeiXing.YuYin:
                        //列表
                        LieBiao.Add(new YuYinJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.YuYin,
                            WenBen = content["voice_md5"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#语音#";
                        break;

                    case (int)TiebaNeiRongLeiXing.HuoDong:
                        //列表
                        LieBiao.Add(new HuoDongJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.HuoDong,
                            WenBen = content["album_name"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#活动={content["album_name"]?.ToString()}#";
                        break;

                    case (int)TiebaNeiRongLeiXing.HuaTi:
                        //列表
                        LieBiao.Add(new HuaTiJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.HuaTi,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#话题={content["text"]?.ToString()}#";
                        break;

                    default:
                        LieBiao.Add(new QiTaJieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = TiebaNeiRongLeiXing.QiTa
                        });
                        break;
                }
            }
        }
    }
}

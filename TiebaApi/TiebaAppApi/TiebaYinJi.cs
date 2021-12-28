using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TiebaApi.TiebaAppApi
{
    public class TiebaYinJi
    {
        /// <summary>
        /// 印记列表
        /// </summary>
        public List<JieGou> LieBiao { get; private set; }

        public TiebaYinJi(JToken jToken)
        {
            //初始化
            LieBiao = new List<JieGou>();

            if (jToken is null) return;

            foreach (var iconinfo in jToken)
            {
                LieBiao.Add(new JieGou
                {
                    MingCheng = iconinfo["name"]?.ToString(),
                    TuBiaoLianJie = iconinfo["icon"]?.ToString()
                });
            }
        }

        public class JieGou
        {
            public string MingCheng;//名称
            public string TuBiaoLianJie;//图标链接
        }
    }
}

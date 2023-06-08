
namespace TiebaApi.TiebaCanShu
{
    /// <summary>
    /// 贴吧删主题参数
    /// </summary>
    public class TiebaShanZhuTiCanShu
    {
        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie { get; set; }

        /// <summary>
        /// 贴吧名
        /// </summary>
        public string TiebaName { get; set; }

        /// <summary>
        /// Fid
        /// </summary>
        public long Fid { get; set; }

        /// <summary>
        /// Tid
        /// </summary>
        public long Tid { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VCode { get; set; }

        /// <summary>
        /// 验证码MD5
        /// </summary>
        public string VCode_md5 { get; set; }
    }
}

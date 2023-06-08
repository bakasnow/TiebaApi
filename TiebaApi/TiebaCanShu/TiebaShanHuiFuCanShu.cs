
namespace TiebaApi.TiebaCanShu
{
    /// <summary>
    /// 贴吧删回复参数
    /// </summary>
    public class TiebaShanHuiFuCanShu
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
        /// Pid
        /// </summary>
        public long Pid { get; set; }

        /// <summary>
        /// 是否楼中楼
        /// </summary>
        public bool IsFinf { get; set; }

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

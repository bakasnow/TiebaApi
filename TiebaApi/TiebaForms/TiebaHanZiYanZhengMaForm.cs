using ImageProcessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiebaApi.TiebaForms
{
    public partial class TiebaHanZiYanZhengMaForm : Form
    {
        public TiebaHanZiYanZhengMaForm()
        {
            InitializeComponent();

            Text = "请输入验证码";
            pictureBox1.Tag = 1;
            pictureBox2.Tag = 2;
            pictureBox3.Tag = 3;
            pictureBox4.Tag = 4;
            pictureBox5.Tag = 5;
            pictureBox6.Tag = 6;
            pictureBox7.Tag = 7;
            pictureBox8.Tag = 8;
            pictureBox9.Tag = 9;
        }

        public string captcha_vcode_str;
        public string captcha_input_str;

        private void TiebaHanZiYanZhengMaForm_Load(object sender, EventArgs e)
        {
            //刷新验证码
            ShuaXinYanZhengMa();
        }

        private void pictureBox0_Click(object sender, EventArgs e)
        {
            //刷新验证码
            ShuaXinYanZhengMa();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox2);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox3);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox4);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox5);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox6);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox7);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox8);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            DanJiShiJian(pictureBox9);
        }

        /// <summary>
        /// 刷新验证码
        /// </summary>
        private void ShuaXinYanZhengMa()
        {
            //获取验证码
            Image image = Image.FromStream(System.Net.WebRequest.Create($"https://tieba.baidu.com/cgi-bin/genimg?{captcha_vcode_str}").GetResponse().GetResponseStream());
            ImageFactory imageFactory = new ImageFactory(true);

            //分割图片
            pictureBox0.Image = imageFactory.Load(image).Crop(new Rectangle(0, 0, 240, 80)).Image;
            pictureBox1.Image = imageFactory.Load(image).Crop(new Rectangle(0, 80, 80, 80)).Image;
            pictureBox2.Image = imageFactory.Load(image).Crop(new Rectangle(80, 80, 80, 80)).Image;
            pictureBox3.Image = imageFactory.Load(image).Crop(new Rectangle(160, 80, 80, 80)).Image;
            pictureBox4.Image = imageFactory.Load(image).Crop(new Rectangle(0, 160, 80, 80)).Image;
            pictureBox5.Image = imageFactory.Load(image).Crop(new Rectangle(80, 160, 80, 80)).Image;
            pictureBox6.Image = imageFactory.Load(image).Crop(new Rectangle(160, 160, 80, 80)).Image;
            pictureBox7.Image = imageFactory.Load(image).Crop(new Rectangle(0, 240, 80, 80)).Image;
            pictureBox8.Image = imageFactory.Load(image).Crop(new Rectangle(80, 240, 80, 80)).Image;
            pictureBox9.Image = imageFactory.Load(image).Crop(new Rectangle(160, 240, 80, 80)).Image;

            //重置控件
            YanZhengMaLieBiao = new List<int>();
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            pictureBox3.BorderStyle = BorderStyle.FixedSingle;
            pictureBox4.BorderStyle = BorderStyle.FixedSingle;
            pictureBox5.BorderStyle = BorderStyle.FixedSingle;
            pictureBox6.BorderStyle = BorderStyle.FixedSingle;
            pictureBox7.BorderStyle = BorderStyle.FixedSingle;
            pictureBox8.BorderStyle = BorderStyle.FixedSingle;
            pictureBox9.BorderStyle = BorderStyle.FixedSingle;
        }

        /// <summary>
        /// 验证码列表
        /// </summary>
        private List<int> YanZhengMaLieBiao = new List<int>();

        /// <summary>
        /// 单击事件
        /// </summary>
        /// <param name="pictureBox"></param>
        private void DanJiShiJian(PictureBox pictureBox)
        {
            //取出图片框Tag标签里的数字
            int tag = (int)pictureBox.Tag;

            if (pictureBox.BorderStyle == BorderStyle.FixedSingle)
            {
                pictureBox.BorderStyle = BorderStyle.Fixed3D;

                YanZhengMaLieBiao.Add(tag);
            }
            else
            {
                //如果不是最后一个数组
                if (YanZhengMaLieBiao[YanZhengMaLieBiao.Count - 1] != tag)
                {
                    return;
                }

                //移除最后一个数组
                YanZhengMaLieBiao.RemoveAt(YanZhengMaLieBiao.Count - 1);

                pictureBox.BorderStyle = BorderStyle.FixedSingle;
            }

            //验证码是否到达4位
            if (YanZhengMaLieBiao.Count >= 4)
            {
                foreach (var yanZhengMa in YanZhengMaLieBiao)
                {
                    switch (yanZhengMa)
                    {
                        case 1:
                            captcha_input_str += "00000000";
                            break;
                        case 2:
                            captcha_input_str += "00010000";
                            break;
                        case 3:
                            captcha_input_str += "00020000";
                            break;
                        case 4:
                            captcha_input_str += "00000001";
                            break;
                        case 5:
                            captcha_input_str += "00010001";
                            break;
                        case 6:
                            captcha_input_str += "00020001";
                            break;
                        case 7:
                            captcha_input_str += "00000002";
                            break;
                        case 8:
                            captcha_input_str += "00010002";
                            break;
                        case 9:
                            captcha_input_str += "00020002";
                            break;
                        default:
                            break;
                    }
                }

                Close();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace WebMonitorPlugin.Utils
{
    public class MailUtil
    {
        public static bool SendMail(MailOptions options, out string errorMsg)
        {
            // System.IO.IOException: Unable to read data from the transport connection: The connection was closed.
            // https://stackoverflow.com/questions/62883394/how-to-fix-system-io-ioexception-unable-to-read-data-from-the-transport-connec
            // 失败
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            //声明一个Mail对象
            MailMessage mailMessage = new MailMessage();
            // 发件人地址
            mailMessage.From = new MailAddress(options.SenderDisplayAddress, options.SenderDisplayName, Encoding.UTF8);
            // 收件人地址
            mailMessage.To.Add(new MailAddress(options.ReceiveAddress));
            // 邮件主题
            mailMessage.Subject = options.Subject;
            // 邮件标题编码
            mailMessage.SubjectEncoding = Encoding.UTF8;
            // 发送邮件的内容
            mailMessage.Body = options.Content;
            // 邮件内容编码
            mailMessage.BodyEncoding = Encoding.UTF8;
            if (options.Attachments != null && options.Attachments.Count >= 1)
            {
                // 添加附件
                foreach (var attachment in options.Attachments)
                {
                    mailMessage.Attachments.Add(attachment);
                }
            }
            if (options.Cc != null && options.Cc.Count >= 1)
            {
                // 抄送到其他邮箱
                foreach (var str in options.Cc)
                {
                    mailMessage.CC.Add(new MailAddress(str));
                }
            }


            // 是否是HTML邮件
            mailMessage.IsBodyHtml = options.IsHtml;
            // 邮件优先级
            mailMessage.Priority = MailPriority.Low;

            // 创建一个邮件服务器类
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;// 指定电子邮件发送方式
            smtpClient.EnableSsl = options.EnableSsl; // 启用SSL
            smtpClient.Host = options.Host;
            // SMTP服务端口
            smtpClient.Port = options.Port;
            // 验证登录
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(options.UserName, options.Password);//"@"输入有效的邮件名, "*"输入有效的密码

            try
            {
                smtpClient.Send(mailMessage);
                errorMsg = "";

                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();

                return false;
            }

        }
    }

    public class MailOptions
    {
        #region 发送者

        public string SenderDisplayAddress { get; set; }

        public string SenderDisplayName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        #endregion

        #region 接收者

        public string ReceiveAddress { get; set; }

        #endregion

        #region 邮件设置

        public string Subject { get; set; }

        public string Content { get; set; }

        public bool IsHtml { get; set; } = true;

        /// <summary>
        /// 附件
        /// </summary>
        public IList<Attachment> Attachments { get; set; }

        #endregion

        #region 发送设置

        /// <summary>
        /// 抄送地址
        /// </summary>
        public IList<string> Cc { get; set; }

        public string Host { get; set; }

        public int Port { get; set; } = 25;

        public bool EnableSsl { get; set; } = true;

        #endregion

    }
}

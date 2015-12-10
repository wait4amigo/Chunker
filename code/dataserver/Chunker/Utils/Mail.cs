using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Chunker.Utils
{
    public class MailConfig
    {
        private string _smtpServer = string.Empty;
        private int _smtpServerPort = 45;
        private string _smtpServerMailFrom = string.Empty;
        private string _smtpServerMailTo = string.Empty;
        private string _smtpServerAccount = string.Empty;
        private string _smtpServerPassword = string.Empty;
        private string _smtpServerAuthType = string.Empty;
        private string _smtpServerMailFromDisplayname = string.Empty;

        public string SmtpServer { get { return _smtpServer; } }
        public int SmtpServerPort { get { return _smtpServerPort; } }
        public string SmtpServerMailFrom { get { return _smtpServerMailFrom; } }
        public string SmtpServerMailTo { get { return _smtpServerMailTo; } }
        public string SmtpServerAccount { get { return _smtpServerAccount; } }
        public string SmtpServerPassword { get { return _smtpServerPassword; } }
        public string SmtpServerAuthType { get { return _smtpServerAuthType; } }
        public string SmtpServerMailFromDisplayname { get { return _smtpServerMailFromDisplayname; } }

        public MailConfig()
        {
            _smtpServer = ConfigurationManager.AppSettings.Get("SmtpServer");
            _smtpServerPort = int.Parse(ConfigurationManager.AppSettings.Get("SmtpServerPort"));
            _smtpServerMailFrom = ConfigurationManager.AppSettings.Get("SmtpServerMailFrom");
            _smtpServerMailTo = ConfigurationManager.AppSettings.Get("SmtpServerMailTo");
            _smtpServerAccount = ConfigurationManager.AppSettings.Get("SmtpServerAccount");
            _smtpServerPassword = ConfigurationManager.AppSettings.Get("SmtpServerPassword");
            _smtpServerAuthType = ConfigurationManager.AppSettings.Get("SmtpServerAuthType");
            _smtpServerMailFromDisplayname = ConfigurationManager.AppSettings.Get("SmtpServerMailFromDisplayname");
        }
    }

    public class Mail
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");
        private static MailConfig _mailConfig = new MailConfig();

        public static bool SendMail(string mailTo, string subject, string body)
        {
            return SendMail(
                _mailConfig.SmtpServer,
                _mailConfig.SmtpServerPort,
                _mailConfig.SmtpServerAccount,
                _mailConfig.SmtpServerPassword,
                _mailConfig.SmtpServerAuthType,
                _mailConfig.SmtpServerMailFromDisplayname,
                _mailConfig.SmtpServerMailFrom,
                mailTo,
                subject,
                body);
        }

        private static bool SendMail(string smtpServer, int smtpServerPort,
                                     string smtpServerAccount, string smtpServerPassword, string smtpServerAuthType,
                                     string smtpServerMailFromDisplayname,
                                     string smtpServerMailFrom, string smtpServerMailTo,
                                     string title, string content)
        {
            bool ret = false;

            try
            {
                using (SmtpClient client = new SmtpClient(smtpServer, smtpServerPort))
                {
                    client.Host = smtpServer;
                    client.Port = smtpServerPort;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpServerAccount, smtpServerPassword);

                    MailAddress fromAddress = new MailAddress(smtpServerMailFrom, smtpServerMailFromDisplayname);
                    MailAddress toAddress = new MailAddress(smtpServerMailTo, smtpServerMailTo.Substring(0, smtpServerMailTo.IndexOf("@")));

                    using (MailMessage mailMessage = new MailMessage(fromAddress, toAddress))
                    {
                        mailMessage.Subject = title;
                        mailMessage.Body = content;
                        mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                        mailMessage.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");
                        mailMessage.IsBodyHtml = false;
                        mailMessage.Priority = MailPriority.High;

                        client.Send(mailMessage);
                    }
                }

                ret = true;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Exception sending mail: {0}", ex.ToString()));
            }
            finally
            {
            }

            return ret;
        }
    }
}
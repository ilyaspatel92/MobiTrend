﻿using System;
using System.Net;
using System.Net.Mail;

namespace Mobi.Service.EmailServices
{
    /// <summary>
    /// Email service implementation for sending emails via Gmail SMTP.
    /// </summary>
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        /// <summary>
        /// Constructor for EmailService.
        /// </summary>
        /// <param name="smtpHost">SMTP server host.</param>
        /// <param name="smtpPort">SMTP server port.</param>
        /// <param name="smtpUser">SMTP username (email address).</param>
        /// <param name="smtpPass">SMTP password (App Password for Gmail).</param>
        public EmailService(string smtpHost, int smtpPort, string smtpUser, string smtpPass)
        {
            _smtpHost = smtpHost ?? throw new ArgumentNullException(nameof(smtpHost));
            _smtpPort = smtpPort;
            _smtpUser = smtpUser ?? throw new ArgumentNullException(nameof(smtpUser));
            _smtpPass = smtpPass ?? throw new ArgumentNullException(nameof(smtpPass));
        }

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="messageBody">Email message body (HTML or plain text).</param>
        public void SendEmail(string toEmail, string subject, string messageBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email cannot be null or empty.", nameof(toEmail));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be null or empty.", nameof(subject));
            if (string.IsNullOrWhiteSpace(messageBody))
                throw new ArgumentException("Message body cannot be null or empty.", nameof(messageBody));

            try
            {
                using (var client = CreateSmtpClient())
                using (var mailMessage = CreateMailMessage(toEmail, subject, messageBody))
                {
                    client.Send(mailMessage);
                }
            }
            catch (SmtpException smtpEx)
            {
                throw new InvalidOperationException("SMTP error occurred while sending email.", smtpEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }

        /// <summary>
        /// Configures the SMTP client.
        /// </summary>
        /// <returns>A fully configured SmtpClient instance.</returns>
        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 10000
            };
        }

        /// <summary>
        /// Creates the MailMessage with appropriate headers.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="messageBody">HTML message body.</param>
        /// <returns>Configured MailMessage object.</returns>
        private MailMessage CreateMailMessage(string toEmail, string subject, string messageBody)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser, "AdminMobi Support"),
                Subject = subject,
                Body = messageBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            return mailMessage;
        }
    }
}

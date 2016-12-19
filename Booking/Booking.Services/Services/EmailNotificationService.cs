using System;
using System.Net;
using System.Net.Mail;
using Booking.Models;
using Booking.Models.EfModels;
using System.IO;
using System.Threading.Tasks;
using Booking.Services.Interfaces;
using NLog;
using RazorEngine.Templating;


namespace Booking.Services.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly MailAddress _emailFromAddress = new MailAddress("audiencebookingtest@gmail.com", "Audience");

        private readonly string _templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "EmailTemplates");

        private readonly TemplateService _templateService;

        public EmailNotificationService()
        {
            _templateService = new TemplateService();
        }

        private void SendMail(MailMessage email)
        {
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("audiencebookingtest@gmail.com", "Qwer123!"),
                EnableSsl = true
            };
            try
            {
                smtp.Send(email);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private MailMessage GenerateEmail(string emailHtmlBody, string subject)
        {
            return new MailMessage
            {
                From = _emailFromAddress,
                Body = emailHtmlBody,
                IsBodyHtml = true,
                Subject = subject
            };
        }

        public void AccountRegisteredNotification(ApplicationUser user)
        {
            Task.Run(() =>
            {
                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "AccountRegisteredNotificationTemplate.cshtml");

                var modelEmail = new Booking.Services.EmailModels.AccountRegisteredRemovedNotificationModel
                {
                    Name = user.UserName,
                    Email = user.Email
                };

                var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null, null);

                var subject = Resources.Resources.Registered_to + " " + Resources.Resources.Site_address;
                var email = GenerateEmail(emailHtmlBody, subject);

                email.To.Add(new MailAddress(modelEmail.Email, modelEmail.Name));

                SendMail(email);
            });
        }


        public void AccountRemovedNotification(ApplicationUser user)
        {
            Task.Run(() =>
            {
                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "AccountRemovedNotificationTemplate.cshtml");

                var modelEmail = new Booking.Services.EmailModels.AccountRegisteredRemovedNotificationModel
                {
                    Name = user.UserName,
                    Email = user.Email
                };

                var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null, null);

                var subject = Resources.Resources.Account_remove_from + " " + Resources.Resources.Site_address;
                var email = GenerateEmail(emailHtmlBody, subject);

                email.To.Add(new MailAddress(modelEmail.Email, modelEmail.Name));

                SendMail(email);
            });
        }

        public void EventCancelledNotification(Event eventEntity)
        {
            Task.Run(() =>
            {
                var participants = eventEntity.EventParticipants;
                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "EventCancelledNotificationTemplate.cshtml");

                var subject = Resources.Resources.Event_cancelled_notification;
                var modelEmail = new Booking.Services.EmailModels.EventCancellModel
                {
                    EventTitle = eventEntity.Title,
                    EventDateTime = eventEntity.StartTime.ToLongDateString()
                };

                foreach (var participant in participants)
                {
                    modelEmail.Email = participant.ParticipantEmail;
                    var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null,
                        null);
                    var email = GenerateEmail(emailHtmlBody, subject);

                    email.To.Add(new MailAddress(modelEmail.Email));

                    SendMail(email);
                }
            });
        }

        public void EventCancelledAuthorNotification(Event eventEntity)
        {
            Task.Run(() =>
            {
                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "EventCancelledAuthorNotificationTemplate.cshtml");
                var user = eventEntity.Author;
                var modelEmail = new Booking.Services.EmailModels.EventCancelledAuthorModel
                {
                    Name = user.UserName,
                    Email = user.Email,
                    EventDateTime = eventEntity.StartTime.ToLongDateString(),
                    EventTitle = eventEntity.Title
                };

                var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null, null);

                var subject = Resources.Resources.Cancel_event_notification_to_author;
                var email = GenerateEmail(emailHtmlBody, subject);

                email.To.Add(new MailAddress(modelEmail.Email, modelEmail.Name));

                SendMail(email);
            });
        }

        public void RemovedFromParticipantsListNotification(string email, Event eventEntity)
        {
            Task.Run(() =>
            {
                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "RemovedFromParticipantsListNotificationTemplate.cshtml");
                var modelEmail = new Booking.Services.EmailModels.RemovedJoinedFromParticipantsListModel
                {
                    Email = email,
                    EventTitle = eventEntity.Title,
                    EventDate = eventEntity.StartTime.ToLongDateString()
                };

                var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null, null);

                var subject = Resources.Resources.Remove_from_participants_list_notification;
                var mailMessage = GenerateEmail(emailHtmlBody, subject);

                mailMessage.To.Add(new MailAddress(modelEmail.Email));

                SendMail(mailMessage);
            });
        }

        public void EventJoinedNotification(string email, Event eventEntity)
        {
            Task.Run(() =>
            {
                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "JoinedToParticipantsListNotificationTemplate.cshtml");
                var modelEmail = new Booking.Services.EmailModels.RemovedJoinedFromParticipantsListModel
                {
                    Email = email,
                    EventTitle = eventEntity.Title,
                    EventDate = eventEntity.StartTime.ToLongDateString()
                };

                var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null, null);

                var subject = Resources.Resources.Joined_to_participants_list_notification;
                var mailMessage = GenerateEmail(emailHtmlBody, subject);

                mailMessage.To.Add(new MailAddress(modelEmail.Email));

                SendMail(mailMessage);
            });
        }

        public void EventEditedNotification(Event newEvent, Event oldEvent)
        {
            Task.Run(() =>
            {
                var participants = oldEvent.EventParticipants;
                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "EventEditedNotificationTemplate.cshtml");

                var subject = Resources.Resources.Event_edit_notification;
                var modelEmail = new Booking.Services.EmailModels.EventEditedModel
                {
                    Title = oldEvent.Title,
                    OldDate = oldEvent.StartTime.ToLongDateString(),
                    NewDate = newEvent.StartTime.ToLongDateString()
                };

                foreach (var participant in participants)
                {
                    modelEmail.Email = participant.ParticipantEmail;
                    var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null,
                        null);
                    var email = GenerateEmail(emailHtmlBody, subject);

                    email.To.Add(new MailAddress(modelEmail.Email));

                    SendMail(email);
                }
            });
        }

        public void EventEditedAuthorNotification(Event newEvent, Event oldEvent)
        {
            Task.Run(() =>
            {
                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "EventEditedAuthorNotificationTemplate.cshtml");
                var user = oldEvent.Author;
                var modelEmail = new Booking.Services.EmailModels.EventEditedAuthorNotificationModel
                {
                    Name = user.UserName,
                    Email = user.Email,
                    OldDate = oldEvent.StartTime.ToLongDateString(),
                    NewDate = newEvent.Title
                };

                var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null, null);

                var subject = Resources.Resources.Edit_event_notification_to_author;
                var email = GenerateEmail(emailHtmlBody, subject);

                email.To.Add(new MailAddress(modelEmail.Email, modelEmail.Name));

                SendMail(email);
            });
        }

        public void SendFeedbackToAdmins(string name, string surname, string email, string message)
        {
            Task.Run(() =>
            {
                var service = new UsersService();
                var adminsEmails = service.GetAdminsEmails();

                var emailTemplatePath = Path.Combine(_templateFolderPath.Replace("Web", "Services"),
                    "SendFeedbackToAdminsNotificationTemplate.cshtml");

                var subject = Resources.Resources.Feedback_from_user;
                var modelEmail = new Booking.Services.EmailModels.SendFeedbackToAdminsModel
                {
                    Message = message,
                    Name = name + " " + surname,
                    UserEmail = email
                };

                foreach (var adminEmail in adminsEmails)
                {
                    modelEmail.Email = adminEmail;
                    var emailHtmlBody = _templateService.Parse(File.ReadAllText(emailTemplatePath), modelEmail, null,
                        null);
                    var mail = GenerateEmail(emailHtmlBody, subject);

                    mail.To.Add(new MailAddress(modelEmail.Email));

                    SendMail(mail);
                }
            });
        }

        public void ConfirmEmailAddress(ApplicationUser user, string emailBody)
        {
            Task.Run(() =>
            {
                var modelEmail = new Booking.Services.EmailModels.AccountRegisteredRemovedNotificationModel
                {
                    Name = user.UserName,
                    Email = user.Email
                };

                var subject = Resources.Resources.Confirm_account_for + " " + Resources.Resources.Site_address;
                var email = GenerateEmail(emailBody, subject);

                email.To.Add(new MailAddress(modelEmail.Email, modelEmail.Name));

                SendMail(email);
            });
        }
    }
}
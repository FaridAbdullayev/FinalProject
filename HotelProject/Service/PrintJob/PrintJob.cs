using Core.Entities;
using Core.Entities.Enum;
using Data;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Service.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.PrintJob
{
    public class PrintJob:IJob
    {
        private readonly EmailService _emailService;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PrintJob(EmailService emailService, AppDbContext appDbContext,UserManager<AppUser> userManager)
        {
            _emailService = emailService;
            _context = appDbContext;
                _userManager = userManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var threeDaysLater = now.AddDays(3);

            // 3 gün içinde başlayacak rezervasyonları bul
            var reservations = _context.Reservations
                .Where(x => x.Status == OrderStatus.Accepted && x.StartDate > now && x.StartDate <= threeDaysLater)
                .ToList();

            if (!reservations.Any())
                return;

            foreach (var reservation in reservations)
            {
                var user = await _userManager.FindByIdAsync(reservation.AppUserId);
                if (user == null)
                    continue;

                var emailSubject = "Rezervasyonunuzun Başlama Tarihi Yaklaşıyor";
                var emailBody = $"Merhaba {user.UserName}, rezervasyonunuz {reservation.StartDate.ToShortDateString()} tarihinde başlayacaktır. Lütfen hazırlığınızı yapın.";

                // E-posta gönder
                _emailService.Send(user.Email, emailSubject, emailBody);
            }
        }



    }
}

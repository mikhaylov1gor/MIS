using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MIS.Models.DB;
using MIS.Models.DTO;
using Quartz;
using System.Net;
using System.Net.Mail;

namespace MIS.Infrastucture.Email
{
    public class SkippedVisitsNotificationJob : IJob
    {
        private readonly MisDbContext _context;
        private readonly IEmailSender _emailSender;
        public SkippedVisitsNotificationJob(MisDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var missedVisits = await _context.Inspections
                .Include(d => d.doctor)
                .Include(p => p.patient)
                .Where(i => i.nextVisitDate < DateTime.UtcNow && !i.hasNested)
                .ToListAsync();

            foreach (var inspection in missedVisits)
            {
                var subject = $"Пациент {inspection.patient.name} пропустил осмотр";
                var body = $"Уважаемый доктор {inspection.doctor.name}, пациент {inspection.patient.name} пропустил запланированный на {inspection.nextVisitDate} осмотр";

                _emailSender.SendEmail(inspection.doctor.email, subject, body);
            }
        }
    }
}

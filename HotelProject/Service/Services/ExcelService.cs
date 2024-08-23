using Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Service.Services.Interfaces;

using System.ComponentModel;
using LicenseContext = OfficeOpenXml.LicenseContext;


namespace Service.Services
{
    public class ExcelService : IExcelService
    {
        private readonly AppDbContext _context;

        public ExcelService(AppDbContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> ExportSessionsAsync()
        {
            var sessions = await _context.Reservations.Include(x=>x.Room).Include(x=>x.AppUser).ToListAsync();
                
            

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reservation");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Price";
                worksheet.Cells[1, 3].Value = "UserName";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "StartDate";
                worksheet.Cells[1, 6].Value = "EndDate";
                worksheet.Cells[1, 7].Value = "Status";
                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    worksheet.Cells[i + 2, 1].Value = session.Room.Name;
                    worksheet.Cells[i + 2, 2].Value = session.Room.Price.ToString("C");
                    worksheet.Cells[i + 2, 3].Value = session.AppUser.UserName;
                    worksheet.Cells[i + 2, 4].Value = session.AppUser.Email;
                    worksheet.Cells[i + 2, 5].Value = session.StartDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 6].Value = session.EndDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 7].Value = session.Status;
                }

                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}

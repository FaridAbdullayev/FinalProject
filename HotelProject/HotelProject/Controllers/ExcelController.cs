using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly IExcelService _excelExportService;

        public ExcelController(IExcelService excelExportService)
        {
            _excelExportService = excelExportService;
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            var fileContent = await _excelExportService.ExportSessionsAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Rooms.xlsx");
        }
    }
}

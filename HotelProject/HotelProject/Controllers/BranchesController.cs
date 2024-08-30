using Hangfire.MemoryStorage.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos;
using Service.Dtos.Branch;
using Service.Dtos.Users;
using Service.Services;
using Service.Services.Interfaces;
using static Service.Exceptions.ResetException;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _service;

        public BranchesController(IBranchService branchService)
        {
            _service = branchService;
        }




        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("")]
        public ActionResult Create(BranchCreateDto createDto)
        {
            return StatusCode(201, new { id = _service.Create(createDto) });
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var data = _service.GetById(id);
            return StatusCode(200, data);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("all")]
        public ActionResult<List<BranchListItemGetDto>> GetAllBranch()
        {
            return Ok(_service.GetAll());
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("")]
        public ActionResult<PaginatedList<BranchGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public ActionResult Update(int id, BranchUpdateDto branchUpdateDto)
        {
            _service.Update(branchUpdateDto, id);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("incomes")]
        public async Task<ActionResult<List<BranchIncome>>> GetBranchIncomes()
        {
           
                var incomes = _service.GetBranchIncomes();
                return Ok(incomes);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("most-income")]
        public async Task<ActionResult<BranchIncome>> GetBranchWithMostIncome()
        {
            
                var branchWithMostIncome = _service.GetBranchWithMostIncome();
                if (branchWithMostIncome == null)
                {
                    return NotFound("No branches found.");
                }
                return Ok(branchWithMostIncome);
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("user/all")]
        public ActionResult<List<MemberBranchGetDto>> UserGetAllBranch()
        {
            return Ok(_service.UserGetAllBranch());
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("{id}/room")]
        public IActionResult GetBranchWithRooms(int id)
        {
            var branchWithRooms = _service.GetBranchWithRooms(id);
            return StatusCode(200, branchWithRooms);
        }
    }
}

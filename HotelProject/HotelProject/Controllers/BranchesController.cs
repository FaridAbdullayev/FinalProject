﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos;
using Service.Dtos.Branch;
using Service.Services.Interfaces;

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
        [HttpPost("")]
        public ActionResult Create(BranchCreateDto createDto)
        {
            return StatusCode(201, new { id = _service.Create(createDto) });
        }

        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var data = _service.GetById(id);
            return StatusCode(200, data);
        }

        [HttpGet("all")]
        public ActionResult<List<BranchListItemGetDto>> GetAllBranch()
        {
            return Ok(_service.GetAll());
        }
        [HttpGet("")]
        public ActionResult<PaginatedList<BranchGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id,BranchUpdateDto branchUpdateDto)
        {
            _service.Update(branchUpdateDto, id);
            return NoContent();
        }

    }
}

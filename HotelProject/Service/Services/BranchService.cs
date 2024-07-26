﻿using AutoMapper;
using Core.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Service.Dtos;
using Service.Dtos.Branch;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;

namespace Service.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;


        public BranchService(IBranchRepository branchRepository, IMapper mapper)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
        }
        public int Create(BranchCreateDto createDto)
        {
            if(_branchRepository.Exists(x=>x.Name == createDto.Name && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Branch already taken");

            Branch branch = _mapper.Map<Branch>(createDto);

            _branchRepository.Add(branch);

            _branchRepository.Save();

            return branch.Id;
        }

        public void Delete(int id)
        {
            Branch entity = _branchRepository.Get(x=>x.Id == id);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Branch not found");

            entity.IsDeleted = true;
            entity.UpdateAt = DateTime.Now;
            _branchRepository.Save();
        }

        public List<BranchListItemGetDto> GetAll()
        {
            return _mapper.Map<List<BranchListItemGetDto>>(_branchRepository.GetAll(x => !x.IsDeleted)).ToList();
        }

        public PaginatedList<BranchGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _branchRepository.GetAll(x => !x.IsDeleted && (search == null || x.Name.Contains(search)));
            var paginated = PaginatedList<Branch>.Create(query, page, size);
            return new PaginatedList<BranchGetDto>(_mapper.Map<List<BranchGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public BranchGetDto GetById(int id)
        {
            Branch entity = _branchRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Branch not found");

            return _mapper.Map<BranchGetDto>(entity);
        }

        public void Update(BranchUpdateDto updateDto, int id)
        {
            Branch entity = _branchRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity.Name != updateDto.Name && _branchRepository.Exists(x => x.Name == updateDto.Name && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Branch already taken");

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Branch not found");

            entity.Name = updateDto.Name;
            entity.UpdateAt = DateTime.Now;
            _branchRepository.Save();
        }
    }
}
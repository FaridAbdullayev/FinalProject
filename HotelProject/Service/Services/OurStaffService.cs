using AutoMapper;
using Core.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pustok.Helpers;
using Service.Dtos;
using Service.Dtos.OurStaff;
using Service.Dtos.Service;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;

namespace Service.Services
{
    public class OurStaffService : IOurStaffService
    {
        private readonly IOurStaffRepository _serviceRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public OurStaffService(IOurStaffRepository serviceRepository, IMapper mapper,IWebHostEnvironment webHostEnvironment)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _env = webHostEnvironment;
        }

        public async Task<int> OurStaffCount()
        {
            return await _serviceRepository.GetAll(x => x.IsDeleted == false).CountAsync();
        }
        public int Create(OurStaffCreateDto createDto)
        {

            OurStaff service = new()
            {
                Name = createDto.Name,
                Position = createDto.Position,
                Description = createDto.Description,
                Image = createDto.File.Save(_env.WebRootPath, "uploads/staff")
            };

            _serviceRepository.Add(service);

            _serviceRepository.Save();

            return service.Id;


        }

        public void Delete(int id)
        {

            OurStaff entity = _serviceRepository.Get(x => x.Id == id);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "OurStaff not found");

            entity.IsDeleted = true;
            entity.UpdateAt = DateTime.Now;
            _serviceRepository.Save();
        }

        public List<OurStaffListItemGetDto> GetAll()
        {
            return _mapper.Map<List<OurStaffListItemGetDto>>(_serviceRepository.GetAll(x => !x.IsDeleted)).ToList();
        }

        public PaginatedList<OurStaffGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _serviceRepository.GetAll(x => !x.IsDeleted && (search == null || x.Name.Contains(search)));
            var paginated = PaginatedList<OurStaff>.Create(query, page, size);
            return new PaginatedList<OurStaffGetDto>(_mapper.Map<List<OurStaffGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public OurStaffGetDto GetById(int id)
        {
            OurStaff entity = _serviceRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Staff not found");

            return _mapper.Map<OurStaffGetDto>(entity);
        }

        public void Update(OurStaffUpdateDto updateDto, int id)
        {
            OurStaff entity = _serviceRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Staff not found");

        


            string deletedFile = null;
            if (updateDto.File != null)
            {
                deletedFile = entity.Image;
                entity.Image = FileManager.Save(updateDto.File, _env.WebRootPath, "uploads/staff");

            }


            entity.Description = updateDto.Description;
            entity.Name = updateDto.Name;
            entity.Position = updateDto.Position;
            entity.UpdateAt = DateTime.Now;

            if (deletedFile != null)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/staff", deletedFile);
            }


            _serviceRepository.Save();
        }

        public List<MemberOurStaffGetDto> MemberGetAllOurStaff()
        {
            return _mapper.Map<List<MemberOurStaffGetDto>>(_serviceRepository.GetAll(x => !x.IsDeleted)).ToList();
        }

        public List<OurStaffGetForAboutDto> MemberGetAllOurStaffForUser()
        {
            return _mapper.Map<List<OurStaffGetForAboutDto>>(_serviceRepository.GetAll(x => !x.IsDeleted)).Take(4).ToList();
        }

    }
}

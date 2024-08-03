using AutoMapper;
using Core.Entities;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Service.Dtos;
using Service.Dtos.BedType;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;

namespace Service.Services
{
    public class BedTypeService : IBedTypeService
    {
        private readonly IBedTypeRepository _bedRepository;
        private readonly IMapper _mapper;


        public BedTypeService(IBedTypeRepository bedRepository, IMapper mapper)
        {
            _bedRepository = bedRepository;
            _mapper = mapper;
        }
        public int Create(BedTypeCreateDto createDto)
        {
            if (_bedRepository.Exists(x => x.Name == createDto.Name))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Bed already taken");

            BedType bed = _mapper.Map<BedType>(createDto);

            _bedRepository.Add(bed);

            _bedRepository.Save();

            return bed.Id;
        }

        public void Delete(int id)
        {
            BedType bed = _bedRepository.Get(x => x.Id == id);
            if (bed == null) throw new RestException(StatusCodes.Status404NotFound, "Bed not found");
            _bedRepository.Delete(bed);
            _bedRepository.Save();
        }

        public List<BedTypeListItemGetDto> GetAll()
        {
            return _mapper.Map<List<BedTypeListItemGetDto>>(_bedRepository.GetAll(x => true)).ToList();
        }
        public PaginatedList<BedTypeGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _bedRepository.GetAll(x=>x.Name.Contains(search) || search == null);
            var paginated = PaginatedList<BedType>.Create(query, page, size);
            return new PaginatedList<BedTypeGetDto>(_mapper.Map<List<BedTypeGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }
    }
}

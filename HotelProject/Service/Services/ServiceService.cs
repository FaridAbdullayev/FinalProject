using AutoMapper;
using Core.Entities;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Service.Dtos;
using Service.Dtos.Branch;
using Service.Dtos.Service;
using Service.Services.Interfaces;
using static Service.Exceptions.ResetException;

namespace Services.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;
        public ServiceService(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }
        public int Create(ServiceCreateDto createDto)
        {
            if (_serviceRepository.Exists(x => x.Name == createDto.Name && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Service already taken");

            Core.Entities.Service service = _mapper.Map<Core.Entities.Service>(createDto);

            _serviceRepository.Add(service);

            _serviceRepository.Save();

            return service.Id;


        }

        public void Delete(int id)
        {
            Core.Entities.Service entity = _serviceRepository.Get(x => x.Id == id);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Service not found");

            entity.IsDeleted = true;
            entity.UpdateAt = DateTime.Now;
            _serviceRepository.Save();
        }

        public List<ServiceListItemGetDto> GetAll()
        {
            return _mapper.Map<List<ServiceListItemGetDto>>(_serviceRepository.GetAll(x => !x.IsDeleted)).ToList();
        }

        public PaginatedList<ServiceGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _serviceRepository.GetAll(x => !x.IsDeleted && (search == null || x.Name.Contains(search)));
            var paginated = PaginatedList<Core.Entities.Service>.Create(query, page, size);
            return new PaginatedList<ServiceGetDto>(_mapper.Map<List<ServiceGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public ServiceGetDto GetById(int id)
        {
            Core.Entities.Service entity = _serviceRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Service not found");

            return _mapper.Map<ServiceGetDto>(entity);
        }

        public void Update(ServiceUpdateDto updateDto, int id)
        {
            Core.Entities.Service entity = _serviceRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Service not found");


            if (entity.Name != updateDto.Name && _serviceRepository.Exists(x => x.Name == updateDto.Name && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Service already taken");


            entity.Name = updateDto.Name;
            entity.UpdateAt = DateTime.Now;
            _serviceRepository.Save();
        }
    }
}

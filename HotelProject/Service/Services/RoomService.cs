using AutoMapper;
using Core.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Pustok.Helpers;
using Service.Dtos;
using Service.Dtos.Room;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;

namespace Service.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repo;
        public readonly IServiceRepository _serviceRepo;
        public readonly IBranchRepository _branchRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public RoomService(IRoomRepository roomRepository,IServiceRepository serviceRepository,IMapper mapper,IWebHostEnvironment webHostEnvironment ,IBranchRepository branchRepository)
        {
            _env = webHostEnvironment;
            _repo = roomRepository;
            _serviceRepo = serviceRepository;
            _mapper = mapper;
            _branchRepo = branchRepository;
        }


        public int Create(RoomCreateDto createDto)
        {
            List<Core.Entities.Service> services = new List<Core.Entities.Service>();

            Branch branch = _branchRepo.Get(x => x.Id == createDto.BranchId && !x.IsDeleted);
            if (branch == null)
                throw new RestException(StatusCodes.Status404NotFound, "BranchId", "Branch not found by given BranchId");

            var serviceIds = createDto.ServiceIds?.ToList();

            if (serviceIds != null)
            {
                services = _serviceRepo.GetAll(x => serviceIds.Contains(x.Id)).ToList();
            }

            if (serviceIds == null || services.Count == 0)
            {
                throw new RestException(StatusCodes.Status404NotFound, "ServiceId", "One or more service not found by given Ids");
            }

            if (_serviceRepo.Exists(x => x.Name.ToUpper() == createDto.Name.ToUpper() && !x.IsDeleted))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Room already exists by given Name");
            }

            var roomCategories = createDto.ServiceIds.Select(x => new Core.Entities.RoomService { ServiceId = x }).ToList();


            Room room = new()
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                Area = createDto.Area,
                MaxAdultsCount = createDto.MaxAdultsCount,
                MaxChildrenCount = createDto.MaxChildrenCount,
                BedTypeId = createDto.BedTypeId,
                //BedTypeId = createDto.BedType,
                BranchId = createDto.BranchId,
                RoomServices = roomCategories,
                Images = new List<RoomImage>()
            };

            foreach (var file in createDto.Images)
            {
                var filePath = FileManager.Save(file, _env.WebRootPath, "uploads/room");
                room.Images.Add(new RoomImage { Image = filePath });
            }


            _repo.Add(room);
            _repo.Save();

            return room.Id;

        }


        
        public void Delete(int id)
        {
            Room entity = _repo.Get(x=>x.Id == id && !x.IsDeleted);

            if(entity == null) throw new RestException(StatusCodes.Status404NotFound, "Room not found");

            entity.IsDeleted = true;
            entity.UpdateAt = DateTime.Now;
            _repo.Save();
        }

        public List<RoomListItemGetDto> GetAll()
        {
            return _mapper.Map<List<RoomListItemGetDto>>(_repo.GetAll(x => !x.IsDeleted).ToList());
        }

        public PaginatedList<RoomGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _repo.GetAll(x => !x.IsDeleted && (search == null || x.Name.Contains(search)));
            var paginated = PaginatedList<Room>.Create(query, page, size);
            return new PaginatedList<RoomGetDto>(_mapper.Map<List<RoomGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public RoomGetDto GetById(int id)
        {
            Room entity = _repo.Get(x => x.Id == id && !x.IsDeleted, "Images", "RoomServices");

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Room not found");

            return _mapper.Map<RoomGetDto>(entity);
        }

        public void Update(RoomUpdateDto updateDto, int Id)
        {
            throw new NotImplementedException();
        }
    }
}

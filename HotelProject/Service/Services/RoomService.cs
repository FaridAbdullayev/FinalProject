using AutoMapper;
using Core.Entities;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pustok.Helpers;
using Service.Dtos;
using Service.Dtos.Room;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public readonly IBedTypeRepository _bedTypeRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public RoomService(IRoomRepository roomRepository,IServiceRepository serviceRepository,IMapper mapper,IWebHostEnvironment webHostEnvironment ,IBranchRepository branchRepository,IBedTypeRepository bedTypeRepository)
        {
            _env = webHostEnvironment;
            _repo = roomRepository;
            _serviceRepo = serviceRepository;
            _mapper = mapper;
            _branchRepo = branchRepository;
            _bedTypeRepo = bedTypeRepository;
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

        public void Update(RoomUpdateDto updateDto, int id)
        {
            List<Core.Entities.Service> service = new List<Core.Entities.Service>();

            var serviceIds = updateDto.ServiceIds?.ToList();

            if (serviceIds != null)
            {
                service = _serviceRepo.GetAll(x => serviceIds.Contains(x.Id)).ToList();
            }
            if (serviceIds == null || service.Count == 0)
            {
                throw new RestException(StatusCodes.Status404NotFound, "ServiceId", "One or more services not found by given Ids");
            }


            Room entity = _repo.Get(x => x.Id == id && !x.IsDeleted, "RoomServices","Images");

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Room not found");


            if (entity.Name != updateDto.Name && _repo.Exists(x => x.Name == updateDto.Name && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Room already taken");



            Branch branch = _branchRepo.Get(x => x.Id == updateDto.BranchId && !x.IsDeleted);
            if (branch == null)
                throw new RestException(StatusCodes.Status404NotFound, "BranchId", "Branch not found");

            BedType bed = _bedTypeRepo.Get(x => x.Id == updateDto.BedTypeId);

            if(bed == null)
                throw new RestException(StatusCodes.Status404NotFound, "BedTypeId", "BedType not found");




            List<RoomImage> data = entity.Images.Where(x => updateDto.RoomImageIds.Contains(x.Id)).ToList();
            List<RoomImage> removedImages = entity.Images.Where(x => !updateDto.RoomImageIds.Contains(x.Id)).ToList();

            entity.Images = data;

            foreach (var imgFile in updateDto.Images)
            {
                RoomImage bookImg = new RoomImage
                {
                    Image = FileManager.Save(imgFile, _env.WebRootPath, "uploads/room"),
                };
                entity.Images.Add(bookImg);
            }


            var roomService = updateDto.ServiceIds.Select(x => new Core.Entities.RoomService { ServiceId = x }).ToList();


            entity.Name = updateDto.Name;
            entity.Description = updateDto.Description;
            entity.BranchId = updateDto.BranchId;
            entity.Price = updateDto.Price;
            entity.BedTypeId = updateDto.BedTypeId;
            entity.UpdateAt = DateTime.Now;
            entity.MaxAdultsCount = updateDto.MaxAdultsCount;
            entity.MaxChildrenCount = updateDto.MaxChildrenCount;
            entity.RoomServices = roomService;

            foreach (var item in removedImages)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/room", item.Image);
            }


            _repo.Save();
        }
        public async Task<List<RoomGetDto>> GetFilteredRoomsAsync(RoomFilterCriteriaDto criteriaDto)
        {
            var allRooms = await _repo.GetAll(x => true)
                .Include(r => r.RoomServices)
                .Include(r => r.Branch)
                .Include(r => r.Images)
                .ToListAsync();

            var reservedRoomIds = GetReservedRoomIds(criteriaDto.StartDate, criteriaDto.EndDate); // Assuming this method exists and fetches reserved room IDs based on date

            var filteredRooms = allRooms
                .Where(r => !reservedRoomIds.Contains(r.Id))
                .Where(r => !criteriaDto.BranchId.HasValue || r.BranchId == criteriaDto.BranchId.Value) // Filter by branch
                .Where(r => !criteriaDto.ServiceIds.Any() || r.RoomServices.Any(rs => criteriaDto.ServiceIds.Contains(rs.ServiceId))) // Filter by services
                .ToList();

            foreach (var room in filteredRooms)
            {
                room.Price = CalculateRoomPrice(room, criteriaDto.StartDate, criteriaDto.EndDate);
            }

            return _mapper.Map<List<RoomGetDto>>(filteredRooms);
        }
        public List<int> GetReservedRoomIds(DateTime startDate, DateTime endDate)
        {
            
            return new List<int>();
        }
        public double CalculateRoomPrice(Room room, DateTime startDate, DateTime endDate)
        {
            var numberOfDays = (endDate - startDate).TotalDays;
            return room.Price * numberOfDays;
        }



      

        
    }
}

﻿using AutoMapper;
using Core.Entities;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IReservationRepository _reservationRepo;
        public RoomService(IRoomRepository roomRepository, IServiceRepository serviceRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment, IBranchRepository branchRepository, IBedTypeRepository bedTypeRepository, UserManager<AppUser> userManager, IReservationRepository reservationRepo)
        {
            _env = webHostEnvironment;
            _repo = roomRepository;
            _serviceRepo = serviceRepository;
            _mapper = mapper;
            _branchRepo = branchRepository;
            _bedTypeRepo = bedTypeRepository;
            _userManager = userManager;
            _reservationRepo = reservationRepo;
        }











        public RoomPreReservationInfoDto RoomPreReservationInfo(int roomId, DateTime checkIn, DateTime checkOut)
        {
            Room room = _repo.Get(x => x.Id == roomId && !x.IsDeleted,"Images","Branch");

            if (room == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Room not found");
            }

            if (checkOut <= checkIn)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "CheckOut", "Check-out date must be later than check-in date.");
            }

            int nights = (checkOut - checkIn).Days;

            double totalPrice = room.Price * nights;

            int branchId = room.BranchId;

            string firstImageUrl = room.Images?.FirstOrDefault()?.Image ?? string.Empty;

            RoomPreReservationInfoDto info = new RoomPreReservationInfoDto
            {
                RoomName = room.Name,
                BrnachName = room.Branch.Name,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Nights = nights,
                TotalPrice = totalPrice,
                AdultsCount = room.MaxAdultsCount,
                ChildrenCount = room.MaxChildrenCount,
                ImageUrl = $"https://localhost:7119//uploads/room/"+firstImageUrl,
            };

            return info;
        }
        public async Task<List<MemberRoomGetDto>> GetFilteredRoomsAsync(RoomFilterCriteriaDto criteriaDto)
        {
            if (criteriaDto.StartDate < DateTime.Today)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "StartDate", "StartDate cannot be in the past.");
            }
            if (criteriaDto.EndDate <= criteriaDto.StartDate)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "EndDate", "EndDate cannot be earlier than or equal to StartDate.");
            }
            if (criteriaDto.MaxAdultsCount <= 0)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "AdultsCount", "AdultsCount must be greater than zero.");
            }
            if (criteriaDto.MaxChildrenCount < 0)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "ChildrenCount", "ChildrenCount cannot be negative.");
            }

            //int nights = (criteriaDto.EndDate - criteriaDto.StartDate).Days;

            //double totalPrice = criteriaDto.Price * nights;

            var allRooms = _repo.GetAll(x => !x.IsDeleted, "RoomServices", "Branch", "Images").ToList();

            var reservedRoomIds = GetReservedRoomIds(criteriaDto.StartDate, criteriaDto.EndDate);

            var filteredRooms = allRooms
                .Where(r => !reservedRoomIds.Contains(r.Id)) 
                .Where(r => !criteriaDto.BranchId.HasValue || r.BranchId == criteriaDto.BranchId.Value)
                .Where(r => criteriaDto.ServiceIds == null || !criteriaDto.ServiceIds.Any() || r.RoomServices.Any(rs => criteriaDto.ServiceIds.Contains(rs.ServiceId)))
                .Where(r => (!criteriaDto.MinPrice.HasValue || r.Price >= criteriaDto.MinPrice.Value) &&
                            (!criteriaDto.MaxPrice.HasValue || r.Price <= criteriaDto.MaxPrice.Value))
                .Where(r => r.MaxAdultsCount == criteriaDto.MaxAdultsCount && r.MaxChildrenCount == criteriaDto.MaxChildrenCount)
                .ToList();

            return _mapper.Map<List<MemberRoomGetDto>>(filteredRooms);
        }
        public List<int> GetReservedRoomIds(DateTime startDate, DateTime endDate)
        {
            return _reservationRepo.GetAll(reservation =>
                 reservation.StartDate <= endDate && reservation.EndDate >= startDate)
                .Select(reservation => reservation.RoomId)
                .ToList();
        }


















        public int Create(RoomCreateDto createDto)
        {
            List<Core.Entities.Service> services = new List<Core.Entities.Service>();

            Branch branch = _branchRepo.Get(x => x.Id == createDto.BranchId && !x.IsDeleted);
            if (branch == null)
                throw new RestException(StatusCodes.Status404NotFound, "BranchId", "Branch not found");

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
            Room entity = _repo.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Room not found");

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


            Room entity = _repo.Get(x => x.Id == id && !x.IsDeleted, "RoomServices", "Images");

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Room not found");


            if (entity.Name != updateDto.Name && _repo.Exists(x => x.Name == updateDto.Name && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Room already taken");



            Branch branch = _branchRepo.Get(x => x.Id == updateDto.BranchId && !x.IsDeleted);
            if (branch == null)
                throw new RestException(StatusCodes.Status404NotFound, "BranchId", "Branch not found");

            BedType bed = _bedTypeRepo.Get(x => x.Id == updateDto.BedTypeId);

            if (bed == null)
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
        public List<MemberReviewGetDto> GetRoomReviews(int roomId)
        {
            var room = _repo.Get(x => x.Id == roomId && !x.IsDeleted,"Reviews.AppUser");

            if (room == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Room", "Room not found");
            }
            var reviews = room.Reviews.Where(x => x.Status == Core.Entities.Enum.ReviewStatus.Accepted);

            return _mapper.Map<List<MemberReviewGetDto>>(reviews);
        }
        public PaginatedList<MemberRoomGetDto> UserRoomGetAll(string? search = null, int page = 1, int size = 10)
        {
            var query = _repo.GetAll(x => !x.IsDeleted && (search == null || x.Name.Contains(search)),"Images", "RoomServices");
            var paginated = PaginatedList<Room>.Create(query, page, size);
            return new PaginatedList<MemberRoomGetDto>(_mapper.Map<List<MemberRoomGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public MemberRoomDetailGetDto UserGetById(int id)
        {
            Room entity = _repo.Get(x => x.Id == id && !x.IsDeleted, "Images", "RoomServices");

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Room not found");

            return _mapper.Map<MemberRoomDetailGetDto>(entity);
        }
    }
}

using Moq;
using Xunit;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;
using Data.Repositories.Interfaces;
using Service.Dtos;
using Service.Services;
using Service.Services.Interfaces;
using Service.Dtos.Room;
using Service.Dtos.Users;
using static Service.Exceptions.ResetException;

namespace HotelProject.Test
{
    public class RoomServiceTest
    {
        private readonly Mock<IRoomRepository> _mockRoomRepo;
        private readonly Mock<IServiceRepository> _mockServiceRepo;
        private readonly Mock<IBranchRepository> _mockBranchRepo;
        private readonly Mock<IBedTypeRepository> _mockBedTypeRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<IReservationRepository> _mockReservationRepo;
        private readonly Service.Services.RoomService _roomService;

        public RoomServiceTest()
        {
            _mockRoomRepo = new Mock<IRoomRepository>();
            _mockServiceRepo = new Mock<IServiceRepository>();
            _mockBranchRepo = new Mock<IBranchRepository>();
            _mockBedTypeRepo = new Mock<IBedTypeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockUserManager = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null
            );
            _mockReservationRepo = new Mock<IReservationRepository>();

            _roomService = new Service.Services.RoomService(
                _mockRoomRepo.Object,
                _mockServiceRepo.Object,
                _mockMapper.Object,
                _mockEnv.Object,
                _mockBranchRepo.Object,
                _mockBedTypeRepo.Object,
                _mockUserManager.Object,
                _mockReservationRepo.Object
            );
        }

        [Fact]
        public void RoomPreReservationInfo_ShouldReturnExpectedResult()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                Price = 100,
                MaxAdultsCount = 2,
                MaxChildrenCount = 1,
                BranchId = 1
            };

            _mockRoomRepo.Setup(repo => repo.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns(room);

            var checkIn = DateTime.Today;
            var checkOut = checkIn.AddDays(2);

            // Act
            var result = _roomService.RoomPreReservationInfo(1, checkIn, checkOut);

            // Assert
            Assert.Equal(2, result.Nights);
            Assert.Equal(200, result.TotalPrice);
            Assert.Equal(2, result.AdultsCount);
            Assert.Equal(1, result.ChildrenCount);
            Assert.Equal(1, result.BranchId);
        }

        [Fact]
        public async Task GetFilteredRoomsAsync_ShouldReturnFilteredRooms()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { Id = 1, Price = 100, MaxAdultsCount = 2, MaxChildrenCount = 1, BranchId = 1, RoomServices = new List<Core.Entities.RoomService>() },
                new Room { Id = 2, Price = 150, MaxAdultsCount = 3, MaxChildrenCount = 2, BranchId = 2, RoomServices = new List<Core.Entities.RoomService>() }
            }.AsQueryable();

            _mockRoomRepo.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Room, bool>>>(), "RoomServices", "Branch", "Images"))
                .Returns(rooms);

            var criteria = new RoomFilterCriteriaDto
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                MaxAdultsCount = 2,
                MaxChildrenCount = 1
            };

            _mockMapper.Setup(mapper => mapper.Map<List<MemberRoomGetDto>>(It.IsAny<List<Room>>()))
                .Returns(new List<MemberRoomGetDto>());

            // Act
            var result = await _roomService.GetFilteredRoomsAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<MemberRoomGetDto>>(result);
        }
        //[Fact]
        //public void Create_ShouldThrowExceptionWhenBranchNotFound()
        //{
        //    // Arrange
        //    var createDto = new RoomCreateDto
        //    {
        //        BranchId = 1,
        //        Name = "Room1",
        //        ServiceIds = new List<int> { 1 }
        //    };

        //    // Mock ayarlaması: Branch repository, null dönecek
        //    _mockBranchRepo.Setup(repo => repo.Get(It.IsAny<Expression<Func<Branch, bool>>>()))
        //        .Returns<Branch>(null);

        //    // Act & Assert
        //    var exception = Assert.Throws<RestException>(() => _roomService.Create(createDto));


        //    Console.WriteLine(exception.Message);

        //    // Assert'lerin her birini ayrı kontrol edin
        //    Assert.NotNull(exception);
        //    Assert.Equal(StatusCodes.Status404NotFound, exception.Code); // Doğru status kodunu doğrula
        //    Assert.Equal("Branch not found", exception.Message); // Hata mesajını doğrula
        //}

        [Fact]
        public void Delete_ShouldMarkRoomAsDeleted()
        {
            // Arrange
            var room = new Room { Id = 1, IsDeleted = false };
            _mockRoomRepo.Setup(repo => repo.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns(room);
            _mockRoomRepo.Setup(repo => repo.Save());

            // Act
            _roomService.Delete(1);

            // Assert
            Assert.True(room.IsDeleted);
            _mockRoomRepo.Verify(repo => repo.Save(), Times.Once);
        }
    }
}

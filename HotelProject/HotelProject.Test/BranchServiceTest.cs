using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Core.Entities;
using Core.Entities.Enum;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Service.Dtos;
using Service.Dtos.Branch;
using Service.Dtos.Users;
using Service.Exceptions;
using Service.Services;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
using static Service.Exceptions.ResetException;

namespace Service.Tests
{
    public class BranchServiceTest
    {
        private readonly Mock<IBranchRepository> _branchRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly BranchService _branchService;
        public BranchServiceTest()
        {
            _branchRepository = new Mock<IBranchRepository>();
            _mapper = new Mock<IMapper>();
            _branchService = new BranchService(_branchRepository.Object, _mapper.Object);
        }
        [Fact]
        public void Create_BranchNameAlreadyExists_ThrowsRestException()
        {
            // Arrange
            var createDto = new BranchCreateDto { Name = "ExistingBranchName" };
            _branchRepository.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Branch, bool>>>()))
                .Returns(true);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _branchService.Create(createDto));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.NotEmpty(exception.Errors);
            var error = exception.Errors.First();
            Assert.Equal("Name", error.Key);
            Assert.Equal("Branch already taken", error.Message);
        }
        [Fact]
        public void Create_Success_ReturnsBranchId()
        {
            // Arrange
            var createDto = new BranchCreateDto { Name = "NewBranch" };
            var branch = new Branch { Name = createDto.Name, Id = 1 };
            _mapper.Setup(m => m.Map<Branch>(createDto)).Returns(branch);
            _branchRepository.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Branch, bool>>>())).Returns(false);
            _branchRepository.Setup(repo => repo.Add(It.IsAny<Branch>())).Callback<Branch>(b => b.Id = 1);
            _branchRepository.Setup(repo => repo.Save());

            // Act
            var result = _branchService.Create(createDto);

            // Assert
            Assert.Equal(1, result);
        }
        [Fact]
        public void Delete_BranchExists_Success()
        {
            // Arrange
            int branchId = 1;
            var branch = new Branch { Id = branchId };
            _branchRepository.Setup(repo => repo.Get(It.IsAny<Expression<Func<Branch, bool>>>())).Returns(branch);
            _branchRepository.Setup(repo => repo.Save());

            // Act
            _branchService.Delete(branchId);

            // Assert
            _branchRepository.Verify(repo => repo.Save(), Times.Once);
        }
        [Fact]
        public void Delete_BranchDoesNotExist_ThrowsRestException()
        {
            // Arrange
            int branchId = 1;
            _branchRepository.Setup(repo => repo.Get(It.IsAny<Expression<Func<Branch, bool>>>())).Returns((Branch)null);

            // Act
            var exception = Assert.Throws<RestException>(() => _branchService.Delete(branchId));

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Equal("Branch not found", exception.Message);
        }
        [Fact]
        public void GetAllByPage_ReturnsPaginatedBranches()
        {
            // Arrange
            var branches = new List<Branch>
            {
                new Branch { Id = 1, Name = "Branch1" },
                new Branch { Id = 2, Name = "Branch2" }
            };
            var branchDtos = new List<BranchGetDto>
            {
                new BranchGetDto { Id = 1, Name = "Branch1" },
                new BranchGetDto { Id = 2, Name = "Branch2" }
            };
            var paginatedList = new PaginatedList<Branch>(branches, 1, 1, 10);

            _branchRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Branch, bool>>>())).Returns(branches.AsQueryable());
            _mapper.Setup(m => m.Map<List<BranchGetDto>>(paginatedList.Items)).Returns(branchDtos);

            // Act
            var result = _branchService.GetAllByPage();

            // Assert
            Assert.Equal(branchDtos.Count, result.Items.Count);
            Assert.Equal(1, result.TotalPages);
        }
    }
}

using AutoMapper;
using Core.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Pustok.Helpers;
using Service.Dtos;
using Service.Dtos.Slider;
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
    public class SliderService : ISliderService
    {
        private readonly ISliderRepository _slider;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public SliderService(ISliderRepository appDbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _slider = appDbContext;
            _mapper = mapper;
            _env = webHostEnvironment;
        }
        public int Create(SliderCreateDto createDto)
        {
            Slider slider = new Slider
            {
                Description = createDto.Description,
                Order = createDto.Order,
                Image = createDto.File.Save(_env.WebRootPath, "uploads/slider")
            };

            _slider.Add(slider);
            _slider.Save();
            return slider.Id;
        }

        public void Delete(int id)
        {
            Slider entity = _slider.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Slider not found");

            entity.IsDeleted = true;
            entity.UpdateAt = DateTime.Now;
            _slider.Save();
        }

        public List<SliderListItemGetDto> GetAll()
        {
            return _mapper.Map<List<SliderListItemGetDto>>(_slider.GetAll(x => !x.IsDeleted).ToList());
        }

        public PaginatedList<SliderGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _slider.GetAll(x => !x.IsDeleted && (search == null || x.Description.Contains(search)));
            var paginated = PaginatedList<Slider>.Create(query, page, size);
            return new PaginatedList<SliderGetDto>(_mapper.Map<List<SliderGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public SliderGetDto GetById(int id)
        {
            Slider entity = _slider.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Slider not found");

            return _mapper.Map<SliderGetDto>(entity);
        }

        public void Update(SliderUpdateDto updateDto, int id)
        {
            Slider entity = _slider.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Slider not found");

  


            string deletedFile = null;
            if (updateDto.File != null)
            {
                deletedFile = entity.Image;
                entity.Image = FileManager.Save(updateDto.File, _env.WebRootPath, "uploads/slider");

            }


            entity.Description = updateDto.Description;
            entity.Order = updateDto.Order;
            entity.UpdateAt = DateTime.Now;

            if (deletedFile != null)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/slider", deletedFile);
            }


            _slider.Save();
        }


        public List<SliderGetDtoForUser> GetAllSliderForUser()
        {
            return _mapper.Map<List<SliderGetDtoForUser>>(_slider.GetAll(x => !x.IsDeleted).ToList());
        }
    }
}

using Service.Dtos.Slider;
using Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.Users;

namespace Service.Services.Interfaces
{
    public interface ISliderService
    {
        int Create(SliderCreateDto createDto);
        SliderGetDto GetById(int id);
        void Update(SliderUpdateDto updateDto, int Id);
        void Delete(int id);
        List<SliderListItemGetDto> GetAll();
        PaginatedList<SliderGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        public List<SliderGetDtoForUser> GetAllSliderForUser();

    }
}

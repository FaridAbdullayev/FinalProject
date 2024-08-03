using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Pustok.Helpers;
using Service.Dtos.Branch;
using Service.Dtos.OurStaff;
using Service.Dtos.Room;
using Service.Dtos.Service;
using Service.Dtos.Slider;
using Service.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Profiles
{
    public class MapProfile:Profile
    {
        private readonly IHttpContextAccessor _context;
        private readonly IWebHostEnvironment _env;

        public MapProfile(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {

            _context = httpContextAccessor;
            _env = webHostEnvironment;

            var uriBuilder = new UriBuilder(_context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host.Host, _context.HttpContext.Request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }
            string baseUrl = uriBuilder.Uri.AbsoluteUri;


            CreateMap<Branch, BranchGetDto>();
            CreateMap<BranchCreateDto, Branch>();
            CreateMap<Branch,BranchListItemGetDto>();


            CreateMap<Core.Entities.Service, ServiceGetDto>();
            CreateMap<ServiceCreateDto, Core.Entities.Service>();
            CreateMap<Core.Entities.Service,ServiceListItemGetDto>();

            CreateMap<OurStaff, OurStaffListItemGetDto>()
             .ForMember(dest => dest.ImageUrl, s => s.MapFrom(s => baseUrl + "uploads/staff/" + s.Image));
            CreateMap<OurStaff, OurStaffGetDto>()
               .ForMember(dest => dest.ImageUrl, s => s.MapFrom(s => baseUrl + "uploads/staff/" + s.Image));


            CreateMap<Slider, SliderListItemGetDto>()
          .ForMember(dest => dest.ImageUrl, s => s.MapFrom(s => baseUrl + "uploads/slider/" + s.Image));
            CreateMap<Slider, SliderGetDto>()
               .ForMember(dest => dest.ImageUrl, s => s.MapFrom(s => baseUrl + "uploads/slider/" + s.Image));





            CreateMap<Room, RoomListItemGetDto>();

            CreateMap<RoomImage, RoomImageGetDto>()
             .ForMember(dest => dest.Image, s => s.MapFrom(s => baseUrl + "uploads/room/" + s.Image));

            CreateMap<Room, RoomGetDto>()
               .ForMember(dest => dest.ServiceIds,
                opt => opt.MapFrom(src => src.RoomServices.Select(rc => rc.ServiceId).ToList()));



            CreateMap<AppUser, AdminGetDto>();

            CreateMap<AppUser, AdminPaginatedGetDto>();
            // CreateMap<RoomCreateDto, Room>()
            //.ForMember(dest => dest.Images, opt => opt
            //    .MapFrom(src => src.Images
            //        .Select(x => new RoomImage
            //        {
            //            Image = FileManager.Save(x, _env.WebRootPath, "uploads/room"),
            //            IsMain = true
            //        }).ToList()
            //    )
            //);
        }
    }
}

using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Pustok.Helpers;
using Service.Dtos.Branch;
using Service.Dtos.Slider;
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
           
        }
    }
}

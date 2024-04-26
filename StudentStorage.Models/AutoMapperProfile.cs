using AutoMapper;
using StudentStorage.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CourseRequestDTO, Course>();
            CreateMap<CourseResponseDTO, Course>();
            CreateMap<ApplicationUser, UserDTO>();
        }
    }
}

using AutoMapper;
using StudentStorage.Models.DTO;
using StudentStorage.Models.DTO.Course;
using StudentStorage.Models.DTO.Request;
using StudentStorage.Models.DTO.Solution;
using StudentStorage.Models.DTO.User;

namespace StudentStorage.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<Course, CourseResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));

            CreateMap<Request, RequestResponseDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<Assignment, AssignmentResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.AllowLateSubmissions, opt => opt.MapFrom(src => src.AllowLateSubmissions))
                .ForMember(dest => dest.Hidden, opt => opt.MapFrom(src => src.Hidden));

            CreateMap<Course, CourseDetailResponseDTO>()
                .IncludeBase<Course, CourseResponseDTO>()
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments));

            CreateMap<Request, JoinRequestDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        }
    }
}

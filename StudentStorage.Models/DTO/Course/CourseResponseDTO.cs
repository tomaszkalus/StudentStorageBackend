﻿using StudentStorage.Models.DTO.User;

namespace StudentStorage.Models.DTO.Course
{
    public class CourseResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UserDTO Creator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

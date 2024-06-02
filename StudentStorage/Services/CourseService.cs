using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.Services
{
    public class CourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FileManagerService _fileManagerService;
        private readonly DirectoryService _directoryService;

        public CourseService(IUnitOfWork unitOfWork, FileManagerService fileManagerService, DirectoryService directoryService)
        {
            _unitOfWork = unitOfWork;
            _fileManagerService = fileManagerService;
            _directoryService = directoryService;
        }

        public async Task<ServiceResult> CreateNewCourse(Course course)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Course.AddAsync(course);
                await _unitOfWork.CommitAsync();

                var createCourseResult = _directoryService.CreateCourseDirectory(course);
                if (!createCourseResult.Success)
                {
                    await _unitOfWork.Course.RemoveAsync(course);
                    await _unitOfWork.CommitAsync();
                    return createCourseResult;
                }

                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.Rollback();
                throw;
            }
            return new ServiceResult(true, "Course created successfully.");
        }
    }
}

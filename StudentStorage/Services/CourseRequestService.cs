using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.Enums;

namespace StudentStorage.Services
{
    public class CourseRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FileManagerService _fileManagerService;

        public CourseRequestService(IUnitOfWork unitOfWork, FileManagerService fileManagerService)
        {
            _unitOfWork = unitOfWork;
            _fileManagerService = fileManagerService;
        }

        public async Task<ServiceResult> ApproveCourseRequestAsync(Request request)
        {
            try
            {
                request.Course.Students.Add(request.User);
                await _unitOfWork.Request.UpdateAsync(request);
                await _unitOfWork.Course.UpdateAsync(request.Course);
            }
            catch (Exception ex)
            {
                await _unitOfWork.DisposeAsync();
                return new ServiceResult(false, ex.Message);
            }

            ServiceResult result = _fileManagerService.CreateStudentDirectory(request.Course, request.User);
            if (!result.Success)
            {
                await _unitOfWork.DisposeAsync();
                return new ServiceResult(false, result.Message);
            }

            await _unitOfWork.SaveAsync();
            return new ServiceResult(true, "User directory created successfully.");
        }


        public async Task<ServiceResult> UpdateRequestStatus(Request request, CourseRequestStatus status)
        {
            if (request == null)
            {
                return new ServiceResult(false, "Request cannot be null.");
            }
            if (await _unitOfWork.User.IsCourseMemberAsync(request.UserId, request.CourseId))
            {
                return new ServiceResult(false, "User is already a member of the course.");
            }
            if (request.Status != CourseRequestStatus.Pending)
            {
                return new ServiceResult(false, "Only pending requests can be updated.");
            }

            request.Status = status;

            try
            {
                if (status == CourseRequestStatus.Approved)
                {
                    await ApproveCourseRequestAsync(request);
                }
                else if (status == CourseRequestStatus.Denied)
                {
                    await _unitOfWork.Request.UpdateAsync(request);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
            return new ServiceResult(true, "");
        }
    }
}

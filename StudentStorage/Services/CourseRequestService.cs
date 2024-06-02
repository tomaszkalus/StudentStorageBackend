using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.Enums;

namespace StudentStorage.Services
{
    public class CourseRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FileManagerService _fileManagerService;
        private readonly DirectoryService _directoryService;

        public CourseRequestService(IUnitOfWork unitOfWork, FileManagerService fileManagerService, DirectoryService directoryService)
        {
            _unitOfWork = unitOfWork;
            _fileManagerService = fileManagerService;
            _directoryService = directoryService;
        }

        private async Task<ServiceResult> ApproveCourseRequestAsync(Request request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                request.Course.Students.Add(request.User);
                await _unitOfWork.Request.UpdateAsync(request);
                await _unitOfWork.Course.UpdateAsync(request.Course);
                await _unitOfWork.CommitAsync();

                try
                {
                    _directoryService.CreateStudentDirectory(request.Course, request.User);

                }
                catch(Exception ex)
                {
                    await _unitOfWork.Rollback();
                    return new ServiceResult(false, ex.Message);
                }
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.Rollback();
                throw;
            }

            return new ServiceResult(true, "User directory created successfully.");
        }

        private async Task<ServiceResult> ValidateStatusUpdate(Request request)
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
            return new ServiceResult(true, "");
        }


        public async Task<ServiceResult> UpdateRequestStatus(Request request, CourseRequestStatus status)
        {
            ServiceResult validationResult = await ValidateStatusUpdate(request);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            try
            {
                request.Status = status;
                if (status == CourseRequestStatus.Approved)
                {
                    return await ApproveCourseRequestAsync(request);
                }
                else if (status == CourseRequestStatus.Denied)
                {
                    await _unitOfWork.Request.UpdateAsync(request);
                    await _unitOfWork.CommitAsync();
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

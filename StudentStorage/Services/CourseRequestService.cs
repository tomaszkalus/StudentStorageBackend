using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.Enums;

namespace StudentStorage.Services
{
    public class CourseRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                    request.Course.Students.Add(request.User);
                    await _unitOfWork.ApproveCourseRequestTransactionAsync(request);
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

using BookStoreMVC.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.Enums;
using StudentStorage.Models.Exceptions;

namespace StudentStorage.Services
{
    public class CourseRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task ApproveRequest(Request request)
        {
            request.Status = CourseRequestStatus.Approved;
            request.Course.Students.Add(request.User);
            await _unitOfWork.ApproveCourseRequestTransactionAsync(request);
        }

        private async Task DeclineRequest(Request request)
        {
            request.Status = CourseRequestStatus.Denied;
            await _unitOfWork.Request.UpdateAsync(request);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateRequestStatus(Request request)
        {
            if(request.Status == CourseRequestStatus.Approved)
            {
                await ApproveRequest(request);
            }
            else if(request.Status == CourseRequestStatus.Denied)
            {
                await DeclineRequest(request);
            }
            else
            {
                throw new InvalidStatusException("The status value of Request is invalid.");
            }
        }
    }
}

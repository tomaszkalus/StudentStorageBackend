﻿using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.DTO.Solution;
using StudentStorage.Models.Enums;

namespace StudentStorage.Services
{
    public class AssignmentSolutionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FileManagerService _fileManagerService;

        public AssignmentSolutionService(IUnitOfWork unitOfWork, FileManagerService fileManagerService)
        {
            _unitOfWork = unitOfWork;
            _fileManagerService = fileManagerService;
        }

        public SolutionStatus GetSolutionStatus(IEnumerable<Solution> solutions)
        {
            if (solutions == null || solutions.Count() == 0)
            {
                return SolutionStatus.NotSubmitted;
            }
            var earliestSubmittedSolution = solutions.MinBy(x => x.CreatedAt);

            if (earliestSubmittedSolution.CreatedAt > earliestSubmittedSolution.Assignment.DueDate)
            {
                return SolutionStatus.SubmittedLate;
            }

            return SolutionStatus.Submitted;

        }

        public async Task<ServiceResult> SubmitAssignmentSolutionFilesAsync(SolutionRequestDTO solutionRequest, Assignment assignment, ApplicationUser user)
        {

            var solutions = _fileManagerService.SaveSolutionFiles(solutionRequest.Files, assignment, user);

            if (solutions == null)
            {
                return new ServiceResult(false, "Failed to save solution files.");
            }

            foreach (var solution in solutions)
            {
                await _unitOfWork.Solution.AddAsync(solution);
            }

            await _unitOfWork.CommitAsync();

            return new ServiceResult(true, "Assignment solution submitted successfully.");
        }

        public async Task<ServiceResult> DeleteAssignmentSolutionFileAsync(int solutionId, ApplicationUser user)
        {
            var solution = await _unitOfWork.Solution.GetByIdAsync(solutionId);

            if (solution == null)
            {
                return new ServiceResult(false, "Solution not found.");
            }

            try
            {
                _fileManagerService.DeleteSolutionFile(solution);
            }
            catch (Exception)
            {
                return new ServiceResult(false, "Failed to delete solution file.");
            }

            await _unitOfWork.Solution.RemoveAsync(solution);
            await _unitOfWork.CommitAsync();

            return new ServiceResult(true, "Assignment solution deleted successfully.");
        }

        public IFormFile GetAssignmentSolutionFile(Solution solution)
        {
            return _fileManagerService.GetSolutionFile(solution);
        }

        public IFormFile? GetUserAssignmentSolutionsArchive(IEnumerable<Solution> solutions)
        {
            if (solutions == null || solutions.Count() == 0)
            {
                return null;
            }

            List<IFormFile> solutionFiles = solutions.Select(GetAssignmentSolutionFile).ToList();
            var archiveName = DirectoryNameBuilderService.GenerateAssignmentSolutionArchiveName(solutions.First());
            return _fileManagerService.CreateArchive(solutionFiles, archiveName);
        }
    }
}

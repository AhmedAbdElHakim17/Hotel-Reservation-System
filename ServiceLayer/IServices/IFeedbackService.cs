using HRS_BussinessLogic.DTOs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HRS_ServiceLayer.IServices
{
    public interface IFeedbackService
    {
        Task<ResponseDTO<List<FeedbackDTO>>> GetAllFeedbacksAsync();
        Task<ResponseDTO<List<FeedbackDTO>>> GetMyFeedbacksAsync(ClaimsPrincipal User);
        Task<ResponseDTO<FeedbackDTO>> AddFeedbackAsync(FeedbackDTO feedbackDTO, ClaimsPrincipal claims);
        Task<ResponseDTO<FeedbackDTO>> UpdateFeedbackAsync(FeedbackDTO feedbackDTO, int id);
        Task<ResponseDTO<FeedbackDTO>> DeleteFeedbackAsync(int id);


    }
}

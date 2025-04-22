using AutoMapper;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using HRS_DataAccess;
using HRS_ServiceLayer.IServices;
using System.Security.Claims;

namespace HRS_ServiceLayer.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ResponseDTO<List<FeedbackDTO>>> GetAllFeedbacksAsync()
        {
            try
            {
                var feedbacks = await unitOfWork.Feedbacks.GetAllAsync(nameof(Feedback.User));
                if (feedbacks == null || feedbacks.Count == 0)
                    return new ResponseDTO<List<FeedbackDTO>>("No Feedbacks retrieved", null);
                var resultDTO = mapper.Map<List<FeedbackDTO>>(feedbacks);
                if (resultDTO == null || resultDTO.Count == 0)
                    return new ResponseDTO<List<FeedbackDTO>>("Error while Mapping", null);
                return new ResponseDTO<List<FeedbackDTO>>("Feedbacks retrieved Successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<FeedbackDTO>>($"Error:{ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<List<FeedbackDTO>>> GetMyFeedbacksAsync(ClaimsPrincipal User)
        {
            try
            {
                var user = await unitOfWork.UserManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                    return new ResponseDTO<List<FeedbackDTO>>("User not found", null);
                var feedbacks = await unitOfWork.Feedbacks.FindAllAsync(f => f.UserId == user.Id, nameof(Feedback.User));
                if (feedbacks == null || feedbacks.Count == 0)
                    return new ResponseDTO<List<FeedbackDTO>>("No Feedbacks Retrieved", null);
                var resultDTO = mapper.Map<List<FeedbackDTO>>(feedbacks);
                return new ResponseDTO<List<FeedbackDTO>>("Your Feedbacks retrieved successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<FeedbackDTO>>($"Error: {ex.Message}", null);
            }

        }
        public async Task<ResponseDTO<FeedbackDTO>> AddFeedbackAsync(FeedbackDTO feedbackDTO)
        {
            try
            {
                var user = await unitOfWork.UserManager.FindByNameAsync(feedbackDTO.UserName);
                if (user == null)
                    return new ResponseDTO<FeedbackDTO>("User not found", null);
                var validReservation = await unitOfWork.Reservations
                    .FindAsync(r => r.User == user && r.Id == feedbackDTO.ReservationId);
                if (validReservation == null)
                    return new ResponseDTO<FeedbackDTO>("Invalid ReservationId, please enter a correct one",null);
                var Exist = await unitOfWork.Feedbacks.FindAsync(f => f.ReservationId == validReservation.Id);
                if (Exist != null)
                    return new ResponseDTO<FeedbackDTO>("This Reservation has a feedback already, You can Update the feedback",null);
                var feedback = mapper.Map<Feedback>(feedbackDTO);
                if (feedback == null)
                    return new ResponseDTO<FeedbackDTO>("Error while Mapping", null);
                feedback.UserId = user.Id;
                await unitOfWork.Feedbacks.AddAsync(feedback);
                await unitOfWork.CompleteAsync();
                mapper.Map(feedback,feedbackDTO);
                return new ResponseDTO<FeedbackDTO>("Feedback added Successfully", feedbackDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<FeedbackDTO>($"Error:{ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<FeedbackDTO>> UpdateFeedbackAsync(FeedbackDTO feedbackDTO, int id)
        {
            try
            {
                var feedback = await unitOfWork.Feedbacks.FindAsync(f => f.Id == id);
                if (feedback == null)
                    return new ResponseDTO<FeedbackDTO>("Feedback not found", null);
                if (feedbackDTO.Id != id)
                    return new ResponseDTO<FeedbackDTO>("ID Mismatch", null);
                mapper.Map(feedbackDTO, feedback);
                unitOfWork.Feedbacks.Update(feedback);
                await unitOfWork.CompleteAsync();
                return new ResponseDTO<FeedbackDTO>("Feedback Updated successfully", feedbackDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<FeedbackDTO>($"Error:{ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<FeedbackDTO>> DeleteFeedbackAsync(int id)
        {
            try
            {
                var feedback = await unitOfWork.Feedbacks.FindAsync(f => f.Id == id);
                if (feedback == null)
                    return new ResponseDTO<FeedbackDTO>("Feedback not found", null);
                unitOfWork.Feedbacks.Delete(feedback);
                await unitOfWork.CompleteAsync();
                var resultDTO = mapper.Map<FeedbackDTO>(feedback);
                return new ResponseDTO<FeedbackDTO>("feedback Deleted Successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<FeedbackDTO>($"Error:{ex.Message}", null);
            }
        } 
    }
}

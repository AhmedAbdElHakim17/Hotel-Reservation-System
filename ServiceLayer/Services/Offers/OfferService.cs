using AutoMapper;
using HRS_DataAccess.Models;
using HRS_DataAccess;
using HRS_ServiceLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRS_BussinessLogic.DTOs.Queries;

namespace HRS_ServiceLayer.Services.Offers
{
    public class OfferService : IOfferService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public OfferService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ResponseDTO<List<OfferDTO>>> GetAllOffersAsync()
        {
            try
            {
                var offers = await unitOfWork.Offers.GetAllAsync();
                if (offers == null || offers.Count == 0)
                    return new ResponseDTO<List<OfferDTO>>("No offers found", null);

                var offersDTOs = mapper.Map<List<OfferDTO>>(offers);
                return new ResponseDTO<List<OfferDTO>>("Offers retrieved successfully", offersDTOs);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<OfferDTO>>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<OfferDTO>> AddOfferAsync(OfferDTO offerDTO)
        {
            try
            {
                var offer = mapper.Map<Offer>(offerDTO);
                await unitOfWork.Offers.AddAsync(offer);
                await unitOfWork.CompleteAsync();

                var resultDTO = mapper.Map<OfferDTO>(offer);
                return new ResponseDTO<OfferDTO>("Offer added successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<OfferDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<OfferDTO>> UpdateOfferAsync(int id, OfferDTO offerDTO)
        {
            try
            {
                var offer = await unitOfWork.Offers.FindAsync(r => r.Id == id);
                if (offer == null)
                    return new ResponseDTO<OfferDTO>("offer not found", null);

                if (offerDTO.Id != id)
                    return new ResponseDTO<OfferDTO>("ID mismatch", null);

                mapper.Map(offerDTO, offer);
                unitOfWork.Offers.Update(offer);
                await unitOfWork.CompleteAsync();

                var resultDTO = mapper.Map<OfferDTO>(offer);
                return new ResponseDTO<OfferDTO>("Offer updated successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<OfferDTO>($"Error: {ex.Message}", null);

            }
        }
        public async Task<ResponseDTO<OfferDTO>> DeleteOfferAsync(int id)
        {
            try
            {
                var offer = await unitOfWork.Offers.FindAsync(r => r.Id == id);
                if (offer == null)
                    return new ResponseDTO<OfferDTO>("Offer not found", null);

                unitOfWork.Offers.Delete(offer);
                await unitOfWork.CompleteAsync();

                var resultDTO = mapper.Map<OfferDTO>(offer);
                return new ResponseDTO<OfferDTO>("Room deleted successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<OfferDTO>($"Error: {ex.Message}", null);
            }
        }
    }
}

using HRS_BussinessLogic.DTOs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRS_ServiceLayer.IServices
{
    public interface IOfferService
    {
        Task<ResponseDTO<List<OfferDTO>>> GetAllOffersAsync();
        Task<ResponseDTO<OfferDTO>> AddOfferAsync(OfferDTO offerDTO);
        Task<ResponseDTO<OfferDTO>> UpdateOfferAsync(int id, OfferDTO offerDTO);
        Task<ResponseDTO<OfferDTO>> DeleteOfferAsync(int id);
    }
}

using HRS_BussinessLogic.DTOs.Queries;
using HRS_DataAccess;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRS_ServiceLayer.Services.Offers
{
    public class OfferCleanupService(IUnitOfWork unitOfWork)
    {
        public async Task MarkExpiredOffers()
        {
            var expiredOffers = await unitOfWork.Offers
                .FindAllAsync(o => o.EndDate.Day < DateTime.Now.Day);
            if (expiredOffers == null) return;
            foreach (var offer in expiredOffers)
            {
                unitOfWork.Offers.Delete(offer);
            }
            await unitOfWork.CompleteAsync();
        }
    }
}

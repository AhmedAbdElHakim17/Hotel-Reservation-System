using HRS_ServiceLayer.IServices;

namespace HRS_BussinessLogic.Services
{
    public class HandleExceptionService : IExceptionService
    {
        public HandleExceptionService()
        {
            
        }
        public void HandleException()
        {
            //logger.LogError("Error adding a new Reservation");
            //return StatusCode(500, "Internal Server Error");
        }
    }
}

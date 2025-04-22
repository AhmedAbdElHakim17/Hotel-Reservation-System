namespace HRS_BussinessLogic.DTOs.Queries
{
    public class ResponseDTO<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
        public bool IsSuccess => Data != null;
        public ResponseDTO(string Message, T Data)
        {
            this.Message = Message;
            this.Data = Data;
        }

    }
}

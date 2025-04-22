namespace HRS_SharedLayer.Interfaces.IBases
{
    public interface IBaseValidator<T> where T : class
    {
        bool IsRoomNumberUnique(int num, int roomId);
    }
}

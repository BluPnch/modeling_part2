namespace lab_4.Interfaces
{
    public interface IQueue
    {
        int Count { get; }
        int MaxSize { get; set; }
        int LostRequests { get; }
        void Enqueue(IRequest request);
        IRequest? Dequeue();
        bool IsFull { get; }
    }
}
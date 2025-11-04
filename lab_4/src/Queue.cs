using lab_3.Interfaces;


namespace lab_3
{
    public class Queue : IQueue
    {
        private readonly Queue<IRequest> _queue;

        public int Count => _queue.Count;
        public int MaxSize { get; set; }
        public int LostRequests { get; private set; }

        public bool IsFull => Count >= MaxSize;

        public Queue(int maxSize)
        {
            MaxSize = maxSize;
            _queue = new Queue<IRequest>();
            LostRequests = 0;
        }

        public void Enqueue(IRequest request)
        {
            if (IsFull)
            {
                LostRequests++;
            }
            else
            {
                _queue.Enqueue(request);
            }
        }

        public IRequest? Dequeue()
        {
            return _queue.Count > 0 ? _queue.Dequeue() : null;
        }
    }
}
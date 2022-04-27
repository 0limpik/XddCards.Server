using System.Collections.Generic;
using System.Threading;

namespace XddCards.Server.Base
{
    public class CustomAwaiter
    {
        private List<Awaiter> awaiters = new();

        public void Execute()
        {
            lock (awaiters)
            {
                foreach (var awaiter in awaiters.ToArray())
                {
                    awaiter.Execute();
                    awaiters.Remove(awaiter);
                }
            }
        }

        public Awaiter GetAwaiter()
            => CreateAwaiter();

        public Awaiter SetToken(CancellationToken token)
            => CreateAwaiter().SetToken(token);

        private Awaiter CreateAwaiter()
        {
            var awaiter = new Awaiter();
            awaiters.Add(awaiter);
            return awaiter;
        }
    }

    public class CustomAwaiter<T>
    {
        private List<Awaiter<T>> awaiters = new();

        public void Execute(T result)
        {
            lock (awaiters)
            {
                foreach (var awaiter in awaiters.ToArray())
                {
                    awaiter.Execute(result);
                    awaiters.Remove(awaiter);
                }
            }
        }

        public Awaiter<T> GetAwaiter()
            => CreateAwaiter();

        public Awaiter<T> SetToken(CancellationToken token)
            => CreateAwaiter().SetToken(token);

        private Awaiter<T> CreateAwaiter()
        {
            var awaiter = new Awaiter<T>();
            awaiters.Add(awaiter);
            return awaiter;
        }
    }
}

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace XddCards.Server.Base
{
    public class Awaiter : INotifyCompletion
    {
        private Action continuation;

        private bool hasToken;
        private CancellationToken token;

        public void Execute()
        {
            if (hasToken)
            {
                if (!token.IsCancellationRequested)
                    continuation?.Invoke();
            }
            else
            {
                continuation?.Invoke();
            }
        }

        public bool IsCompleted => false;

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        public Awaiter SetToken(CancellationToken token)
        {
            this.token = token;
            hasToken = true;
            return this;
        }

        public Awaiter GetAwaiter()
            => this;
    }

    public class Awaiter<T> : INotifyCompletion
    {
        private Action continuation;

        private bool hasToken;
        private CancellationToken token;

        private T Result;

        public void Execute(T result)
        {
            Result = result;
            if (hasToken)
            {
                if (!token.IsCancellationRequested)
                    continuation?.Invoke();
            }
            else
            {
                continuation?.Invoke();
            }
        }

        public bool IsCompleted => false;

        public T GetResult() => Result;

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        public Awaiter<T> GetAwaiter()
            => this;

        public Awaiter<T> SetToken(CancellationToken token)
        {
            this.token = token;
            hasToken = true;
            return this;
        }
    }
}

using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading.Tasks;

namespace GStoreServer
{
    class RequestInterceptor : Interceptor
    {
        private readonly int minDelay;
        private readonly int maxDelay;
        private readonly Random r = new Random();

        public RequestInterceptor(int minDelay = 0, int maxDelay = 0)
        {
            if (minDelay > maxDelay)
            {
                throw new ArgumentException("Minimum delay has to be less or equal than maximum delay");
            }
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var response = await base.UnaryServerHandler(request, context, continuation);
            await Task.Delay(r.Next(minDelay, maxDelay));
            return response;
        }
    }
}

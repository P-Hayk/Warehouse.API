using System;
using System.Threading;
using System.Threading.Tasks;
using Forex.Infrastructure.RabbitMq.Abstractions;

namespace Warehouse.Application.Extensions
{
    internal class RequestSender : IRequestSender
    {
        private readonly RequestClientAccessor _requestClientAccessor;

        public RequestSender(RequestClientAccessor requestClientAccessor)
        {
            _requestClientAccessor = requestClientAccessor;
        }

        public async Task<TReply> SendAsync<TRequest, TReply>(
            TRequest request,
            TimeSpan timeout,
            CancellationToken cancellationToken)
            where TRequest : class
            where TReply : class
        {
            var client = _requestClientAccessor.Get<TRequest>(timeout);
            var handle = client.Create(request, cancellationToken, timeout);
            return (await handle.GetResponse<TReply>()).Message;
        }
    }
}

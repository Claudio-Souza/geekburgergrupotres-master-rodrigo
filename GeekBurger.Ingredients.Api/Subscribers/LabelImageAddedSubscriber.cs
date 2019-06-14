using AutoMapper;
using GeekBurger.Ingredients.Api.Services;
using GeekBurger.Ingredients.DataLayer;
using GeekBurger.Ingredients.DomainModel;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Subscribers
{
    public class LabelImageAddedSubscriber
    {
        private readonly IMapper _mapper;
        private readonly IMergeService _mergeService;
        private readonly IQueueClient _queue;
        private readonly IUnitOfWork _unitOfWork;

        public LabelImageAddedSubscriber(IMapper mapper, IMergeService mergeService, IQueueClient queue, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _mergeService = mergeService;
            _unitOfWork = unitOfWork;

            var messageHandlerOptions = new MessageHandlerOptions(this.ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 3
            };

            queue.RegisterMessageHandler(this.ReceivedMessage, messageHandlerOptions);

            _queue = queue;
        }

        private async Task ReceivedMessage(Message message, CancellationToken cancellationToken)
        {
            var content = Encoding.UTF8.GetString(message.Body);

            var labelImageAddedMessage = JsonConvert.DeserializeObject<LabelImageAddedMessage>(content);

            var ingredient = _mapper.Map<Ingredient>(labelImageAddedMessage);
            await _mergeService.UpdateProductsMergesAsync(ingredient);
        }

        private async Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            await _unitOfWork.LogRepository.SaveAsync(arg.Exception.ToString());
        }

    }
}

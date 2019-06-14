using AutoMapper;
using GeekBurger.Ingredients.Api.Services;
using GeekBurger.Ingredients.DataLayer;
using GeekBurger.Ingredients.DomainModel;
using GeekBurger.Products.Contract;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Subscribers
{
    public class ProductChangedSubscriber
    {
        private readonly IMapper _mapper;
        private readonly IMergeService _mergeService;
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly IUnitOfWork _unitOfWork;

        public ProductChangedSubscriber(IMapper mapper, IMergeService mergeService, ISubscriptionClient subscriptionClient, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _mergeService = mergeService;
            _unitOfWork = unitOfWork;

            var messageHandlerOptions = new MessageHandlerOptions(this.ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 3
            };

            subscriptionClient.RegisterMessageHandler(this.ReceivedMessage, messageHandlerOptions);

            _subscriptionClient = subscriptionClient;
        }

        private async Task ReceivedMessage(Message message, CancellationToken cancellationToken)
        {
            var content = Encoding.UTF8.GetString(message.Body);

            var productChangedMessage = JsonConvert.DeserializeObject<ProductChangedMessage>(content);

            if (productChangedMessage.State == ProductState.Deleted)
            {
                await _unitOfWork.MergedProductsRepository.DeleteAsync(productChangedMessage.Product.ProductId);
                return;
            }

            //var product = _mapper.Map<ProductWithIngredients>(productChangedMessage);

            await _mergeService.MergeProductWithIngredientsAsync(productChangedMessage.Product);
        }

        private async Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            await _unitOfWork.LogRepository.SaveAsync(arg.Exception.ToString());
        }
    }
}

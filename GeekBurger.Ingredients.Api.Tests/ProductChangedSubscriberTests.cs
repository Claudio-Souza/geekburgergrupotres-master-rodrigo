using AutoFixture;
using GeekBurger.Ingredients.DataLayer;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using NSubstitute;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using GeekBurger.Ingredients.Api.Subscribers;
using AutoMapper;
using GeekBurger.Products.Contract;
using System.Threading;
using Newtonsoft.Json;
using GeekBurger.Ingredients.Api.Services;
using GeekBurger.Ingredients.DomainModel;

namespace GeekBurger.Ingredients.Api.Tests
{
    public class ProductChangedSubscriberTests
    {
        private Fixture _fixture;
        private IUnitOfWork _unitOfWork;
        private IMergeService _mergeService;
        private IMapper _mapper;
        private ISubscriptionClient _subscriptionClient;

        public ProductChangedSubscriberTests()
        {
            _fixture = new Fixture();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mergeService = Substitute.For<IMergeService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();

            _subscriptionClient = Substitute.For<ISubscriptionClient>();
        }

        [Fact]
        public async Task Upon_product_changed_message_received_and_an_error_occur_should_log()
        {
            //Arrange
            MessageHandlerOptions messageHandlerOptions = null;

            _subscriptionClient.When(q => q.RegisterMessageHandler(Arg.Any<Func<Message, CancellationToken, Task>>(), Arg.Any<MessageHandlerOptions>()))
                .Do(c => messageHandlerOptions = c.Arg<MessageHandlerOptions>());


            var productChangedSubscriber = new ProductChangedSubscriber(_mapper, _mergeService, _subscriptionClient, _unitOfWork);

            //Act
            await messageHandlerOptions.ExceptionReceivedHandler(_fixture.Create<ExceptionReceivedEventArgs>());

            //Assert
            await _unitOfWork.LogRepository.Received().SaveAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task Upon_product_changed_message_received_should_call_merge_service()
        {
            //Arrange
            Func<Message, CancellationToken, Task> call = null;

            _subscriptionClient.When(q => q.RegisterMessageHandler(Arg.Any<Func<Message, CancellationToken, Task>>(), Arg.Any<MessageHandlerOptions>()))
                .Do(c => call = c.Arg<Func<Message, CancellationToken, Task>>());


            var productChangedSubscriber = new ProductChangedSubscriber(_mapper, _mergeService, _subscriptionClient, _unitOfWork);

            var messageObject = _fixture.Create<ProductChangedMessage>();
            messageObject.State = ProductState.Added | ProductState.Modified;
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObject));

            //Act
            await call(new Message(messageBody), new CancellationToken());

            //Assert
            await _mergeService.Received().MergeProductWithIngredientsAsync(Arg.Any<ProductToGet>());
        }

        [Fact]
        public async Task Upon_product_changed_message_with_delete_state_should_call_method_product_resository_deletion_method()
        {
            //Arrange
            Func<Message, CancellationToken, Task> call = null;

            _subscriptionClient.When(q => q.RegisterMessageHandler(Arg.Any<Func<Message, CancellationToken, Task>>(), Arg.Any<MessageHandlerOptions>()))
                .Do(c => call = c.Arg<Func<Message, CancellationToken, Task>>());


            var productChangedSubscriber = new ProductChangedSubscriber(_mapper, _mergeService, _subscriptionClient, _unitOfWork);

            var messageObject = _fixture.Create<ProductChangedMessage>();
            messageObject.State = ProductState.Deleted;

            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObject));

            //Act
            await call(new Message(messageBody), new CancellationToken());

            //Assert
            await _unitOfWork.MergedProductsRepository.Received().DeleteAsync(Arg.Any<Guid>());
        }
    }
}

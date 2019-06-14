using AutoFixture;
using AutoMapper;
using GeekBurger.Ingredients.Api.Services;
using GeekBurger.Ingredients.Api.Subscribers;
using GeekBurger.Ingredients.DataLayer;
using GeekBurger.Ingredients.DomainModel;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GeekBurger.Ingredients.Api.Tests
{
    public class LabelImageAddedSubscriberTests
    {
        private Fixture _fixture;
        private IQueueClient _queue;
        private IMapper _mapper;
        private IMergeService _mergeService;
        private ServiceBusSettings _serviceBusSettings;
        private IUnitOfWork _unitOfWork;

        public LabelImageAddedSubscriberTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();

            _mergeService = Substitute.For<IMergeService>();

            _queue = Substitute.For<IQueueClient>();


            _unitOfWork = Substitute.For<IUnitOfWork>();

            _fixture = new Fixture();

        }

        [Fact]
        public async Task Upon_label_image_added_message_received_should_call_merge_service()
        {
            //Arrange
            Func<Message, CancellationToken, Task> call = null;

            _queue.When(q => q.RegisterMessageHandler(Arg.Any<Func<Message, CancellationToken, Task>>(), Arg.Any<MessageHandlerOptions>()))
                .Do(c => call = c.Arg<Func<Message, CancellationToken, Task>>());


            var labelImageAddedSubscriber = new LabelImageAddedSubscriber(_mapper, _mergeService, _queue, _unitOfWork);

            var messageObject = _fixture.Create<LabelImageAddedMessage>();
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObject));

            //Act
            await call(new Message(messageBody), new CancellationToken());

            //Assert
            await _mergeService.Received().UpdateProductsMergesAsync(Arg.Any<Ingredient>());
        }

        [Fact]
        public async Task Upon_label_image_added_message_received_and_an_error_occur_should_log()
        {
            //Arrange
            MessageHandlerOptions messageHandlerOptions = null;

            _queue.When(q => q.RegisterMessageHandler(Arg.Any<Func<Message, CancellationToken, Task>>(), Arg.Any<MessageHandlerOptions>()))
                .Do(c => messageHandlerOptions = c.Arg<MessageHandlerOptions>());


            var labelImageAddedSubscriber = new LabelImageAddedSubscriber(_mapper, _mergeService, _queue, _unitOfWork);

            //Act
            await messageHandlerOptions.ExceptionReceivedHandler(_fixture.Create<ExceptionReceivedEventArgs>());

            //Assert
            await _unitOfWork.LogRepository.Received().SaveAsync(Arg.Any<string>());
        }
    }
}

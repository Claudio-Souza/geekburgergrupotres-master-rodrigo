using AutoFixture;
using AutoMapper;
using GeekBurger.Ingredients.Api.Services;
using GeekBurger.Products.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GeekBurger.Ingredients.Api.Tests
{
    public class ProductsServiceTests
    {
        private string _productsServiceUri;

        private HttpClient _client;
        private Fixture _fixture;
        private IMapper _mapper;
        private IMergeService _mergeService;
        private Mock<HttpMessageHandler> _httpHandlerMock;
        private ProductService _productService;

        public ProductsServiceTests()
        {
            _fixture = new Fixture();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = new Mapper(config);

            _mergeService = Substitute.For<IMergeService>();

            _httpHandlerMock = new Mock<HttpMessageHandler>();
            _client = new HttpClient(_httpHandlerMock.Object);

            _productsServiceUri = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<string>("ProductsServiceUri");

            _productService = new ProductService(_productsServiceUri, _client, _mapper, _mergeService);
        }

        [Fact]
        public async void Product_service_should_make_call_to_get_products()
        {
            //Arrange
            var content = JsonConvert.SerializeObject(_fixture.Create<IEnumerable<ProductToGet>>());

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(content) });

            var storeName = _fixture.Create<string>();

            //Act -> Assert
            await _productService.GetStoreProducts(storeName);
        }

        [Fact]
        public async void Product_service_call_to_get_store_products_should_return_a_list_of_ProductToGet()
        {
            //Arrange
            var content = JsonConvert.SerializeObject(_fixture.Create<IEnumerable<ProductToGet>>());

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(content) });


            var storeName = _fixture.Create<string>();

            //Act
            var result = (await _productService.GetStoreProducts(storeName)) as IEnumerable<ProductToGet>;

            //Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}

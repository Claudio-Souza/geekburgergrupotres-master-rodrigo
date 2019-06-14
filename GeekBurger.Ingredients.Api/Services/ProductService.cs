using AutoMapper;
using GeekBurger.Ingredients.DomainModel;
using GeekBurger.Products.Contract;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Services
{
    public class ProductService : BackgroundService, IProductService
    {
        private string _productsServiceUri;
        private HttpClient _client;
        private IMapper _mapper;
        private IMergeService _mergeService;

        public ProductService(string productsServiceUri, HttpClient client, IMapper mapper, IMergeService mergeService)
        {
            _productsServiceUri = productsServiceUri;

            _client = client;

            _mapper = mapper;
            _mergeService = mergeService;
        }

        public async Task<IEnumerable<ProductToGet>> GetStoreProducts(string storeName)
        {
            var response = await _client.GetAsync($"{_productsServiceUri}/products?storeName={storeName}");

            return JsonConvert.DeserializeObject<IEnumerable<ProductToGet>>(await response.Content.ReadAsStringAsync());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var storeProducts = await this.GetStoreProducts("Los Angeles - Pasadena");

            //foreach (var product in _mapper.Map<IEnumerable<ProductWithIngredients>>(storeProducts))
            //{
            //    await _mergeService.MergeProductWithIngredientsAsync(product);
            //}

            foreach (var product in storeProducts)
            {
                await _mergeService.MergeProductWithIngredientsAsync(product);
            }
        }
    }
}

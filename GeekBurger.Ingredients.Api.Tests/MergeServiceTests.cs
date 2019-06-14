using AutoFixture;
using GeekBurger.Ingredients.Api.Services;
using GeekBurger.Ingredients.DomainModel;
using GeekBurger.Products.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GeekBurger.Ingredients.Api.Tests
{
    public class MergeServiceTests
    {
        [Fact]
        public async void Given_a_list_of_products_and_a_list_of_ingredients_should_merge_they()
        {
            //Arrange
            var fixture = new Fixture();

            var productList = fixture.CreateMany<ProductToGet>();
            //var igredientList = fixture.CreateMany<>();
             

            //var mergeService = new MergeService();


            //Act
            //var result = mergeService.MergeProductsAndIngredients()

            //Assert
        }
    }
}

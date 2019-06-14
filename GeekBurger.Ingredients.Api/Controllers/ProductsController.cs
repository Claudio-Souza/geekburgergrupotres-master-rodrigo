using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GeekBurger.Ingredients.Contract.Request;
using GeekBurger.Ingredients.Contract.Response;
using GeekBurger.Ingredients.DataLayer;
using GeekBurger.Ingredients.DomainModel;
using Microsoft.AspNetCore.Mvc;

namespace GeekBurger.Ingredients.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public ProductsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("byrestrictions/{idloja}/{restricao}")]
        public async Task<ActionResult<IEnumerable<IngredientsToUpsert>>> Get(IngredientsToGet request)
        {
            var productsWithRestrictions = await _unitOfWork.MergedProductsRepository.GetProductRestrictionByStore(request.StoreId, request.Restrictions);

            return Ok(_mapper.Map<IEnumerable<IngredientsToUpsert>>(productsWithRestrictions));
        }
    }
}

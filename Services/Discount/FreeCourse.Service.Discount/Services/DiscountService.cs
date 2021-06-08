using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using FreeCourse.Service.Discount.Models;
using FreeCourse.Shared;
using FreeCourse.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FreeCourse.Service.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
        }

        public async Task<Response<List<Discounts>>> GetAll()
        {
            var discounts = (await _dbConnection.GetAllAsync<Discounts>()).ToList();
            return Response<List<Discounts>>.Success(discounts,HttpStatusCode.OK);
        }

        public async Task<Response<Discounts>> GetById(int id)
        {
            var discount = (await _dbConnection.QueryAsync<Discounts>("select * from discount where id=@Id", new { Id = id })).SingleOrDefault();
            return discount == null
                ? Response<Discounts>.Fail("Not found discount", HttpStatusCode.NotFound)
                : Response<Discounts>.Success(discount, HttpStatusCode.OK);
        }
        
        public async Task<Response<Discounts>> GetByCodeAndUserId(string code, string userId)
        {
            var discounts = await _dbConnection.QueryAsync<Discounts>("select * from discount where userid=@UserId and code=@Code",
                new
                {
                    UserId = userId,
                    Code = code
                });
            var hasDiscount = discounts.FirstOrDefault();
            return hasDiscount == null
                ? Response<Discounts>.Fail("Not found discount", HttpStatusCode.NotFound)
                : Response<Discounts>.Success(hasDiscount,HttpStatusCode.OK);
        }

        public async Task<Response<NoContent>> Save(Discounts discounts)
        {
            var status = await _dbConnection.ExecuteAsync("INSERT INTO discount (userid,rate,code) VALUES(@UserId,@Rate,@Code)", discounts);
            return status > 0
                ? Response<NoContent>.Success(HttpStatusCode.NoContent)
                : Response<NoContent>.Fail("an error while adding", HttpStatusCode.InternalServerError);
        }

        public async Task<Response<NoContent>> Update(Discounts discounts)
        {
            var status = await _dbConnection.ExecuteAsync("update discount set userid=@UserId, code=@Code, rate=@Rate where id=@Id", new { Id = discounts.Id, UserId = discounts.UserId, Code = discounts.Code, Rate = discounts.Rate });
            return status > 0
                ? Response<NoContent>.Success(HttpStatusCode.NoContent)
                : Response<NoContent>.Fail("an error while updating", HttpStatusCode.InternalServerError);
        }

        public async Task<Response<NoContent>> Delete(int id)
        {
            var status = await _dbConnection.ExecuteAsync("delete from discount where id=@Id", new { Id = id });
            
            return status > 0
                ? Response<NoContent>.Success(HttpStatusCode.NoContent)
                : Response<NoContent>.Fail("Not found discount", HttpStatusCode.NotFound);
        }
    }
}
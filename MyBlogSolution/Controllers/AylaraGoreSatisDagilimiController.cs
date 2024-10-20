using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyBlog.Entities.DTOs;

namespace MyBlogSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AylaraGoreSatisDagilimiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;
        private readonly ILogger<AylaraGoreSatisDagilimiController> _logger;

        public AylaraGoreSatisDagilimiController(IMapper mapper, ILogger<AylaraGoreSatisDagilimiController> logger, IConfiguration configuration)
        {
            _mapper = mapper;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> GetAylaraGoreSatisDagilimi()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT 1 AS OcakUyeSay, 11 AS OcakCiro, 2 AS SubatUyeSay, 22 AS SubatCiro, 3 AS MartUyeSay, 33 AS MartCiro,
                                      4 AS NisanUyeSay, 44 AS NisanCiro, 5 AS MayisUyeSay, 55 AS MayisCiro, 6 AS HaziranUyeSay, 66 AS HaziranCiro,
                                      7 AS TemmuzUyeSay, 77 AS TemmuzCiro, 8 AS AgustosUyeSay, 88 AS AgustosCiro, 9 AS EylulUyeSay, 99 AS EylulCiro,
                                      10 AS EkimUyeSay, 100 AS EkimCiro, 11 AS KasimUyeSay, 111 AS KasimCiro, 12 AS AralikUyeSay, 122 AS AralikCiro";
                    var result = await connection.QueryFirstOrDefaultAsync<AylaraGoreSatisDagilimiDto>(query);
                    return Ok(_mapper.Map<AylaraGoreSatisDagilimiDto>(result));
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error occurred while fetching AylaraGoreSatisDagilimi");
                return StatusCode(500, "SQL error occurred: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching AylaraGoreSatisDagilimi");
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
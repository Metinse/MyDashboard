using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyBlog.Entities.DTOs;

namespace MyBlogSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SatisCirosuKartiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;
        private readonly ILogger<SatisCirosuKartiController> _logger;

        public SatisCirosuKartiController(IMapper mapper, ILogger<SatisCirosuKartiController> logger, IConfiguration configuration)
        {
            _mapper = mapper;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> GetSatisCirosuKarti()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT 1000 AS GunlukCiro, 7000 AS HaftalikCiro, 30000 AS AylikCiro, 300000 AS YillikCiro";
                    var result = await connection.QueryFirstOrDefaultAsync<SatisCirosuKartiDto>(query);
                    return Ok(_mapper.Map<SatisCirosuKartiDto>(result));
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error occurred while fetching SatisCirosuKarti");
                return StatusCode(500, "SQL error occurred: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching SatisCirosuKarti");
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
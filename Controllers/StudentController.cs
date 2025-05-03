using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace rnjm_test_plugin_api.Controllers
{
    [Route("stu-plugin-api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _pluginConfiguration;

        public StudentController(IConfiguration pluginConfiguration)
        {
            _pluginConfiguration = pluginConfiguration;
        }

        // 计算加权成绩
        [HttpGet]
        [Route("CalculateWeightedScore/{studentId}")]
        public IActionResult CalculateWeightedScore(int studentId)
        {
            string connectionString = _pluginConfiguration.GetConnectionString("DbConnect")!;
            double weightedScore = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string query = "SELECT Score, Weight FROM StudentScores WHERE StudentId = @studentId";

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@studentId", studentId);

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            while (sqlDataReader.Read())
                            {
                                double score = sqlDataReader.GetDouble(sqlDataReader.GetOrdinal("Score"));
                                double weight = sqlDataReader.GetDouble(sqlDataReader.GetOrdinal("Weight"));
                                weightedScore += score * weight;
                            }
                        }
                    }
                }

                return Ok(new { WeightedScore = weightedScore });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // 确定通过所需的百分比
        [HttpGet]
        [Route("DeterminePassingPercentage")]
        public IActionResult DeterminePassingPercentage(double totalScore, double passingScore)
        {
            if (totalScore <= 0)
            {
                return BadRequest("Total score must be greater than zero.");
            }

            double passingPercentage = (passingScore / totalScore) * 100;
            return Ok(new { PassingPercentage = passingPercentage });
        }
    }
}
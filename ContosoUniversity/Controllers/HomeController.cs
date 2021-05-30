using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewModels;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using System.Linq;
using System;

namespace ContosoUniversity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SchoolContext _context;
        public HomeController(ILogger<HomeController> logger, SchoolContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Using Dapper
        public async Task<ActionResult> About()
        {
            var groups = Enumerable.Empty<EnrollmentDateGroup>();

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    string sqlQuery = "SELECT EnrollmentDate, COUNT(*) AS StudentCount "
                        + "FROM Student "
                        + "GROUP BY EnrollmentDate";

                    groups = await connection.QueryAsync<EnrollmentDateGroup>(
                      sqlQuery,
                      commandTimeout: 480,
                      commandType: CommandType.Text
                    );
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return View(groups);
        }

        //Without Dapper

        //public async Task<ActionResult> About()
        //{
        //    List<EnrollmentDateGroup> groups = new List<EnrollmentDateGroup>();
        //    var conn = _context.Database.GetDbConnection();
        //    try
        //    {
        //        await conn.OpenAsync();
        //        using (var command = conn.CreateCommand())
        //        {
        //            string query = "SELECT EnrollmentDate, COUNT(*) AS StudentCount "
        //                + "FROM Student "
        //                + "GROUP BY EnrollmentDate";
        //            command.CommandText = query;
        //            DbDataReader reader = await command.ExecuteReaderAsync();

        //            if (reader.HasRows)
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    var row = new EnrollmentDateGroup { EnrollmentDate = reader.GetDateTime(0), StudentCount = reader.GetInt32(1) };
        //                    groups.Add(row);
        //                }
        //            }
        //            reader.Dispose();
        //        }
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //    return View(groups);
        //}
    }
}

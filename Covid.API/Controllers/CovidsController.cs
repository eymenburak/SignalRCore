using Covid.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidsController : ControllerBase
    {
        private readonly CovidService _covidService;

        public CovidsController(CovidService covidService)
        {
            _covidService = covidService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCovid(Covid19 covid)
        {
            await _covidService.SaveCovidData(covid);
            return Ok(_covidService.GetCovidChartList());
        }


        [HttpGet]
        public  IActionResult InitializeCovid()
        {
            Random rnd = new Random();

            Enumerable.Range(1, 10).ToList().ForEach(x =>
            { 
                foreach(ECity item in Enum.GetValues(typeof(ECity)))
                {
                    var newcovid = new Covid19 { City = item, Count = rnd.Next(100, 1000), CovidDate = DateTime.Now.AddDays(x) };
                     _covidService.SaveCovidData(newcovid).Wait();
                    System.Threading.Thread.Sleep(1000);
                }
            });

            return Ok("Covid data was inserted Db");
        }
    }
}

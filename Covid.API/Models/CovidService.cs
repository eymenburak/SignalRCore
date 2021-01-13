using Covid.API.Hubs;
using Covid.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid.API.Models
{
    public class CovidService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<CovidHub> _hubContext;
        

        public CovidService(AppDbContext context, IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IQueryable<Covid19> GetList()
        {
            return _context.Covid19s.AsQueryable();
        }

        public async Task SaveCovidData(Covid19 covid)
        {
            await _context.Covid19s.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChartList());
        }

        public List<CovidChart> GetCovidChartList()
        {
            List<CovidChart> covidCharts = new List<CovidChart>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT tarih, [1],[2],[3],[4],[5] FROM (SELECT[City],[Count], Cast([CovidDate] as date) as tarih FROM Covid19s) as covidTable PIVOT (SUM(Count) For City IN([1],[2],[3],[4],[5])) as PTable ORDER BY tarih asc";

                command.CommandType = System.Data.CommandType.Text;

                _context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart covidChart = new CovidChart();
                        covidChart.CovidDate = reader.GetDateTime(0).ToShortDateString();
                        Enumerable.Range(1, 5).ToList().ForEach(x => 
                        {
                            if (System.DBNull.Value.Equals(reader[x]))
                            {
                                covidChart.Counts.Add(0);
                            }
                            else
                            {
                                covidChart.Counts.Add(reader.GetInt32(x));
                            }
                        
                        });

                        covidCharts.Add(covidChart);
                    }
                }
            }

            _context.Database.CloseConnection();

            return covidCharts;
        }

    }
}

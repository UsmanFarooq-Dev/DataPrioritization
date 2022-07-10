using DataPrioritization.WebApp.Data;
using DataPrioritization.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using DataPrioritization.DataAccess;

using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Forms;
//inject ILogger<FileUpload1> Logger
//inject IWebHostEnvironment Environment

namespace DataPrioritization.WebApp.Pages;

public partial class FetchData
{

    [Inject] WeatherForecastService ForecastService { get; set; }
    [Inject] IOptions<ConnectionStringModel> settings { get; set; }
    private WeatherForecast[]? forecasts;
    private IList<Employee>? employees;

    protected override async Task OnInitializedAsync()
    {
        var conString = settings.Value.ConnectionString;
        forecasts = await ForecastService.GetForecastAsync(DateTime.Now);
        employees = Db.GetEmployees(conString);
    }

}

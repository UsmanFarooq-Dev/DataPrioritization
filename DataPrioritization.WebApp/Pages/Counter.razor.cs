
using DataPrioritization.WebApp.Data;

namespace DataPrioritization.WebApp.Pages;

public partial class Counter
{

    private int currentCount = 0;
    private WeatherForecast test = new WeatherForecast();
    private void IncrementCount()
    {
        currentCount++;
    }
}

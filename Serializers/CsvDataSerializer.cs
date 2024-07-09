using System.Text;

namespace NBU_Currency_Rate_Monitor.Serializers;

public class CsvDataSerializer : IDataSerializer
{
    public string Serialize(CurrenciesData data)
    {
        var csv = new StringBuilder();
        foreach (var item in data)
        {
            csv.AppendLine($"{item.Code},{item.Rate},{item.Date},{item.Time}");
        }
        return csv.ToString();
    }
}
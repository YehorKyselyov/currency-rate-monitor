using System.Text;

namespace NBU_Currency_Rate_Monitor.Serializers;

public class CsvDataSerializer : IDataSerializer
{
    public void Serialize(CurrenciesData data, string filePath)
    {
        var csv = new StringBuilder();

        bool fileExists = File.Exists(filePath);

        if (!fileExists)
        {
            csv.AppendLine("Code,Rate,Date,Time");
        }

        foreach (var item in data)
        {
            csv.AppendLine($"{item.Code},{item.Rate},{item.Date},{item.Time}");
        }

        File.AppendAllText(filePath, csv.ToString());
    }

    public CurrenciesData Deserialize(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var data = new CurrenciesData();

        // Skip the header line
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            if (parts.Length == 4)
            {
                var currencyData = new CurrencyData(parts[0], double.Parse(parts[1]), parts[2], parts[3]);
                data.Add(currencyData);
            }
        }

        return data;
    }
}
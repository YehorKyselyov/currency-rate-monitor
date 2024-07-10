using System.Collections.Generic;
using System.Text.Json;

namespace NBU_Currency_Rate_Monitor.Serializers;

public class JsonDataSerializer : IDataSerializer
{
    public void Serialize(CurrenciesData data, string filePath)
    {
        var existingData = File.Exists(filePath) ? Deserialize(filePath) : new CurrenciesData();

        existingData.AddRange(data);

        var json = JsonSerializer.Serialize(existingData, new JsonSerializerOptions {IncludeFields = true, WriteIndented = true});
        File.WriteAllText(filePath, json);
    }

    public CurrenciesData Deserialize(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<CurrenciesData>(json, new JsonSerializerOptions { IncludeFields = true });
    }
}
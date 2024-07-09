using System.Collections.Generic;
using System.Text.Json;

namespace NBU_Currency_Rate_Monitor.Serializers;

public class JsonDataSerializer : IDataSerializer
{
    public string Serialize(CurrenciesData data)
    {
        return JsonSerializer.Serialize(data);
    }
}
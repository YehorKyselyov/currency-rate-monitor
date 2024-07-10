using NBU_Currency_Rate_Monitor.Serializers;

namespace NBU_Currency_Rate_Monitor;

public class DataIO
{
    private readonly IDataSerializer _dataSerializer;

    public DataIO(IDataSerializer dataSerializer)
    {
        _dataSerializer = dataSerializer;
    }

    public async Task SaveDataAsync(CurrenciesData data, string filePath)
    {
        await Task.Run(() => _dataSerializer.Serialize(data, filePath));
    }

    public async Task<CurrenciesData> LoadDataAsync(string filePath)
    {
        return await Task.Run(() => _dataSerializer.Deserialize(filePath));
    }
}
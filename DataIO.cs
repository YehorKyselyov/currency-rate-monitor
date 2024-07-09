using NBU_Currency_Rate_Monitor.Serializers;

namespace NBU_Currency_Rate_Monitor;

public class DataIO
{
    private readonly IDataSerializer _dataSerializer;
    private readonly string _outputPath;

    public DataIO(IDataSerializer dataSerializer, string outputPath)
    {
        _dataSerializer = dataSerializer;
        _outputPath = outputPath;
    }

    public async Task SaveDataAsync(CurrenciesData data)
    {
        var serializedData = _dataSerializer.Serialize(data);
        await File.WriteAllTextAsync(_outputPath, serializedData);
    }
}
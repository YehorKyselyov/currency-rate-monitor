namespace NBU_Currency_Rate_Monitor.Serializers;

public interface IDataSerializer
{
    public void Serialize(CurrenciesData data, string filePath);
    public CurrenciesData Deserialize(string filePath);
}
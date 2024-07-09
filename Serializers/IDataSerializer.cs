namespace NBU_Currency_Rate_Monitor.Serializers;

public interface IDataSerializer
{
    string Serialize(CurrenciesData data);
}
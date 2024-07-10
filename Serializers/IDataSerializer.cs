namespace NBU_Currency_Rate_Monitor.Serializers;

/// <summary>
/// Defines methods for serializing and deserializing currency data.
/// </summary>
public interface IDataSerializer
{
    /// <summary>
    /// Serializes the provided currency data to the specified file path.
    /// </summary>
    /// <param name="data">The currency data to serialize.</param>
    /// <param name="filePath">The file path where the data should be saved.</param>
    public void Serialize(CurrenciesData data, string filePath);

    /// <summary>
    /// Deserializes currency data from the specified file path.
    /// </summary>
    /// <param name="filePath">The file path to read the data from.</param>
    /// <returns>The deserialized currency data.</returns>
    public CurrenciesData Deserialize(string filePath);
}
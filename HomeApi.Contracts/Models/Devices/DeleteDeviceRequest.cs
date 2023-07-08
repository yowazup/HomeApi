namespace HomeApi.Contracts.Models.Devices
{
    /// <summary>
    /// Запрос для удаления подключенного устройства
    /// </summary>
    public class DeleteDeviceRequest
    {
        public string Serial { get; set; }
    }
}
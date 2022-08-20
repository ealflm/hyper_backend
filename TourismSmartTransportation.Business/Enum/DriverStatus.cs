public enum DriverStatus
{
    Disabled = 0, // Tài xế bị vô hiệu quá
    Active = 1, // Đang hoạt động đối với mọi tài xế
    On = 2, // Mở chế độ nhận request - dịch vụ đặt xe
    Off = 3, // Tắt chế độ nhận request - dịch vụ đặt xe
    OnBusy = 4, // Tài xế đã và đang nhận yêu cầu từ một khách hàng
    OnArriving = 5, // Driver đang đi đón khách hàng
    OnArrived = 6, // Đã đến điểm đón khách hàng
    PickedUp = 7, // đã đón khách hàng
}
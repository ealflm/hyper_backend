public enum DriverStatus
{
    Disabled = 0, // Tài xế bị vô hiệu quá
    Active = 1, // Đang hoạt động đối với mọi tài xế
    On = 2, // Mở chế độ nhận request - dịch vụ đặt xe
    Off = 3 // Tắt chế độ nhận request - dịch vụ đặt xe
}
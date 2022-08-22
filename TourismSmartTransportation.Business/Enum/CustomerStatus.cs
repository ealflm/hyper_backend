public enum CustomerStatus
{
    Disabled = 0, // Vô hiệu hóa
    Normal = 1, // Bình thường
    Waiting = 2, // Đang đợi tài xế đến
    DriverArrived = 3, // Tài xế đã đến
    PickedUp = 4, //  Customer đã lên xe
    NotFound = 5, // Customer đã nhận thông báo không tìm thấy tài xế
    Finding = 6, // customer đang tìm kiếm driver
    OnDisconnect = 7 // ngắt kết nối đột ngột như tắt app
}
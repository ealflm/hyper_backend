public enum OrderStatus
{
    Canceled = 0, // Hủy hóa đơn -> booking service
    Paid = 1, // Đã thanh toán
    Done = 2, // Hoàn thành
    Unpaid = 3, // Chưa thanh toán
}
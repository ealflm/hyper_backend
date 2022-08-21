public enum OrderStatus
{
    Canceled = 0, // Hủy hóa đơn -> booking service
    Paid = 1, // Đã thanh toán
    Done = 2, // Hoàn thành
    Unpaid = 3, // Chưa thanh toán
    NotUse=4, //Gói dịch vụ ko sử dụng nữa
    WrongStatus = 10, // Trạng thái chưa được xử lý, lỗi logic code
}
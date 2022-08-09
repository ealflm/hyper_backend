public enum CustomerTripStatus
{
    Canceled = 0, // Bị hủy bỏ -> booking service
    New = 1, // Customer trip vừa tạo -> all services
    Accepted = 2, // Đã chấp thuận -> booking service
    PickedUp = 3, // Đã đón khách -> booking service
    Renting = 4, // Đang thuê -> renting service
    Overdue = 5, // Thời gian thuê quá hạn -> renting service
    Done = 6 // Customer trip hoàn thành -> all service
}
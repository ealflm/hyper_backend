public enum NotificationStatus
{
    Disabled = 0, // trạng thái không hiển thị
    Active = 1, // trạng thái để hiện thị cho thông báo bình thường
    BeNotUsed = 2, // dành cho discount, trạng thái để hiện thị cho discount và thể hiện trạng thái discount chưa được sử dụng
    BeUsed = 3, // dành cho discount, trạng thái discount đã sử dụng
}
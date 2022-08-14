public enum DiscountStatus
{
    Disabled = 0, // bị vô hiệu hóa, bị hủy bỏ
    UnSent = 1, // còn hạn sử dụng, chưa được gửi đến khách hàng
    BeSent = 2, // còn hạn sử dụng, đã gửi đến khách hàng, khách hàng chưa sử dụng
    BeUsed = 3, // đã được sử dụng
    Expire = 4, // hết hạn sử dụng
}
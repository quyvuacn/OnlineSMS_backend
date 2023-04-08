using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace OnlineSMS
{
    public static class Constants
    {
        public class RegisterResultMessage 
        {
            public const string Success = "Đăng kí thành công !";
            public const string PhoneNumberNotVerified = "Số điện thoại chưa xác minh !";
            public const string EmailNotVerified = "Email chưa xác minh !";
            public const string AccountExist = "Tài khoản đã tồn tại !";
            public const string BadRequest = "Lỗi chưa xác định !";
        };

        public class LoginResultMessage
        {
            public const string Success = "Đăng nhập thành công";
            public const string WrongInfo = "Tài khoản hoặc mật khẩu sai !";
            public const string LockedOut = "Tài khoản đã bị khóa !";
        };

        public class VerificationCodeMessage
        {
            public const string Success = "Successfull";
            public const string CodeExpired = "Mã xác minh hết hạn";
            public const string PhoneLockedOut = "Số điện thoại đang bị hạn chế xác minh"; 
        }
    }
    
}

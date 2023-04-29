using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace OnlineSMS
{
    public static class Constants
    {
        public class RegisterResultMessage
        {
            public const string Success = "Đăng kí thành công !";
            public const string PhoneNumberNotVerified = "Mã xác thực chưa đúng!";
            public const string EmailNotVerified = "Email chưa xác minh !";
            public const string PhonenumberExist = "Số điện thoại đã được sử dụng!";
            public const string EmailExist = "Số điện thoại đã được sử dụng!";
            public const string BadRequest = "Lỗi chưa xác định !";
        };

        public class LoginResultMessage
        {
            public const string Success = "Đăng nhập thành công";
            public const string WrongInfo = "Tài khoản hoặc mật khẩu sai !";
            public const string LockedOut = "Tài khoản đã bị khóa ! Do đăng nhập sai nhiều lần!";
        };

        public class VerificationCodeMessage
        {
            public const string Success = "Successfull";
            public const string CodeExpired = "Mã xác minh hết hạn";
            public const string CodeUsed = "Hãy thử lại xác minh số điện thoại!";
            public const string PhoneLockedOut = "Số điện thoại đang bị hạn chế xác minh";
        }

        public class StatusFriendship
        {
            public const int Pending = 0;
            public const int Friend = 1;
            public const int Cancelled = 2;
        }

        public class BoxChatType
        {
            public const string Normal = "Normal";
            public const string Group = "Group";
            public const string OnlyMe = "OnlyMe";
        }

        public class StatusBoxChatMessage
        {
            public const int Show = 1;
            public const int Hide = 2;
            public const int Deleted = 0;
        }
    }
    
}

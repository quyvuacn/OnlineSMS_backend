using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSMS.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public string UserRequestId { get; set; }
        public string UserAcceptId { get; set; }
        public int Status { get; set; } = 0;

        public User? UserRequest { get; set; }
        public User? UserAccept { get; set; }

    }
}

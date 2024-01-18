using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingSystemAPI.Models
{
    public class OtpModel
    {
        [Key]
        public int id { get; set; }

        public string email { get; set; }

        public string otpValue { get; set; }


        public bool isVerified { get; set; }

        // Foreign key for the SignupLogin model
        public int userId { get; set; }
        [ForeignKey("userId")]
        public virtual SignupLogin user { get; set; }
    }
}

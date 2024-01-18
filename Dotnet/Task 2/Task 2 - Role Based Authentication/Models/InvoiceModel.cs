using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FoodOrderingSystemAPI.Models
{
    public class InvoiceModel
    {
        [Key]
        public int id { get; set; }
        public DateTime? dateOfInvoice { get; set; }
        public float Total_Bill { get; set; }
        public string? transactionId{get;set;}
        public string? orderId{get;set;}
        public int? userId { get; set; }
        [ForeignKey("userId")]
        public virtual SignupLogin user { get; set; }
    }
}
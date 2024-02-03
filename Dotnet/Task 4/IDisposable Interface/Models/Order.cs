using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FoodOrderingSystemAPI.Models
{
    public class Order
    {
        [Key]
        public int id { get; set; }
        public int quantity { get; set; }
        public int unitPrice { get; set; }
        public float orderBill { get; set; }
        public DateTime? orderDate { get; set; }
        public string? orderStatus {get;set;}

        public int? foodItemId { get; set; }
        [ForeignKey("foodItemId")]
        public virtual FoodItem foodItems { get; set; }

        public int? invoiceId { get; set; }
        [ForeignKey("invoiceId")]
        public virtual InvoiceModel invoices { get; set; }

       
    }
}
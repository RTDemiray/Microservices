using System;
using System.Collections.Generic;

namespace FreeCourse.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        //Sipariş geçmişinde adres alanına ihtiyaç olmadığından dolayı alınmadı.
        /*public Address Address { get; set; }*/
        public string BuyerId { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}
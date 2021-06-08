﻿using System;
using System.Collections.Generic;
using FreeCourse.Service.Order.Domain.OrderAggregate;

namespace FreeCourse.Service.Order.Application.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Address Address { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
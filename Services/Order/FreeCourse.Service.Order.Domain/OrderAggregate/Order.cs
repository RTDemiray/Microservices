using System;
using System.Collections.Generic;
using System.Linq;
using FreeCourse.Service.Order.Domain.Core;

namespace FreeCourse.Service.Order.Domain.OrderAggregate
{
    //EF Core features
    // -- Owned Types
    // -- Shadow Property
    // -- Backing Field
    public class Order : Entity,IAggregateRoot
    {
        public DateTime CreatedDate { get; private set; }
        public Address Address { get; private set; }
        public string BuyerId { get; private set; }

        private readonly List<OrderItem> _orderItems;

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        public Order()
        {
            
        }
        
        public Order(string buyerId, Address address)
        {
            _orderItems = new();
            CreatedDate = DateTime.Now;
            BuyerId = buyerId;
            Address = address;
        }

        public void AddOrderItem(string productId, string productName, string pictureUrl, Decimal price)
        {
            var existProduct = _orderItems.Any(x => x.ProductId == productId);
            if (!existProduct)
            {
                var newOrderItem = new OrderItem(productId, productName, pictureUrl, price);
                _orderItems.Add(newOrderItem);
            }
        }

        public Decimal GetTotalPrice => _orderItems.Sum(x => x.Price);
    }
}
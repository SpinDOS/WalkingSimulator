using System;

namespace WalkEngine
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        public OrderAttribute(int order) => Order = order;
        
        public int Order { get; }

        public static int GetOrder(Type type)
        {
            var orderAttribute = (OrderAttribute) Attribute.GetCustomAttribute(type, typeof(OrderAttribute));
            return orderAttribute?.Order ?? 0;
        }
    }
}
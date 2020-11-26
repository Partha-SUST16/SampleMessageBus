using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompany.Messaging
{
    public interface IRegisterCustomer : IRegisterDomain
    {
        Guid Id { get; }
        DateTime RegisteredUtc { get; }
        int Type { get; }
        string Name { get; }
        bool Preferred { get; }
        decimal DefaultDiscount { get; }
        string Address { get; }
    }
}

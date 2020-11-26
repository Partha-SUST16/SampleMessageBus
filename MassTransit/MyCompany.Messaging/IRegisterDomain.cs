using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompany.Messaging
{
    public interface IRegisterDomain
    {
        string Target { get; }
        int Importance { get; }
    }
}

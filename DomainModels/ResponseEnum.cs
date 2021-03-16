using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels
{
    public enum ResponseEnum
    {
        Ok,
        AlreadyExist,
        Invalid,
        InternalError,
        UserDoesNotExist,
        InvalidPassword,
        Unauthorized,
        NotFound
    }
}

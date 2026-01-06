using System;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.Core.Domain.Exceptions
{
    public class SystemExecutionException : Exception
    {
        public SystemExecutionException(string systemName, Exception inner)
            : base($"Error en sistema '{systemName}'", inner) { }
    }
}

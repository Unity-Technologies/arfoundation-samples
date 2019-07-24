using System;

namespace Unity.iOS.Multipeer
{
    public class NSErrorException : Exception
    {
        public NSErrorException(long code, string description)
        : base($"NSError {code}: {description}")
        {
            Code = code;
            Description = description;
        }

        public long Code { get; private set; }

        public string Description { get; private set; }
    }
}

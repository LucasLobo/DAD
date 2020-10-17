using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    class GStoreObject
    {
        public GStoreObjectIdentifier Identifier { get; }
        public string Value { get; }

        public GStoreObject(GStoreObjectIdentifier identifier, string value)
        {
            ValidateParameters(identifier, value);

            Identifier = identifier;
            Value = value;
        }

        private void ValidateParameters(GStoreObjectIdentifier identifier, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value parameter can't be null or empty.");
            }

            if (identifier == null)
            {
                throw new ArgumentException("identifier parameter can't be null");
            }
        }

    }
}

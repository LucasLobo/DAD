using System;

namespace Utils
{
    public abstract class GenericServer<T> : IEquatable<GenericServer<T>>
    {
        public string Id { get; }

        public bool Alive { get; private set; }

        public T Stub { get; }

        protected GenericServer(string id, T stub)
        {
            Id = id;
            Alive = true;
            Stub = stub;
        }

        public void DeclareDead()
        {
            Alive = false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GenericServer<T>);
        }

        public bool Equals(GenericServer<T> other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public bool Equals(String otherId)
        {
            return otherId != null &&
                   Id == otherId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return $"{Id}, {Alive}";
        }
    }
}

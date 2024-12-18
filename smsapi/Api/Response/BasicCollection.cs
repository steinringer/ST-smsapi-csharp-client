using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using SMSApi.Api.Response.ResponseResolver;

namespace SMSApi.Api.Response
{
    [DataContract]
    public class BasicCollection<T> : Countable, IResponseCodeAwareResolver
    {
        [DataMember(Name = "collection", IsRequired = false)]
        protected List<T> collection;

        [DataMember(Name = "size", IsRequired = false)]
        protected int size;

        protected BasicCollection()
        { }

        public List<T> Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = new List<T>();
                }

                return collection;
            }

            set
            { }
        }

        [Obsolete("use Size instead")]
        public override int Count => Size;

        [Obsolete("use Collection instead")]
        [DataMember(Name = "list", IsRequired = false)]
        public List<T> List
        {
            get => Collection;
            protected set => collection = value;
        }

        public int Size
        {
            get
            {
                if (size == 0)
                {
                    return base.Count;
                }

                return size;
            }
        }

#if NETSTANDARD
        public Dictionary<int, Action<Stream>> HandleExceptionActions() => new();
#endif
    }
}

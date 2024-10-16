using System;

namespace EditorTools
{
    public abstract class BaseAttribute : Attribute
    {
        public string ID { get; private set; }
        public int Order { get; private set; }

        public BaseAttribute(string id = null)
        {
            ID = id ?? Guid.NewGuid().ToString();
        }

        public BaseAttribute(int order)
        {
            ID = Guid.NewGuid().ToString();
            Order = order;
        }

        public BaseAttribute(string id, int order)
        {
            ID = id;
            Order = order;
        }


    }


}

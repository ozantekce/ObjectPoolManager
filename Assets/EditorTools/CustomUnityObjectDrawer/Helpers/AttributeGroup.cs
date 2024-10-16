#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EditorTools
{
    public class AttributeGroup : IComparable<AttributeGroup>, IEnumerable<(BaseAttribute, MemberInfo)>
    {
        private List<(BaseAttribute, MemberInfo)> _attributes;

        public int Order
        {
            get
            {
                if (_attributes == null || _attributes.Count == 0)
                    return 0;

                int min = 0;
                foreach ((BaseAttribute attribute, MemberInfo) a in _attributes)
                {
                    min = Math.Min(min, a.attribute.Order);
                }
                return min;
            }
        }

        public Type AttributeType
        {
            get
            {
                if (_attributes == null || _attributes.Count == 0)
                    return null;
                return _attributes[0].Item1.GetType();
            }
        }

        public (BaseAttribute, MemberInfo) First => _attributes[0];

        public AttributeGroup()
        {
            _attributes = new List<(BaseAttribute, MemberInfo)>();
        }

        public void Add(BaseAttribute attribute, MemberInfo memberInfo)
        {
            _attributes.Add((attribute, memberInfo));
        }

        // Implements IComparable<AttributeGroup>. Compare AttributeGroups based on their Order.
        public int CompareTo(AttributeGroup other)
        {
            if (other == null)
                return 1; // Non-null always greater than null.

            return this.Order.CompareTo(other.Order);
        }

        // Implements IEnumerable<(BaseAttribute, MemberInfo)>.
        public IEnumerator<(BaseAttribute, MemberInfo)> GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }

        // Explicit non-generic interface implementation for IEnumerable.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}


#endif

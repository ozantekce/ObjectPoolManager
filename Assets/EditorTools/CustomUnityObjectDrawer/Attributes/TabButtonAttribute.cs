using System;


namespace EditorTools
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TabButtonAttribute : BaseAttribute
    {
        public string TabMenuName { get; private set; }
        public string TabButtonName { get; private set; }
        public int TabButtonOrder { get; private set; }

        public TabButtonAttribute(string tabMenuName, int order = 0) : base(tabMenuName, order)
        {
            TabMenuName = tabMenuName;
        }

        public TabButtonAttribute(string tabMenuName, string tabButtonName, int order = 0) : base(tabMenuName, order)
        {
            TabMenuName = tabMenuName;
            TabButtonName = tabButtonName;
        }

        public TabButtonAttribute(string tabMenuName, string tabButtonName, int tabButtonOrder, int order = 0) : base(tabMenuName, order)
        {
            TabMenuName = tabMenuName;
            TabButtonName = tabButtonName;
            TabButtonOrder = tabButtonOrder;
        }


    }

}
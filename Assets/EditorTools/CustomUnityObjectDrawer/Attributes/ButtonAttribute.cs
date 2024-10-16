using System;

namespace EditorTools
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : BaseAttribute
    {
        public string ButtonName { get; private set; }

        public ButtonAttribute(string buttonName, int order = 0) : base(order)
        {
            ButtonName = buttonName;
        }
    }

}


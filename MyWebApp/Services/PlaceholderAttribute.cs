using System;

namespace MyWebApp.Services
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PlaceholderAttribute : Attribute
    {
        public string Text { get; }

        public PlaceholderAttribute(string text)
        {
            Text = text;
        }
    }
}
#region Usings

using System.Collections.Generic;
using System.Text;

#endregion

namespace Bluedot.HabboServer.Install
{
    internal abstract class Step
    {
        protected string Description;
        protected ICollection<string> Examples;
        protected string Title;

        internal Step SetTitle(string title)
        {
            Title = title;
            return this;
        }

        internal Step SetDescription(string description)
        {
            Description = description;
            return this;
        }

        internal Step AddExample(string example)
        {
            Examples.Add(example);
            return this;
        }

        internal Step RemoveExample(string example)
        {
            Examples.Remove(example);
            return this;
        }

        protected string ToString(string defaultValue)
        {
            StringBuilder outputBuilder = new StringBuilder();

            outputBuilder.
                Append(">>>   ").
                Append(Title).
                AppendLine("   <<<").
                AppendLine().
                AppendLine(Description).
                Append("Examples: ");

            bool firstExample = true;
            foreach (string example in Examples)
            {
                if (!firstExample)
                    outputBuilder.Append("          ");
                else
                    firstExample = false;
                outputBuilder.AppendLine(example);
            }

            outputBuilder.
                AppendLine().
                Append("Default Value: ").
                AppendLine(defaultValue).
                AppendLine().
                Append("=> ");

            return outputBuilder.ToString();
        }

        internal abstract object Run();
    }
}
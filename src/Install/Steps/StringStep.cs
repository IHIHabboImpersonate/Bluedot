#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Bluedot.HabboServer.Install
{
    internal class StringStep : Step
    {
        private string _default;

        internal StringStep(string title = "", string description = "", ICollection<string> examples = null,
                          string @default = "")
        {
            Title = title;
            Description = description;
            Examples = examples;
            _default = @default;
        }

        internal StringStep SetDefault(string @default)
        {
            _default = @default;
            return this;
        }

        internal override object Run()
        {
            CoreManager.InstallerCore.Out.OverwritePageContents(ToString(_default));

            string inputValue = Console.ReadLine();
            return (inputValue.Length == 0 ? _default : inputValue);
        }
    }
}
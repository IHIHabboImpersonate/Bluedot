#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace IHI.Server.Install
{
    internal class UShortStep : Step
    {
        private ushort _default;
        private ushort _maximum;
        private ushort _minimum;

        internal UShortStep(string title = "", string description = "", ICollection<string> examples = null,
                          ushort @default = (ushort) 0, ushort minimum = ushort.MinValue,
                          ushort maximum = ushort.MaxValue)
        {
            Title = title;
            Description = description;
            Examples = examples;
            _default = @default;
            _minimum = minimum;
            _maximum = maximum;
        }

        internal UShortStep SetDefault(ushort @default)
        {
            _default = @default;
            return this;
        }

        internal UShortStep SetMinimum(ushort minimum)
        {
            _minimum = minimum;
            return this;
        }

        internal UShortStep SetMaximum(ushort maximum)
        {
            _maximum = maximum;
            return this;
        }

        internal override object Run()
        {
            CoreManager.InstallerCore.Out.OverwritePageContents(ToString(_default.ToString(CultureInfo.InvariantCulture)));

            string inputString = Console.ReadLine();

            if (inputString.Length == 0)
                return _default;

            ushort inputValue;
            if (ushort.TryParse(inputString, out inputValue))
            {
                if (inputValue < _minimum)
                    throw new InputException("Given input is lower than mimimum value [ " + inputValue + " < " +
                                             _minimum + " ]");
                if (inputValue > _maximum)
                    throw new InputException("Given input is higher than maximum value [ " + inputValue + " > " +
                                             _maximum + " ]");
                return inputValue;
            }
            throw new InputException("Given input is could not be parsed as an unsigned short [ " + inputValue + " ]");
        }
    }
}
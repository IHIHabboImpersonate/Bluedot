#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Bluedot.HabboServer.Install
{
    internal class Category
    {
        private readonly IDictionary<string, Step> _steps;
        private int _currentStep = 1;

        internal Category()
        {
            _steps = new Dictionary<string, Step>();
        }

        internal Category AddStep(string installerValueId, Step step)
        {
            _steps.Add(installerValueId, step);
            return this;
        }

        internal IDictionary<string, object> Run()
        {
            IDictionary<string, object> installerOutputValues = new Dictionary<string, object>();

            foreach (KeyValuePair<string, Step> step in _steps)
            {
                Retry:
                try
                {
                    object value = step.Value.Run();
                    installerOutputValues.Add(step.Key, value);
                    _currentStep++;
                }
                catch (InputException e)
                {
                    e.Display();

                    Console.SetCursorPosition(3, 15);
                    Console.Write("".PadRight(Console.BufferWidth - 3));
                    Console.SetCursorPosition(3, 15);

                    goto Retry;
                }
            }
            return installerOutputValues;
        }
    }
}
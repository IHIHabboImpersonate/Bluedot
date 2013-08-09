#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Bluedot.HabboServer.Install
{
    internal class InstallerCore
    {
        private readonly IDictionary<string, Category> _categories;
        private readonly IDictionary<string, IDictionary<string, object>> _installerOutputValues;

        internal InstallerCore()
        {
            _categories = new Dictionary<string, Category>();
            _installerOutputValues = new Dictionary<string, IDictionary<string, object>>();

            In = new InstallerIn();
            Out = new InstallerOut();
        }

        internal InstallerIn In { get; private set; }
        internal InstallerOut Out { get; private set; }

        internal InstallerCore AddCategory(string installerCategoryId, Category category)
        {
            _categories.Add(installerCategoryId, category);
            return this;
        }

        internal InstallerCore Run()
        {
            if (_categories.Count == 0)
            {
                Console.WriteLine("No installation tasks detected.");
                return this;
            }
            Console.WriteLine();
            Console.WriteLine("Installation tasks detected - Press any key to continue");

            Console.ReadKey();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;

            foreach (KeyValuePair<string, Category> category in _categories)
            {
                _installerOutputValues.Add(
                    category.Key,
                    category.Value.Run());
            }

            Console.WriteLine("Installation tasks finished - resuming server start up.");
            return this;
        }

        internal object GetInstallerOutputValue(string category, string name)
        {
            if (!_installerOutputValues.ContainsKey(category))
                return null;
            if (!_installerOutputValues[category].ContainsKey(name))
                return null;

            return _installerOutputValues[category][name];
        }
    }
}
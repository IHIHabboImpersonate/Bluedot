#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General internal License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General internal License for more details.
// 
// You should have received a copy of the GNU General internal License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

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
                CoreManager.
                    ServerCore.
                    StandardOut.
                    PrintNotice("Installer", "No installation tasks detected.");
                return this;
            }
            CoreManager.
                ServerCore.
                StandardOut.
                PrintImportant("Installer", "Installation tasks detected!").
                PrintNotice("Standard Out", "Formatting Disabled (Installer)").
                Hidden = true;

            Console.WriteLine("Press any key to continue.");

            Console.ReadKey();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;

            foreach (KeyValuePair<string, Category> category in _categories)
            {
                _installerOutputValues.Add(
                    category.Key,
                    category.Value.Run());
            }

            CoreManager.ServerCore.StandardOut.Hidden = false;
            CoreManager.ServerCore.StandardOut.PrintNotice("Standard Out", "Formatting Enabled (Installer)");
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
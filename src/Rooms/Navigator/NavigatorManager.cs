#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Bluedot.HabboServer.Useful.Collections;

#endregion

namespace Bluedot.HabboServer.Rooms.Navigator
{
    public class NavigatorManager
    {
        #region Properties
        public Category GuestRoot
        {
            get;
            private set;
        }
        public Category PublicRoot
        {
            get;
            private set;
        }
        public Category NonCategory
        {
            get;
            private set;
        }
        #endregion

        #region Fields

        private readonly NestedSetDictionary<string, Category> _guestCategories;
        private readonly NestedSetDictionary<string, Category> _publicCategories;
        private readonly BiDirectionalDictionary<string, int> _numericCategoryIdCache;
        private int _nextUnusedCategoryId = 0;

        #endregion

        #region Constructors

        public NavigatorManager()
        {
            _numericCategoryIdCache = new BiDirectionalDictionary<string, int>();

            PublicRoot = GetCategoryOrCreate("pRoot", "Public Rooms", true);
            _publicCategories = new NestedSetDictionary<string, Category>("pRoot", PublicRoot);

            GuestRoot = GetCategoryOrCreate("gRoot", "Guest Rooms", false);
            _guestCategories = new NestedSetDictionary<string, Category>("gRoot", GuestRoot);

            NonCategory = GetCategoryOrCreate("gLibnavNone", "No Category");

            CoreManager.ServerCore.StandardOut
                .PrintImportant("Navigator Manager => Special categories ready:")
                .PrintImportant("                     Public = " + PublicRoot.ID)
                .PrintImportant("                     Guest = " + GuestRoot.ID)
                .PrintImportant("                     None = " + NonCategory.ID);
        }

        #endregion

        #region Methods

        public Category GetCategoryOrCreate(string idString, string name)
        {
            Category result;
            if (_publicCategories.TryGetValue(idString, out result))
                return result;
            if (_guestCategories.TryGetValue(idString, out result))
                return result;

            int numericalID = _nextUnusedCategoryId;

            Category category = new Category(this)
            {
                ID = numericalID,
                IdString = idString,
                Name = name
            };

            CancelEventArgs eventArgs = new CancelEventArgs();
            CoreManager.ServerCore.EventManager.Fire("navigator_category_created:before", category, eventArgs);

            if (eventArgs.Cancel)
                return null; // TODO: Probably should throw an exception here instead.

            _numericCategoryIdCache.Add(idString, numericalID);
            _nextUnusedCategoryId++;

            CoreManager.ServerCore.EventManager.Fire("navigator_category_created:after", category, EventArgs.Empty);

            CoreManager.ServerCore.StandardOut.PrintDebug("Navigator Manager => Category created: " + numericalID +
                                                               ", idstring, " + name);
            return category;
        }
        private Category GetCategoryOrCreate(string idString, string name, bool isPublicCategory)
        {
            Category result;
            if (_publicCategories.TryGetValue(idString, out result))
                return result;
            if (_guestCategories.TryGetValue(idString, out result))
                return result;

            int numericalID = _nextUnusedCategoryId;

            Category category = new Category(this, isPublicCategory)
            {
                ID = numericalID,
                IdString = idString,
                Name = name
            };

            CancelEventArgs eventArgs = new CancelEventArgs();
            CoreManager.ServerCore.EventManager.Fire("navigator_category_created:before", category, eventArgs);

            if (eventArgs.Cancel)
                return null; // TODO: Probably should throw an exception here instead.

            _numericCategoryIdCache.Add(idString, numericalID);
            _nextUnusedCategoryId++;

            CoreManager.ServerCore.EventManager.Fire("navigator_category_created:after", category, EventArgs.Empty);

            CoreManager.ServerCore.StandardOut.PrintDebug("Navigator Manager => Category created: " + numericalID +
                                                               ", idstring, " + name);
            return category;
        }

        public NavigatorManager UnregisterCategory(Category category)
        {
            if (_publicCategories.ContainsKey(category.IdString))
                throw new NavigatorException("Unable to forget category \"" +
                                             category.IdString + "\" because the category exists in the public tree.");

            if (_guestCategories.ContainsKey(category.IdString))
                throw new NavigatorException("Unable to forget category \"" +
                                             category.IdString + "\" because the category exists in the guest tree.");

            _numericCategoryIdCache.TryRemoveByFirst(category.IdString);
            return this;
        }

        public Category GetCategory(int numericalID)
        {
            string idString;
            if(_numericCategoryIdCache.TryGetBySecond(numericalID, out idString))
            {
                return GetCategory(idString);
            }
            return null;
        }
        
        public Category GetCategory(string idString, NavigatorTreeSearchMode treeSearchMode = NavigatorTreeSearchMode.PublicFirst)
        {
            Category result = null;
            switch (treeSearchMode)
            {
                case NavigatorTreeSearchMode.PublicOnly:
                    {
                        _publicCategories.TryGetValue(idString, out result);
                        break;
                    }
                case NavigatorTreeSearchMode.GuestOnly:
                    {
                        _guestCategories.TryGetValue(idString, out result);
                        break;
                    }
                case NavigatorTreeSearchMode.PublicFirst:
                    {
                        _publicCategories.TryGetValue(idString, out result);
                        if (result == null)
                            _guestCategories.TryGetValue(idString, out result);
                        break;
                    }
                case NavigatorTreeSearchMode.GuestFirst:
                    {
                        _guestCategories.TryGetValue(idString, out result);
                        if (result == null)
                            _publicCategories.TryGetValue(idString, out result);
                        break;
                    }
            }

            return result;
        }

        public IEnumerable<Category> GetChildren(Category category, NavigatorTreeSearchMode treeSearchMode = NavigatorTreeSearchMode.PublicFirst)
        {
            switch (treeSearchMode)
            {
                case NavigatorTreeSearchMode.PublicOnly:
                    {
                        if (!_publicCategories.ContainsKey(category.IdString))
                            throw new NavigatorException("This instance of NavigatorManager does not contain a \"" +
                                                         category.IdString + "\" category. SearchMode: PublicOnly");
                        return _publicCategories.GetChildren(category.IdString);
                    }
                case NavigatorTreeSearchMode.GuestOnly:
                    {
                        if (!_guestCategories.ContainsKey(category.IdString))
                            throw new NavigatorException("This instance of NavigatorManager does not contain a \"" +
                                                         category.IdString + "\" category. SearchMode: GuestOnly");
                        return _guestCategories.GetChildren(category.IdString);
                    }
                case NavigatorTreeSearchMode.PublicFirst:
                    {
                        if (!_publicCategories.ContainsKey(category.IdString))
                            if (!_guestCategories.ContainsKey(category.IdString))
                                throw new NavigatorException("This instance of NavigatorManager does not contain a \"" +
                                                             category.IdString + "\" category. SearchMode: PublicFirst");
                            else
                                return _guestCategories.GetChildren(category.IdString);
                            return _publicCategories.GetChildren(category.IdString);
                    }
                case NavigatorTreeSearchMode.GuestFirst:
                    {
                        if (!_guestCategories.ContainsKey(category.IdString))
                            if (!_publicCategories.ContainsKey(category.IdString))
                                throw new NavigatorException("This instance of NavigatorManager does not contain a \"" +
                                                             category.IdString + "\" category. SearchMode: GuestFirst");
                            else
                                return _publicCategories.GetChildren(category.IdString);
                            return _guestCategories.GetChildren(category.IdString);
                    }
                default:
                    {
                        throw new NavigatorException("Invalid SearchMode.");
                    }
            }
        }

        internal NavigatorManager LinkCategory(Category category, Category parent, NavigatorTreeSearchMode treeSearchMode = NavigatorTreeSearchMode.PublicFirst)
        {
            switch (treeSearchMode)
            {
                case NavigatorTreeSearchMode.PublicOnly:
                    {
                        if (!_publicCategories.ContainsKey(parent.IdString))
                            throw new NavigatorException("This instance of NavigatorManager does not contain the parent category \"" +
                                                         parent.IdString + "\". SearchMode: PublicOnly");
                        _publicCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        break;
                    }
                case NavigatorTreeSearchMode.GuestOnly:
                    {
                        if (!_guestCategories.ContainsKey(parent.IdString))
                            throw new NavigatorException("This instance of NavigatorManager does not contain the parent category \"" +
                                                         parent.IdString + "\". SearchMode: GuestOnly");
                        _guestCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        break;
                    }
                case NavigatorTreeSearchMode.PublicFirst:
                    {
                        if (!_publicCategories.ContainsKey(parent.IdString))
                            if (!_guestCategories.ContainsKey(parent.IdString))
                                throw new NavigatorException("This instance of NavigatorManager does not contain the parent category \"" +
                                                             parent.IdString + "\". SearchMode: PublicFirst");
                            else
                                _guestCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        else
                            _publicCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        break;
                    }
                case NavigatorTreeSearchMode.GuestFirst:
                    {
                        if (!_guestCategories.ContainsKey(parent.IdString))
                            if (!_publicCategories.ContainsKey(parent.IdString))
                                throw new NavigatorException("This instance of NavigatorManager does not contain the parent category \"" +
                                                             parent.IdString + "\". SearchMode: GuestFirst");
                            else
                                _publicCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        else
                            _guestCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        break;
                    }
            }
            CoreManager.ServerCore.StandardOut.PrintDebug("Navigator Manager => Category " + category.IdString + " adopted by " + parent.IdString);
            return this;
        }

        internal NavigatorManager UnlinkCategory(Category category, NavigatorTreeSearchMode treeSearchMode = NavigatorTreeSearchMode.PublicFirst, NestedSetRemoveChildAction childAction = NestedSetRemoveChildAction.MoveUpGeneration)
        {
            switch (treeSearchMode)
            {
                case NavigatorTreeSearchMode.PublicOnly:
                    {
                        if (!_publicCategories.ContainsKey(category.IdString))
                            throw new NavigatorException("This instance of NavigatorManager does not contain a \"" +
                                                         category.IdString + "\" category. SearchMode: PublicOnly");
                        _publicCategories.Remove(category.IdString, childAction);
                        break;
                    }
                case NavigatorTreeSearchMode.GuestOnly:
                    {
                        if (!_guestCategories.ContainsKey(category.IdString))
                            throw new NavigatorException("This instance of NavigatorManager does not contain a \"" +
                                                         category.IdString + "\" category. SearchMode: GuestOnly");
                        _guestCategories.Remove(category.IdString, childAction);
                        break;
                    }
                case NavigatorTreeSearchMode.PublicFirst:
                    {
                        if (!_publicCategories.ContainsKey(category.IdString))
                            if (!_guestCategories.ContainsKey(category.IdString))
                                throw new NavigatorException("This instance of NavigatorManager does not contain a \"" +
                                                             category.IdString + "\" category. SearchMode: PublicFirst");
                            else
                                _guestCategories.Remove(category.IdString, childAction);
                        else
                            _publicCategories.Remove(category.IdString, childAction);
                        break;
                    }
                case NavigatorTreeSearchMode.GuestFirst:
                    {
                        if (!_guestCategories.ContainsKey(category.IdString))
                            if (!_publicCategories.ContainsKey(category.IdString))
                                throw new NavigatorException("This instance of NavigatorManager does not contain a \"" +
                                                             category.IdString + "\" category. SearchMode: GuestFirst");
                            else
                                _publicCategories.Remove(category.IdString, childAction);
                        else
                            _guestCategories.Remove(category.IdString, childAction);
                        break;
                    }
            }
            return this;
        }

        #endregion
    }
}
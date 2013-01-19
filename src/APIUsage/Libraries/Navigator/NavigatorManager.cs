using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bluedot.HabboServer.Events;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.APIUsage.Libraries.Navigator
{
    public class NavigatorManager
    {
        #region Fields
        #region Field: _guestCategories
        private readonly NestedSetDictionary<string, Category> _guestCategories;
        #endregion
        #region Field: _publicCategories
        private readonly NestedSetDictionary<string, Category> _publicCategories;
        #endregion
        #region Field: _numericCategoryIdCache
        private readonly BiDirectionalDictionary<string, int> _numericCategoryIdCache;
        #endregion
        #region Field: _nextUnusedCategoryId
        private int _nextUnusedCategoryId = 0;
        #endregion
        #endregion

        #region Properties
        #region Property: GuestRoot
        public Category GuestRoot
        {
            get;
            private set;
        }
        #endregion
        #region Property: PublicRoot
        public Category PublicRoot
        {
            get;
            private set;
        }
        #endregion
        #region Property: NonCategory
        public Category NonCategory
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: NavigatorManager (Constructor)
        public NavigatorManager()
        {
            _numericCategoryIdCache = new BiDirectionalDictionary<string, int>();

            PublicRoot = NewCategory("pLibnavRoot", "Public Rooms");
            GuestRoot = NewCategory("gLibnavRoot", "Guest Rooms");
            NonCategory = NewCategory("gLibnavNone", "No Category");
            PublicRoot.IsPublicCategory = true;
            GuestRoot.IsPublicCategory = false;
            NonCategory.IsPublicCategory = false;

            _publicCategories = new NestedSetDictionary<string, Category>("pLibnavRoot", PublicRoot);
            _guestCategories = new NestedSetDictionary<string, Category>("gLibnavRoot", GuestRoot);

            CoreManager.ServerCore.StandardOut
                .PrintImportant("Navigator Manager", "Special categories ready:")
                .PrintImportant("Navigator Manager", "Public = " + PublicRoot.Id)
                .PrintImportant("Navigator Manager", "Guest = " + GuestRoot.Id)
                .PrintImportant("Navigator Manager", "None = " + NonCategory.Id);
        }
        #endregion

        #region Method: NewCategory
        public Category NewCategory(string idString, string name)
        {
            // TODO: navigator-category_created event

            Category result;
            if (_publicCategories != null && _publicCategories.TryGetValue(idString, out result))
                return result;
            if (_guestCategories != null && _guestCategories.TryGetValue(idString, out result))
                return result;

            int numericalId = _nextUnusedCategoryId++;
            _numericCategoryIdCache.Add(idString, numericalId);

            Category category = new Category
            {
                Id = numericalId,
                IdString = idString,
                Name = name
            };


            CoreManager.ServerCore.StandardOut.PrintDebug("Navigator Manager", "Category created: " + numericalId + "," + idString + "," + name);
            return category;
        }
        #endregion

        #region Method: ForgetCategory
        public NavigatorManager ForgetCategory(Category category)
        {
            if (_publicCategories.ContainsKey(category.IdString))
                throw new Exception("Unable to forget category \"" + category.IdString + "\" because the category exists in the public tree.");

            if (_guestCategories.ContainsKey(category.IdString))
                throw new Exception("Unable to forget category \"" + category.IdString + "\" because the category exists in the guest tree.");

            _numericCategoryIdCache.TryRemoveByFirst(category.IdString);
            return this;
        }
        #endregion

        #region Method: GetCategory
        public Category GetCategory(int numericalId)
        {
            string idString;
            if (_numericCategoryIdCache.TryGetBySecond(numericalId, out idString))
            {
                return GetCategory(idString);
            }
            return null;
        }

        public Category GetCategory(string idString, TreeSearchMode treeSearchMode = TreeSearchMode.PublicFirst)
        {
            Category result = null;
            switch (treeSearchMode)
            {
                case TreeSearchMode.PublicOnly:
                    {
                        _publicCategories.TryGetValue(idString, out result);
                        break;
                    }
                case TreeSearchMode.GuestOnly:
                    {
                        _guestCategories.TryGetValue(idString, out result);
                        break;
                    }
                case TreeSearchMode.PublicFirst:
                    {
                        _publicCategories.TryGetValue(idString, out result);
                        if (result == null)
                            _guestCategories.TryGetValue(idString, out result);
                        break;
                    }
                case TreeSearchMode.GuestFirst:
                    {
                        _guestCategories.TryGetValue(idString, out result);
                        if (result == null)
                            _publicCategories.TryGetValue(idString, out result);
                        break;
                    }
                default:
                    {
                        throw new Exception("Invalid TreeSearchMode.");
                    }
            }

            return result;
        }
        #endregion

        #region Method: GetChildren
        public ICollection<Category> GetChildren(Category category, TreeSearchMode treeSearchMode = TreeSearchMode.PublicFirst)
        {
            switch (treeSearchMode)
            {
                case TreeSearchMode.PublicOnly:
                    {
                        if (!_publicCategories.ContainsKey(category.IdString))
                            throw new Exception("The Navigator Manager does not contain a \"" + category.IdString + "\" category. SearchMode: PublicOnly");
                        return _publicCategories.GetChildren(category.IdString);
                    }
                case TreeSearchMode.GuestOnly:
                    {
                        if (!_guestCategories.ContainsKey(category.IdString))
                            throw new Exception("The Navigator Manager does not contain a \"" + category.IdString + "\" category. SearchMode: GuestOnly");
                        return _guestCategories.GetChildren(category.IdString);
                    }
                case TreeSearchMode.PublicFirst:
                    {
                        if (!_publicCategories.ContainsKey(category.IdString))
                            if (!_guestCategories.ContainsKey(category.IdString))
                                throw new Exception("The Navigator Manager does not contain a \"" + category.IdString + "\" category. SearchMode: PublicFirst");
                            else
                                return _guestCategories.GetChildren(category.IdString);
                        return _publicCategories.GetChildren(category.IdString);
                    }
                case TreeSearchMode.GuestFirst:
                    {
                        if (!_guestCategories.ContainsKey(category.IdString))
                            if (!_publicCategories.ContainsKey(category.IdString))
                                throw new Exception("The Navigator Manager does not contain a \"" + category.IdString + "\" category. SearchMode: GuestFirst");
                            else
                                return _publicCategories.GetChildren(category.IdString);
                        return _guestCategories.GetChildren(category.IdString);
                    }
                default:
                    {
                        throw new Exception("Invalid TreeSearchMode.");
                    }
            }
        }

        internal NavigatorManager AddCategory(Category category, Category parent, TreeSearchMode treeSearchMode = TreeSearchMode.PublicFirst)
        {
            switch (treeSearchMode)
            {
                case TreeSearchMode.PublicOnly:
                    {
                        if (!_publicCategories.ContainsKey(parent.IdString))
                            throw new Exception("The Navigator Manager does not contain a \"" + parent.IdString + "\" category. SearchMode: PublicOnly");
                        _publicCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        break;
                    }
                case TreeSearchMode.GuestOnly:
                    {
                        if (!_guestCategories.ContainsKey(parent.IdString))
                            throw new Exception("The Navigator Manager does not contain a \"" + parent.IdString + "\" category. SearchMode: GuestOnly");
                        _guestCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        break;
                    }
                case TreeSearchMode.PublicFirst:
                    {
                        if (!_publicCategories.ContainsKey(parent.IdString))
                            if (!_guestCategories.ContainsKey(parent.IdString))
                                throw new Exception("The Navigator Manager does not contain a \"" + parent.IdString + "\" category. SearchMode: PublicFirst");
                            else
                                _guestCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        else
                            _publicCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        break;
                    }
                case TreeSearchMode.GuestFirst:
                    {
                        if (!_guestCategories.ContainsKey(parent.IdString))
                            if (!_publicCategories.ContainsKey(parent.IdString))
                                throw new Exception("The Navigator Manager does not contain a \"" + parent.IdString + "\" category. SearchMode: GuestFirst");
                            else
                                _publicCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        else
                            _guestCategories.AddAsChildOf(category.IdString, category, parent.IdString);
                        break;
                    }
                default:
                    {
                        throw new Exception("Invalid TreeSearchMode.");
                    }
            }
            CoreManager.ServerCore.StandardOut.PrintDebug("Navigator Manager", "Category " + category.IdString + " adopted by " + parent.IdString);
            return this;
        }

        internal NavigatorManager RemoveCategory(Category category, TreeSearchMode treeSearchMode = TreeSearchMode.PublicFirst, NestedSetRemoveChildAction childAction = NestedSetRemoveChildAction.MoveUpGeneration)
        {
            switch (treeSearchMode)
            {
                case TreeSearchMode.PublicOnly:
                    {
                        if (!_publicCategories.ContainsKey(category.IdString))
                            throw new Exception("The Navigator Manager does not contain a \"" + category.IdString + "\" category. SearchMode: PublicOnly");
                        _publicCategories.Remove(category.IdString, childAction);
                        break;
                    }
                case TreeSearchMode.GuestOnly:
                    {
                        if (!_guestCategories.ContainsKey(category.IdString))
                            throw new Exception("The Navigator Manager does not contain a \"" + category.IdString + "\" category. SearchMode: GuestOnly");
                        _guestCategories.Remove(category.IdString, childAction);
                        break;
                    }
                case TreeSearchMode.PublicFirst:
                    {
                        if (!_publicCategories.ContainsKey(category.IdString))
                            if (!_guestCategories.ContainsKey(category.IdString))
                                throw new Exception("The Navigator Manager does not contain a \"" + category.IdString + "\" category. SearchMode: PublicFirst");
                            else
                                _guestCategories.Remove(category.IdString, childAction);
                        else
                            _publicCategories.Remove(category.IdString, childAction);
                        break;
                    }
                case TreeSearchMode.GuestFirst:
                    {
                        if (!_guestCategories.ContainsKey(category.IdString))
                            if (!_publicCategories.ContainsKey(category.IdString))
                                throw new Exception("The Navigator Manager does not contain a \"" + category.IdString + "\" category. SearchMode: GuestFirst");
                            else
                                _publicCategories.Remove(category.IdString, childAction);
                        else
                            _guestCategories.Remove(category.IdString, childAction);
                        break;
                    }
                default:
                    {
                        throw new Exception("Invalid TreeSearchMode.");
                    }
            }
            return this;
        }

        #endregion
        #endregion
    }
}

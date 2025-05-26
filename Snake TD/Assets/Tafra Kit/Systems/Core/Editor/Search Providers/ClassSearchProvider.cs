using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TafraKit;
using System.Reflection;

namespace TafraKitEditor
{
    public class ClassSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private static ClassSearchProvider activeInstance;

        private string title;
        private Type baseType;
        private Type nextShowBaseType;
        private List<Type> nextShowExcludedTypes = new List<Type>();
        private Action<Type, SearchWindowContext> onSelect;
        private Texture2D indentationTex;

        private struct ClassSearchElement
        {
            public Type type;
            public string menuItem;
             
            public ClassSearchElement(Type type, string menuItem)
            { 
                this.type = type;
                this.menuItem = menuItem;
            }
        }

        public static ClassSearchProvider CreateOrGetInstance()
        {
            if(activeInstance != null)
                return activeInstance;

            activeInstance = CreateInstance<ClassSearchProvider>();

            return activeInstance;
        }

        public void Initialize(Type baseType, string title, Action<Type, SearchWindowContext> onSelect)
        {
            this.title = title;
            this.baseType = baseType;
            this.onSelect = onSelect;

            indentationTex = new Texture2D(1, 1);
            indentationTex.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationTex.Apply();
        }
        public void SetNextShowBaseType(Type newBaseType)
        {
            nextShowBaseType = newBaseType;
        }
        public void AddNextShowExcludedType(Type excludedType)
        {
            nextShowExcludedTypes.Add(excludedType);
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            Type searchBaseType = nextShowBaseType != null ? nextShowBaseType :  baseType;

            
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();

            entries.Add(new SearchTreeGroupEntry(new GUIContent(title), 0));

            List<ClassSearchElement> searchElements = new List<ClassSearchElement>();

            //Extract available types and their corresponding menu path
            var allTypes = TypeCache.GetTypesDerivedFrom(searchBaseType);
            for(int i = 0; i < allTypes.Count; i++)
            {
                Type type = allTypes[i];

                if(type.IsAbstract || type.GetCustomAttribute<RemoveFromSearchMenu>() != null)
                    continue;

                bool exclude = false;
                for (int j = 0; j < nextShowExcludedTypes.Count; j++)
                {
                    Type excludedType = nextShowExcludedTypes[j];

                    if (type == excludedType)
                    {
                        exclude = true;
                        break;
                    }

                    bool excludeBecauseOfBase = false;
                    Type baseType = type.BaseType;
                    while (baseType != null)
                    {
                        if (baseType == excludedType)
                        {
                            excludeBecauseOfBase = true;
                            break;
                        }

                        baseType = baseType.BaseType;
                    }

                    if (excludeBecauseOfBase)
                    {
                        exclude = true;
                        break;
                    }
                }

                if (exclude)
                    continue;

                if (type.GetCustomAttribute<SearchMenuItem>() is SearchMenuItem searchMenuItem)
                {
                    searchElements.Add(new ClassSearchElement(type, searchMenuItem.MenuName));
                }
                else
                {
                    string menuName = $"Uncategorized/{ObjectNames.NicifyVariableName(type.Name)}";

                    searchElements.Add(new ClassSearchElement(type, menuName));
                }
            }

            //Sort elements by menu name
            searchElements.Sort((entry1, entry2) =>
            {
                string[] splits1 = entry1.menuItem.Split('/');
                string[] splits2 = entry2.menuItem.Split('/');

                for(int i = 0; i < splits1.Length; i++)
                {
                    if(i >= splits2.Length)
                        return 1;

                    int value = splits1[i].CompareTo(splits2[i]);
                    if(value != 0)
                    {
                        //Make sure that leaves go before nodes
                        if(splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                            return splits1.Length < splits2.Length ? 1 : -1;

                        return value;
                    }
                }

                return 0;
            });

            List<string> groups = new List<string>();
            for(int i = 0; i < searchElements.Count; i++)
            {
                var element = searchElements[i];

                Type type = element.type;
                string menuItem = element.menuItem;
                string[] menuItemSections = menuItem.Split('/');
                string groupName = "";

                for(int j = 0; j < menuItemSections.Length -1; j++)
                {
                    groupName += menuItemSections[j];
                    if(!groups.Contains(groupName))
                    {
                        entries.Add(new SearchTreeGroupEntry(new GUIContent(menuItemSections[j]), j + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(menuItemSections[menuItemSections.Length - 1], indentationTex));
                entry.level = menuItemSections.Length;
                entry.userData = type;
                entries.Add(entry);
            }

            nextShowBaseType = null;
            nextShowExcludedTypes.Clear();

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            onSelect?.Invoke(SearchTreeEntry.userData as Type, context);
            return true;
        }
    }
}

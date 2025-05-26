using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TafraKit;
using TreeEditor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    public class TafraSettingsWindow : EditorWindow
    {
        public class TreeElement
        {
            public SettingsModule settings;
            public string name;
            public int id;
            public int priority;
            public List<TreeElement> nestedSettings;

            public TreeElement()
            {
                nestedSettings = new List<TreeElement>();
                priority = 5000;
            }
        }

        private const string darkModeIconPath = "Tafra Kit/Icons/d_TafraSettings.png";
        private const string lightModeIconPath = "Tafra Kit/Icons/TafraSettings.png";

        private TreeView settingsTree;
        private List<TreeElement> treeElements = new List<TreeElement>();
        private TypeCache.TypeCollection allSettingTypes;
        private static List<SettingsModule> existingSettings = new List<SettingsModule>();
        private static HashSet<SettingsModule> existingSettingsHashSet = new HashSet<SettingsModule>();
        private static HashSet<Type> existingSettingTypesHashSet = new HashSet<Type>();
        private VisualElement settingsInspectorContent;
        private Label settingsInspectorTitle;
        private Label settingsInspectorDescription;
        private Editor activeSettingsEditor;
        private TreeElement selectedTreeElement;

        private static Dictionary<string, int> priorityByCategory = new Dictionary<string, int>()
        {
            //Game Specific
            { "Dungeons of Souls", 20 },
                { "Dungeon Management", 0 },

            //Main
            { "General", 10 },
            { "Editor", 5001 },
        };

        [MenuItem("Tafra Games/Settings", priority = 0)]
        public static void OpenWindow()
        {
            TafraSettingsWindow window = GetWindow<TafraSettingsWindow>();

            string iconPath = EditorGUIUtility.isProSkin ? darkModeIconPath : lightModeIconPath;
            Texture icon = EditorGUIUtility.Load(iconPath) as Texture;

            window.titleContent = new GUIContent("Tafra Settings", icon);

            GetWindow<TafraSettingsWindow>();
        }

        private void OnEnable()
        {
            allSettingTypes = TypeCache.GetTypesDerivedFrom<SettingsModule>();
            UpdateExistingSettingsList();
            CreateNonExistingSettingFiles();

            ////////////////////////////////////////

            #region Build Tree Elements
            treeElements.Clear();
            int count = 0;
            for(int i = 0; i < existingSettings.Count; i++)
            {
                var settings = existingSettings[i];

                string[] segments = settings.Name.Split('/');

                List<TreeElement> curList = treeElements;
                TreeElement curElement = null;
                for(int segmentIndex = 0; segmentIndex < segments.Length; segmentIndex++)
                {
                    var segmentString = segments[segmentIndex];

                    if(segmentIndex < segments.Length - 1)
                    {
                        //This is a category or sub-category.
                        for(int nestedCategoryIndex = 0; nestedCategoryIndex <= segmentIndex; nestedCategoryIndex++)
                        {
                            TreeElement element = curElement;

                            if((curElement != null && curElement.name == segmentString) || TryGetTreeElementInList(curList, segmentString, out element))
                            {
                                //Tree element already exist.
                                curList = element.nestedSettings;
                                curElement = element;
                            }
                            else
                            {
                                //Should create the tree element.
                                TreeElement newElement = new TreeElement();
                                newElement.name = segmentString;
                                newElement.id = count;

                                if(priorityByCategory.TryGetValue(segmentString, out var priority))
                                    newElement.priority = priority;

                                curList.Add(newElement);
                                curList = newElement.nestedSettings;
                                curElement = newElement;

                                count++;
                            }
                        }
                    }
                    else
                    {
                        //This is the actual settings.

                        TreeElement element = curElement;

                        //If the settings element already exists as a category, then also assign the settings object to it.
                        if((curElement != null && curElement.name == segmentString) || TryGetTreeElementInList(curList, segmentString, out element))
                        {
                            if(settings != null)
                            {
                                element.settings = settings;
                                element.priority = settings.Priority;
                            }

                            curList = element.nestedSettings;

                            curElement = element;
                        }
                        else
                        {
                            TreeElement newElement = new TreeElement();
                            newElement.name = segmentString;
                            newElement.id = count;
                            newElement.priority = settings.Priority;
                            newElement.settings = settings;
                            curList.Add(newElement);
                            count++;
                        }
                    }
                }
            }

            treeElements = treeElements.OrderBy(x => x.priority).ToList();

            //Order tree elements
            for(int i = 0; i < treeElements.Count; i++)
            {
                SortNestedElements(treeElements[i]);
            }
            #endregion

            //Draw elements
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/Systems/Settings/Editor/UXML/TafraSettingsWindow.uxml");
            uxml.CloneTree(rootVisualElement);

            VisualElement settingsContainer = rootVisualElement.Q<VisualElement>("settings-container");
            settingsInspectorContent = settingsContainer.Q<VisualElement>("content");
            settingsInspectorTitle = settingsContainer.Q<Label>("title");
            settingsInspectorDescription = settingsContainer.Q<Label>("description");

            VisualElement settingsListContainer = rootVisualElement.Q<VisualElement>("settings-list-container");

            #region Setup Tree
            settingsTree = settingsListContainer.Q<TreeView>("settings-tree");
            settingsTree.viewDataKey = "tafra-settings";

            // The "makeItem" function will be called as needed
            // when the TreeView needs more items to render
            Func<VisualElement> makeItem = CreateTreeElement;

            // As the user scrolls through the list, the TreeView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var item = settingsTree.GetItemDataForIndex<TreeElement>(i);

                Label label = e.Q<Label>();
                label.text = item.name;
            };

            settingsTree.makeItem = makeItem;
            settingsTree.bindItem = bindItem;
            settingsTree.selectionType = SelectionType.Single;

            // Callback invoked when the user double clicks an item
            settingsTree.itemsChosen += OnTreeItemChosen;

            // Callback invoked when the user changes the selection inside the TreeView
            settingsTree.selectedIndicesChanged += OnTreeSelectedIndicesChanged;

            BuildFullTree();
            #endregion

            //Search bar
            ToolbarSearchField searchField = settingsListContainer.Q<ToolbarSearchField>();
            searchField.RegisterValueChangedCallback(OnSearchFieldValueChange);
        }

        private void OnSearchFieldValueChange(ChangeEvent<string> evt)
        {
            BuildSearchTree(evt.newValue);
        }

        private void OnTreeItemChosen(IEnumerable<object> enumerable)
        {
            //Items double clicked
        }
        private void OnTreeSelectedIndicesChanged(IEnumerable<int> enumerable)
        {
            //Item clicked

            foreach(int i in enumerable)
            {
                TreeElement element = settingsTree.GetItemDataForIndex<TreeElement>(i);

                if(element.settings != null)
                    BuildSettingsInspector(element);
                else
                    BuildCategoryInspector(element);

                selectedTreeElement = element;

                //We only allow the selection of 1 item at a time.
                break;
            }
        }

        private void BuildFullTree()
        {
            if(settingsTree.childCount > 0)
            {
                settingsTree.Clear();
            }

            var items = new List<TreeViewItemData<TreeElement>>();
            for(int i = 0; i < treeElements.Count; i++)
            {
                TreeElement mainElement = treeElements[i];

                List<TreeViewItemData<TreeElement>> nestedElements = BuildFullNestedTreeList(mainElement);

                TreeViewItemData<TreeElement> item = new TreeViewItemData<TreeElement>(mainElement.id, mainElement, nestedElements);

                items.Add(item);
            }

            settingsTree.SetRootItems(items);
            settingsTree.Rebuild();

            if(selectedTreeElement == null)
                settingsTree.selectedIndex = 0;
        }
        private void BuildSearchTree(string searchValue)
        {
            if(string.IsNullOrEmpty(searchValue))
            {
                BuildFullTree();
                return;
            }

            searchValue = searchValue.ToLower();

            if(settingsTree.childCount > 0)
                settingsTree.Clear();

            var items = new List<TreeViewItemData<TreeElement>>();
            for(int i = 0; i < treeElements.Count; i++)
            {
                TreeElement mainElement = treeElements[i];

                if(mainElement.name.ToLower().Contains(searchValue))
                    items.Add(new TreeViewItemData<TreeElement>(mainElement.id, mainElement));

                ExtractFlatNestedTreeList(mainElement, items, searchValue);
            }

            settingsTree.SetRootItems(items);
            settingsTree.Rebuild();
        }
        private void BuildSettingsInspector(TreeElement treeElement)
        {
            SettingsModule settings = treeElement.settings;

            settingsInspectorTitle.text = treeElement.name;
            settingsInspectorDescription.text = settings.Description;

            settingsInspectorContent.Clear();

            SerializedObject so = new SerializedObject(settings);

            ScrollView sv = new ScrollView();

            activeSettingsEditor = Editor.CreateEditor(settings);

            var element = activeSettingsEditor.CreateInspectorGUI();

            if(element != null)
                sv.Add(element);
            else
            {
                IMGUIContainer imguiContainer = new IMGUIContainer(() =>
                {
                    activeSettingsEditor.OnInspectorGUI();
                });

                sv.Add(imguiContainer);
            }

            sv.Bind(so);

            settingsInspectorContent.Add(sv);
        }
        private void BuildCategoryInspector(TreeElement category)
        {
            settingsInspectorTitle.text = category.name;
            settingsInspectorDescription.text = category.name;

            settingsInspectorContent.Clear();
            activeSettingsEditor = null;
        }
        private VisualElement CreateTreeElement()
        {
            VisualElement element = new VisualElement();
            element.style.flexGrow = 1;

            Label label = new Label();
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;

            element.Add(label);

            return element;
        }

        private void UpdateExistingSettingsList()
        {
            SettingsModule[] presentSettings = Resources.LoadAll<SettingsModule>("Tafra Settings");

            for(int i = 0; i < presentSettings.Length; i++)
            {
                var setting = presentSettings[i];

                if(!existingSettingsHashSet.Contains(setting))
                    RegisterExistingSettings(setting);
            }
        }
        private void CreateNonExistingSettingFiles()
        {
            string localPath = "Resources/Tafra Settings/";

            if(!Directory.Exists(Application.dataPath + "/" + localPath))
            {
                Directory.CreateDirectory(Application.dataPath + "/" + localPath);
                Debug.Log("Created directory");
            }

            for(int i = 0; i < allSettingTypes.Count; i++)
            {
                Type settingType = allSettingTypes[i];

                if(!existingSettingTypesHashSet.Contains(settingType))
                {
                    TafraDebugger.Log("Tafra Settings Window", $"Creating a new settings file for the type {settingType.Name}.", TafraDebugger.LogType.Info);

                    CreateSettingsFile(settingType);

                    //SettingsModule so = Resources.Load<SettingsModule>("Tafra Settings/" + settingType.Name);
                    //Debug.Log(so);
                    //RegisterExistingSettings(so);
                }
            }
        }
        private void CreateSettingsFile(Type settingsType)
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(settingsType);

            AssetDatabase.CreateAsset(asset, "Assets/Resources/Tafra Settings/" + settingsType.Name + ".asset");
            AssetDatabase.SaveAssets();

            if(!existingSettingsHashSet.Contains(asset))
                RegisterExistingSettings(asset as SettingsModule);
        }
        private void RegisterExistingSettings(SettingsModule settings)
        {
            existingSettings.Add(settings);
            existingSettingsHashSet.Add(settings);
            existingSettingTypesHashSet.Add(settings.GetType());
        }
        private bool TryGetTreeElementInList(List<TreeElement> list, string elementName, out TreeElement element)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].name == elementName)
                {
                    element = list[i];
                    return true;
                }
            }

            element = null;
            return false;
        }
        private List<TreeViewItemData<TreeElement>> BuildFullNestedTreeList(TreeElement mainElement)
        {
            List<TreeViewItemData<TreeElement>> mainElementNestedList = new List<TreeViewItemData<TreeElement>>();

            for(int i = 0; i < mainElement.nestedSettings.Count; i++)
            {
                var nestedElement = mainElement.nestedSettings[i];

                List<TreeViewItemData<TreeElement>> nestedElementNestedList = BuildFullNestedTreeList(nestedElement);

                mainElementNestedList.Add(new TreeViewItemData<TreeElement>(nestedElement.id, nestedElement, nestedElementNestedList));
            }

            return mainElementNestedList;
        }
        private List<TreeViewItemData<TreeElement>> BuildFullNestedTreeList(TreeElement mainElement, string searchValue)
        {
            List<TreeViewItemData<TreeElement>> mainElementNestedList = new List<TreeViewItemData<TreeElement>>();

            for(int i = 0; i < mainElement.nestedSettings.Count; i++)
            {
                var nestedElement = mainElement.nestedSettings[i];

                List<TreeViewItemData<TreeElement>> nestedElementNestedList = BuildFullNestedTreeList(nestedElement, searchValue);

                //If this is a category, and it's empty, then ignore it.
                if(nestedElement.settings == null && nestedElementNestedList.Count == 0)
                    continue;
                else if(nestedElement.settings != null && !nestedElement.name.Contains(searchValue))
                    continue;

                mainElementNestedList.Add(new TreeViewItemData<TreeElement>(nestedElement.id, nestedElement, nestedElementNestedList));
            }

            return mainElementNestedList;
        }
        private void ExtractFlatNestedTreeList(TreeElement mainElement, List<TreeViewItemData<TreeElement>> listToFill, string searchValue)
        {
            for(int i = 0; i < mainElement.nestedSettings.Count; i++)
            {
                var nestedElement = mainElement.nestedSettings[i];

                if(nestedElement.name.ToLower().Contains(searchValue))
                    listToFill.Add(new TreeViewItemData<TreeElement>(nestedElement.id, nestedElement));

                ExtractFlatNestedTreeList(nestedElement, listToFill, searchValue);
            }
        }
        private void SortNestedElements(TreeElement treeElement)
        {
            treeElement.nestedSettings = treeElement.nestedSettings.OrderBy(x => x.priority).ToList();

            for(int i = 0; i < treeElement.nestedSettings.Count; i++)
            {
                SortNestedElements(treeElement.nestedSettings[i]);
            }
        }
    }
}
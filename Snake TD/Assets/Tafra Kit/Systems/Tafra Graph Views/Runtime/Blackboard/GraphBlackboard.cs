using System;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    [Serializable]
    public class GraphBlackboard
    {
        private static List<Type> supportedTypes = new List<Type>()
        {
            typeof(float),
            typeof(TafraAdvancedFloat),
            typeof(int),
            typeof(bool),
            typeof(string),
            typeof(Vector3),
            typeof(GameObject),
            typeof(ScriptableObject),
            typeof(TafraActor),
            typeof(UnityEngine.Object),
            typeof(object)
        };

        public static List<Type> SupportedTypes => supportedTypes;

        [SerializeField] protected List<GenericExposableProperty<float>> floatProperties = new List<GenericExposableProperty<float>>();
        [SerializeField] protected List<GenericExposableProperty<TafraAdvancedFloat>> advancedFloatProperties = new List<GenericExposableProperty<TafraAdvancedFloat>>();
        [SerializeField] protected List<GenericExposableProperty<int>> intProperties = new List<GenericExposableProperty<int>>();
        [SerializeField] protected List<GenericExposableProperty<bool>> boolProperties = new List<GenericExposableProperty<bool>>();
        [SerializeField] protected List<GenericExposableProperty<string>> stringProperties = new List<GenericExposableProperty<string>>();
        [SerializeField] protected List<GenericExposableProperty<Vector3>> vector3Properties = new List<GenericExposableProperty<Vector3>>();
        [SerializeField] protected List<GenericExposableProperty<GameObject>> gameObjectProperties = new List<GenericExposableProperty<GameObject>>();
        [SerializeField] protected List<GenericExposableProperty<ScriptableObject>> scriptableObjectProperties = new List<GenericExposableProperty<ScriptableObject>>();
        [SerializeField] protected List<GenericExposableProperty<TafraActor>> actorProperties = new List<GenericExposableProperty<TafraActor>>();
        [SerializeField] protected List<GenericExposableProperty<UnityEngine.Object>> objectProperties = new List<GenericExposableProperty<UnityEngine.Object>>();
        [SerializeField] protected List<GenericExposableProperty<object>> systemObjectProperties = new List<GenericExposableProperty<object>>();
        [SerializeField] protected int nextPropertyID;

        private Dictionary<int, GenericExposableProperty<float>> floatPropertiesByName;
        private Dictionary<int, GenericExposableProperty<TafraAdvancedFloat>> advancedFloatPropertiesByName;
        private Dictionary<int, GenericExposableProperty<int>> intPropertiesByName;
        private Dictionary<int, GenericExposableProperty<bool>> boolPropertiesByName;
        private Dictionary<int, GenericExposableProperty<string>> stringPropertiesByName;
        private Dictionary<int, GenericExposableProperty<Vector3>> vector3PropertiesByName;
        private Dictionary<int, GenericExposableProperty<GameObject>> gameObjectPropertiesByName;
        private Dictionary<int, GenericExposableProperty<ScriptableObject>> scriptableObjectPropertiesByName;
        private Dictionary<int, GenericExposableProperty<TafraActor>> actorPropertiesByName;
        private Dictionary<int, GenericExposableProperty<UnityEngine.Object>> objectPropertiesByName;
        private Dictionary<int, GenericExposableProperty<object>> systemObjectPropertiesByName;
       
        private Dictionary<int, GenericExposableProperty<float>> floatPropertiesByID;
        private Dictionary<int, GenericExposableProperty<TafraAdvancedFloat>> advancedFloatPropertiesByID;
        private Dictionary<int, GenericExposableProperty<int>> intPropertiesByID;
        private Dictionary<int, GenericExposableProperty<bool>> boolPropertiesByID;
        private Dictionary<int, GenericExposableProperty<string>> stringPropertiesByID;
        private Dictionary<int, GenericExposableProperty<Vector3>> vector3PropertiesByID;
        private Dictionary<int, GenericExposableProperty<GameObject>> gameObjectPropertiesByID;
        private Dictionary<int, GenericExposableProperty<ScriptableObject>> scriptableObjectPropertiesByID;
        private Dictionary<int, GenericExposableProperty<TafraActor>> actorPropertiesByID;
        private Dictionary<int, GenericExposableProperty<UnityEngine.Object>> objectPropertiesByID;
        private Dictionary<int, GenericExposableProperty<object>> generalObjectPropertiesByID;
        
        private GraphBlackboard exposedBlackboard;
        private bool isInitialized;

        public void Initialize(GraphBlackboard exposedBlackboard = null)
        {
            this.exposedBlackboard = exposedBlackboard;

            #region Create Dictionaries for each property type
            if(floatProperties.Count > 0)
            {
                floatPropertiesByName = new Dictionary<int, GenericExposableProperty<float>>();
                floatPropertiesByID = new Dictionary<int, GenericExposableProperty<float>>();

                for(int i = 0; i < floatProperties.Count; i++)
                {
                    var property = floatProperties[i];
                    floatPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    floatPropertiesByID.Add(property.ID, property);
                }
            }
            if(advancedFloatProperties.Count > 0)
            {
                advancedFloatPropertiesByName = new Dictionary<int, GenericExposableProperty<TafraAdvancedFloat>>();
                advancedFloatPropertiesByID = new Dictionary<int, GenericExposableProperty<TafraAdvancedFloat>>();

                for(int i = 0; i < advancedFloatProperties.Count; i++)
                {
                    var property = advancedFloatProperties[i];
                    advancedFloatPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    advancedFloatPropertiesByID.Add(property.ID, property);
                }
            }
            if(intProperties.Count > 0)
            {
                intPropertiesByName = new Dictionary<int, GenericExposableProperty<int>>();
                intPropertiesByID = new Dictionary<int, GenericExposableProperty<int>>();

                for(int i = 0; i < intProperties.Count; i++)
                {
                    var property = intProperties[i];
                    intPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    intPropertiesByID.Add(property.ID, property);
                }
            }
            if(boolProperties.Count > 0)
            {
                boolPropertiesByName = new Dictionary<int, GenericExposableProperty<bool>>();
                boolPropertiesByID = new Dictionary<int, GenericExposableProperty<bool>>();

                for(int i = 0; i < boolProperties.Count; i++)
                {
                    var property = boolProperties[i];
                    boolPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    boolPropertiesByID.Add(property.ID, property);
                }
            }
            if(stringProperties.Count > 0)
            {
                stringPropertiesByName = new Dictionary<int, GenericExposableProperty<string>>();
                stringPropertiesByID = new Dictionary<int, GenericExposableProperty<string>>();

                for(int i = 0; i < stringProperties.Count; i++)
                {
                    var property = stringProperties[i];
                    stringPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    stringPropertiesByID.Add(property.ID, property);
                }
            }
            if(vector3Properties.Count > 0)
            {
                vector3PropertiesByName = new Dictionary<int, GenericExposableProperty<Vector3>>();
                vector3PropertiesByID = new Dictionary<int, GenericExposableProperty<Vector3>>();

                for(int i = 0; i < vector3Properties.Count; i++)
                {
                    var property = vector3Properties[i];
                    vector3PropertiesByName.Add(Animator.StringToHash(property.name), property);
                    vector3PropertiesByID.Add(property.ID, property);
                }
            }
            if(gameObjectProperties.Count > 0)
            {
                gameObjectPropertiesByName = new Dictionary<int, GenericExposableProperty<GameObject>>();
                gameObjectPropertiesByID = new Dictionary<int, GenericExposableProperty<GameObject>>();

                for(int i = 0; i < gameObjectProperties.Count; i++)
                {
                    var property = gameObjectProperties[i];
                    gameObjectPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    gameObjectPropertiesByID.Add(property.ID, property);
                }
            }
            if(scriptableObjectProperties.Count > 0)
            {
                scriptableObjectPropertiesByName = new Dictionary<int, GenericExposableProperty<ScriptableObject>>();
                scriptableObjectPropertiesByID = new Dictionary<int, GenericExposableProperty<ScriptableObject>>();

                for(int i = 0; i < scriptableObjectProperties.Count; i++)
                {
                    var property = scriptableObjectProperties[i];
                    scriptableObjectPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    scriptableObjectPropertiesByID.Add(property.ID, property);
                }
            }
            if(actorProperties.Count > 0)
            {
                actorPropertiesByName = new Dictionary<int, GenericExposableProperty<TafraActor>>();
                actorPropertiesByID = new Dictionary<int, GenericExposableProperty<TafraActor>>();

                for(int i = 0; i < actorProperties.Count; i++)
                {
                    var property = actorProperties[i];
                    actorPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    actorPropertiesByID.Add(property.ID, property);
                }
            }
            if(objectProperties.Count > 0)
            {
                objectPropertiesByName = new Dictionary<int, GenericExposableProperty<UnityEngine.Object>>();
                objectPropertiesByID = new Dictionary<int, GenericExposableProperty<UnityEngine.Object>>();

                for(int i = 0; i < objectProperties.Count; i++)
                {
                    var property = objectProperties[i];
                    objectPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    objectPropertiesByID.Add(property.ID, property);
                }
            }
            if(systemObjectProperties.Count > 0)
            {
                systemObjectPropertiesByName = new Dictionary<int, GenericExposableProperty<object>>();
                generalObjectPropertiesByID = new Dictionary<int, GenericExposableProperty<object>>();

                for(int i = 0; i < systemObjectProperties.Count; i++)
                {
                    var property = systemObjectProperties[i];
                    systemObjectPropertiesByName.Add(Animator.StringToHash(property.name), property);
                    generalObjectPropertiesByID.Add(property.ID, property);
                }
            }
            #endregion

            isInitialized = true;
        }

        /// <summary>
        /// Adds an item to the blackboard and returns the validated name.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemName"></param>
        /// <returns>The validated name, could have an extra "(X)" at the end if its a repated name.</returns>
        public ExposableProperty AddProperty(Type itemType, string itemName, out string validatedName)
        {
            if(Application.isPlaying)
            {
                TafraDebugger.Log("Blackboard", "Adding properties during runtime is not supported.", TafraDebugger.LogType.Error);
                validatedName = itemName;
                return null;
            }

            itemName = ValidateName(ValidateName(itemName));

            ExposableProperty exposableProperty = null;

            if(itemType == typeof(float))
            {
                GenericExposableProperty<float> property = new GenericExposableProperty<float>(itemName, 0, nextPropertyID);

                floatProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(TafraAdvancedFloat))
            {
                GenericExposableProperty<TafraAdvancedFloat> property = new GenericExposableProperty<TafraAdvancedFloat>(itemName, new TafraAdvancedFloat(0), nextPropertyID);
              
                advancedFloatProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(int))
            {
                GenericExposableProperty<int> property = new GenericExposableProperty<int>(itemName, 0, nextPropertyID);
              
                intProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(bool))
            {
                GenericExposableProperty<bool> property = new GenericExposableProperty<bool>(itemName, false, nextPropertyID);
              
                boolProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(string))
            {
                GenericExposableProperty<string> property = new GenericExposableProperty<string>(itemName, "", nextPropertyID);
          
                stringProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(Vector3))
            {
                GenericExposableProperty<Vector3> property = new GenericExposableProperty<Vector3>(itemName, Vector3.zero, nextPropertyID);
           
                vector3Properties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(GameObject))
            {
                GenericExposableProperty<GameObject> property = new GenericExposableProperty<GameObject>(itemName, null, nextPropertyID);
          
                gameObjectProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(ScriptableObject))
            {
                GenericExposableProperty<ScriptableObject> property = new GenericExposableProperty<ScriptableObject>(itemName, null, nextPropertyID);
         
                scriptableObjectProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(TafraActor))
            {
                GenericExposableProperty<TafraActor> property = new GenericExposableProperty<TafraActor>(itemName, null, nextPropertyID);
         
                actorProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(UnityEngine.Object))
            {
                GenericExposableProperty<UnityEngine.Object> property = new GenericExposableProperty<UnityEngine.Object>(itemName, null, nextPropertyID);
         
                objectProperties.Add(property);
                exposableProperty = property;
            }
            else if(itemType == typeof(object))
            {
                GenericExposableProperty<object> property = new GenericExposableProperty<object>(itemName, null, nextPropertyID);
                
                systemObjectProperties.Add(property);
                exposableProperty = property;
            }
            
            nextPropertyID++;

            validatedName = itemName;

            return exposableProperty;
        }
        public string RenameProperty(ExposableProperty item, string newName)
        {
            if(Application.isPlaying)
            {
                TafraDebugger.Log("Blackboard", "Renaming properties during runtime is not supported.", TafraDebugger.LogType.Error);
                return null;
            }

            if(item.name == newName)
                return newName;

            newName = ValidateName(newName, item.name);
            item.name = newName;

            return newName;
        }
        public List<GenericExposableProperty<T>> GetAllPropertiesOfGenericType<T>()
        {
            Type type = typeof(T);

            if(type == typeof(float))
                return floatProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(TafraAdvancedFloat))
                return advancedFloatProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(int))
                return intProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(bool))
                return boolProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(string))
                return stringProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(Vector3))
                return vector3Properties as List<GenericExposableProperty<T>>;
            else if(type == typeof(GameObject))
                return gameObjectProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(ScriptableObject))
                return scriptableObjectProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(TafraActor))
                return actorProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(UnityEngine.Object))
                return objectProperties as List<GenericExposableProperty<T>>;
            else if(type == typeof(object))
                return systemObjectProperties as List<GenericExposableProperty<T>>;
            else
                return null;
        }
        public object GetAllPropertiesOfType(Type type)
        {
            if(type == typeof(float))
                return floatProperties;
            else if(type == typeof(TafraAdvancedFloat))
                return advancedFloatProperties;
            else if(type == typeof(int))
                return intProperties;
            else if(type == typeof(bool))
                return boolProperties;
            else if(type == typeof(string))
                return stringProperties;
            else if(type == typeof(Vector3))
                return vector3Properties;
            else if(type == typeof(GameObject))
                return gameObjectProperties;
            else if(type == typeof(ScriptableObject))
                return scriptableObjectProperties;
            else if(type == typeof(TafraActor))
                return actorProperties;
            else if(type == typeof(UnityEngine.Object))
                return objectProperties;
            else if(type == typeof(object))
                return systemObjectProperties;
            else
                return null;
        }
        /// <summary>
        /// Wipes all properties of all types off the blackboard.
        /// </summary>
        public void RemoveAllProperties()
        {
            if(Application.isPlaying)
            {
                TafraDebugger.Log("Blackboard", "Removing properties during runtime is not supported.", TafraDebugger.LogType.Error);
                return;
            }

            floatProperties.Clear();
            advancedFloatProperties.Clear();
            intProperties.Clear();
            boolProperties.Clear();
            stringProperties.Clear();
            vector3Properties.Clear();
            gameObjectProperties.Clear();
            scriptableObjectProperties.Clear();
            actorProperties.Clear();
            objectProperties.Clear();
            systemObjectProperties.Clear();

            floatPropertiesByName = null;
            advancedFloatPropertiesByName = null;
            intPropertiesByName = null;
            boolPropertiesByName = null;
            stringPropertiesByName = null;
            vector3PropertiesByName = null;
            gameObjectPropertiesByName = null;
            scriptableObjectPropertiesByName = null;
            actorPropertiesByName = null;
            objectPropertiesByName = null;
            systemObjectPropertiesByName = null;

            floatPropertiesByID = null;
            advancedFloatPropertiesByID = null;
            intPropertiesByID = null;
            boolPropertiesByID = null;
            stringPropertiesByID = null;
            vector3PropertiesByID = null;
            gameObjectPropertiesByID = null;
            scriptableObjectPropertiesByID = null;
            actorPropertiesByID = null;
            objectPropertiesByID = null;
            generalObjectPropertiesByID = null;
        }
        public void RemoveProperty(Type itemType, string itemName)
        {
            if(Application.isPlaying)
            {
                TafraDebugger.Log("Blackboard", "Removing properties during runtime is not supported.", TafraDebugger.LogType.Error);
                return;
            }

            if(itemType == typeof(float))
                RemoveItemIfFound(floatProperties, itemName);
            else if(itemType == typeof(TafraAdvancedFloat))
                RemoveItemIfFound(advancedFloatProperties, itemName);
            else if(itemType == typeof(int))
                RemoveItemIfFound(intProperties, itemName);
            else if(itemType == typeof(string))
                RemoveItemIfFound(stringProperties, itemName);
            else if(itemType == typeof(bool))
                RemoveItemIfFound(boolProperties, itemName);
            else if(itemType == typeof(Vector3))
                RemoveItemIfFound(vector3Properties, itemName);
            else if(itemType == typeof(GameObject))
                RemoveItemIfFound(gameObjectProperties, itemName);
            else if(itemType == typeof(ScriptableObject))
                RemoveItemIfFound(scriptableObjectProperties, itemName);
            else if(itemType == typeof(TafraActor))
                RemoveItemIfFound(actorProperties, itemName);
            else if(itemType == typeof(UnityEngine.Object))
                RemoveItemIfFound(objectProperties, itemName);
            else if(itemType == typeof(object))
                RemoveItemIfFound(systemObjectProperties, itemName);
        }
        public int GetPropertiesCountOfType(Type type)
        {
            if(type == typeof(float))
                return floatProperties.Count;
            else if(type == typeof(TafraAdvancedFloat))
                return advancedFloatProperties.Count;
            else if(type == typeof(int))
                return intProperties.Count;
            else if(type == typeof(string))
                return stringProperties.Count;
            else if(type == typeof(bool))
                return boolProperties.Count;
            else if(type == typeof(Vector3))
                return vector3Properties.Count;
            else if(type == typeof(GameObject))
                return gameObjectProperties.Count;
            else if(type == typeof(ScriptableObject))
                return scriptableObjectProperties.Count;
            else if(type == typeof(TafraActor))
                return actorProperties.Count;
            else if(type == typeof(UnityEngine.Object))
                return objectProperties.Count;
            else if(type == typeof(object))
                return systemObjectProperties.Count;

            return 0;
        }
        public void GetAllProperties(List<ExposableProperty> listToFill)
        {
            listToFill.Clear();

            listToFill.AddRange(floatProperties);
            listToFill.AddRange(advancedFloatProperties);
            listToFill.AddRange(intProperties);
            listToFill.AddRange(boolProperties);
            listToFill.AddRange(stringProperties);
            listToFill.AddRange(vector3Properties);
            listToFill.AddRange(gameObjectProperties);
            listToFill.AddRange(scriptableObjectProperties);
            listToFill.AddRange(actorProperties);
            listToFill.AddRange(objectProperties);
            listToFill.AddRange(systemObjectProperties);
        }
        private string ValidateName(string propertyName, string originalName = null)
        {
            propertyName = propertyName.Trim();

            List<ExposableProperty> allExposableProperties = new List<ExposableProperty>();

            allExposableProperties.AddRange(floatProperties);
            allExposableProperties.AddRange(advancedFloatProperties);
            allExposableProperties.AddRange(intProperties);
            allExposableProperties.AddRange(boolProperties);
            allExposableProperties.AddRange(stringProperties);
            allExposableProperties.AddRange(vector3Properties);
            allExposableProperties.AddRange(gameObjectProperties);
            allExposableProperties.AddRange(scriptableObjectProperties);
            allExposableProperties.AddRange(actorProperties);
            allExposableProperties.AddRange(objectProperties);
            allExposableProperties.AddRange(systemObjectProperties);

            bool isUnique = false;
            int nameIteration = 0;
            while(!isUnique)
            {
                isUnique = true;

                for(int i = 0; i < allExposableProperties.Count; i++)
                {
                    string n = allExposableProperties[i].name;

                    if(propertyName == n)
                    {
                        if(nameIteration == 0)
                            propertyName += " (1)";
                        else
                        {
                            propertyName = propertyName.Remove(propertyName.LastIndexOf('('));
                            propertyName += $"({nameIteration + 1})";
                        }

                        //If this object's name went back to the original name, then no need to increase its iterations, just use the original name.
                        if(originalName != null && propertyName == originalName)
                        {
                            isUnique = true;
                            break;
                        }

                        nameIteration++;
                        isUnique = false;
                    }
                }
            }

            return propertyName;
        }
        private void RemoveItemIfFound<T>(List<GenericExposableProperty<T>> propertiesList, string itemName)
        {
            for(int i = 0; i < propertiesList.Count; i++)
            {
                if(propertiesList[i].name == itemName)
                {
                    propertiesList.RemoveAt(i);
                    return;
                }
            }
        }

        #region Property Getter Functions
        public GenericExposableProperty<float> TryGetFloatProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(floatPropertiesByName == null)
                return null;

            if((propretyID > -1 && floatPropertiesByID.TryGetValue(propretyID, out var prop)) || floatPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetFloatProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<TafraAdvancedFloat> TryGetAdvancedFloatProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(advancedFloatPropertiesByName == null)
                return null;

            if((propretyID > -1 && advancedFloatPropertiesByID.TryGetValue(propretyID, out var prop)) || advancedFloatPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetAdvancedFloatProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<int> TryGetIntProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(intPropertiesByName == null)
                return null;

            if((propretyID > -1 && intPropertiesByID.TryGetValue(propretyID, out var prop)) || intPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetIntProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<bool> TryGetBoolProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(boolPropertiesByName == null)
                return null;

            if((propretyID > -1 && boolPropertiesByID.TryGetValue(propretyID, out var prop)) || boolPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetBoolProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<string> TryGetStringProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(stringPropertiesByName == null)
                return null;

            if((propretyID > -1 && stringPropertiesByID.TryGetValue(propretyID, out var prop)) || stringPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetStringProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<Vector3> TryGetVector3Property(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(vector3PropertiesByName == null)
                return null;

            if((propretyID > -1 && vector3PropertiesByID.TryGetValue(propretyID, out var prop)) || vector3PropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetVector3Property(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<GameObject> TryGetGameObjectProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(gameObjectPropertiesByName == null)
                return null;

            if((propretyID > -1 && gameObjectPropertiesByID.TryGetValue(propretyID, out var prop)) || gameObjectPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetGameObjectProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<ScriptableObject> TryGetScriptableObjectProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(scriptableObjectPropertiesByName == null)
                return null;

            if((propretyID > -1 && scriptableObjectPropertiesByID.TryGetValue(propretyID, out var prop)) || scriptableObjectPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetScriptableObjectProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<TafraActor> TryGetActorProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(actorPropertiesByName == null)
                return null;

            if((propretyID > -1 && actorPropertiesByID.TryGetValue(propretyID, out var prop)) || actorPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetActorProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<UnityEngine.Object> TryGetObjectProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(objectPropertiesByName == null)
                return null;

            if((propretyID > -1 && objectPropertiesByID.TryGetValue(propretyID, out var prop)) || objectPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetObjectProperty(propertyName, propretyID);
            }

            return null;
        }
        public GenericExposableProperty<object> TryGetSystemObjectProperty(int propertyName, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Blackboard", "Blackboard isn't initialized, can't get properties before initilization.", TafraDebugger.LogType.Error);
                return null;
            }

            if(systemObjectPropertiesByName == null)
                return null;

            if((propretyID > -1 && generalObjectPropertiesByID.TryGetValue(propretyID, out var prop)) || systemObjectPropertiesByName.TryGetValue(propertyName, out prop))
            {
                if(!prop.expose)
                    return prop;
                else
                    return exposedBlackboard.TryGetSystemObjectProperty(propertyName, propretyID);
            }

            return null;
        }
        #endregion
    }
}

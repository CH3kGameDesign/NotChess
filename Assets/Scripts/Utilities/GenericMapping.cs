//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             GenericMapping
//             Author: Mitchell Croft
//             Date Created: 11th May, 2021
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//      Allow for generic associating of a key with a value that can be assigned
//      in inspector
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlaySide.Objects
{
    /// <summary> Allow for generic associating of a key with a value that can be assigned in inspector </summary>
    /// <remarks>
    /// To display these in inspector properly, the inheriting child type and the TKey/TValue types need
    /// to be marked as System.Serializable
    /// </remarks>
    public class GenericMapping<TKey, TValue> : GenericMappingBase
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // *	Storage
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [Tooltip("The key value that will be used to link to the value of the mapping")]
        public TKey key;

        [Tooltip("The value that will be accessed via the defined key value")]
        public TValue value;

        /// <summary> Convert this mapping object to a simple representation of the data stored within </summary>
        /// <returns> Returns a display text representation of the current mapping object </returns>
        public override string ToString() { return string.Format("{0} -> {1}", key, value); }
    }

    /// <summary> Provide for a collection object description that can be used to toggle collections of values based on an active key </summary>
    /// <typeparam name="TKey"> The type of value that will be used for the key </typeparam>
    public class ToggleMapping<TKey> : GenericMapping<TKey, GameObject[]> {}

    /// <summary> Provide for a collection of toggle-able objects that can be modified easily </summary>
    /// <typeparam name="TKey"> The type of value that will be used for the key </typeparam>
    public class ComplexToggleMapping<TKey> : GenericMapping<TKey, ToggleTarget[]> {}

    /// <summary> Pre-defined object container for storing collections of objects that will be toggled based on a single bool flag </summary>
    [System.Serializable] public sealed class BoolStateToggleMapping : ToggleMapping<bool> {}
    [System.Serializable] public sealed class BoolStateComplexToggleMapping : ComplexToggleMapping<bool> {}

    /// <summary> Simple container for a referencable object that can have it's enabled state modified </summary>
    [System.Serializable]
    public struct ToggleTarget
    {     
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // *	Data
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary> A reference to the object that will be toggled </summary>
        [SerializeField] private UnityEngine.Object target;

        /// <summary> The method that will be used to toggle the specified target </summary>
        [SerializeField] private ToggleMethod toggleType;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // *	Operation
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary> Get and set the target of this toggle target operation </summary>
        public UnityEngine.Object Target
        {
            get { return target; }
            set { target = (IsTypeUsable(value) ? value : null); }
        }

        /// <summary> Get and set the toggle type that will be used for the target </summary>
        public ToggleMethod ToggleType
        {
            get => toggleType;
            set => toggleType = value;
        }

        /// <summary> Flags if this object is valid for modification </summary>
        public bool Valid
        {
            get
            {
                return target;
            }
        }

        /// <summary> Get and set the enabled state of the contained target object </summary>
        public bool Enabled
        {
            get
            {
                // If there is no target, not enabled
                if (!target)
                {
                    return false;
                }

                // Get the flag to be checked
                switch (target)
                {
                    case GameObject _go:                return _go.activeSelf;
                    case UnityEngine.UI.Selectable _sl: return (toggleType == ToggleMethod.Interactable ? _sl.interactable : _sl.enabled);
                    case MonoBehaviour _mb:             return _mb.enabled;
                    case Component _:
                        PropertyInfo prop = GetToggleProperty(target.GetType());
                        return (prop != null && prop.CanRead ?
                            (bool)prop.GetValue(target) :
                            false
                        );
                    default:                            return false;
                }
            }
            set
            {
                // If no target, nothing to change
                if (target)
                {
                    switch (target)
                    {
                        case GameObject _go:
                            _go.SetActive(value);
                            break;
                        case UnityEngine.UI.Selectable _sl:
                            if (toggleType == ToggleMethod.Interactable)
                            {
                                _sl.interactable = value;
                            }
                            else
                            {
                                _sl.enabled = value;
                            }
                            break;
                        case MonoBehaviour _mb:
                            _mb.enabled = value;
                            break;
                        case Component _:
                            PropertyInfo prop = GetToggleProperty(target.GetType());
                            if (prop != null && prop.CanWrite)
                            {
                                prop.SetValue(target, value);
                            }
                            break;
                    }
                }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // *	Type
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary> Defines the different targets that can be modified by this target operation </summary>
        public enum ToggleMethod
        {
            ObjectActive,
            ComponentEnabled,
            Interactable,
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // *	Utility
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary> Relay the hash code of the target object </summary>
        /// <returns> Returns the hashcode for the target object or 0 </returns>
        public override int GetHashCode() { return (target ? target.GetHashCode() : 0); }

        /// <summary> Relay the string of the target object </summary>
        /// <returns> Returns the string of the target object or a placeholder </returns>
        public override string ToString() { return (target ? target.ToString() : "[Null]"); }

        /// <summary> Check to see if the specified type can be used for referencing </summary>
        /// <param name="_obj"> The object that is to be tested </param>
        /// <returns> Returns true if the object can be referenced and toggled successfully </returns>
        public static bool IsTypeUsable(UnityEngine.Object _obj)
        {
            // Needs to have a type
            if (!_obj)
            {
                return false;
            }
            
            // Determine what it is
            switch (_obj)
            {
                case GameObject _:                  
                case UnityEngine.UI.Selectable _:   
                case MonoBehaviour _:               return true;
                case Component _:                   return GetToggleProperty(_obj.GetType()) != null;
                default:                            return false;
            }
        }

        /// <summary> Look for the property that is to be used for toggling </summary>
        /// <param name="_type"> The type of the object that is to be searched </param>
        /// <param name="_method"> The method that will be used to toggle the target, defining the property to retrieve </param>
        /// <returns> Returns the Property Info object for the corresponding property </returns>
        private static PropertyInfo GetToggleProperty(System.Type _type)
        {
            return _type.GetProperty
            (
                "enabled",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
                null,
                typeof(bool),
                System.Type.EmptyTypes,
                null
            );
        }

#if UNITY_EDITOR
        /// <summary> Display a basic object field for the toggle target within the inspector </summary>
        [CustomPropertyDrawer(typeof(ToggleTarget))]
        private sealed class ToggleTargetDrawer : PropertyDrawer
        {
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // *	Data
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            /// <summary> Cache display options that will be shown for different types </summary>
            private struct OptionCache
            {
                public GUIContent[] popupLabels;
                public int[] values;
                public Dictionary<int, int> indexLookup;
                public OptionCache(params ToggleMethod[] _methods)
                {
                    values = new int[_methods.Length];
                    popupLabels = new GUIContent[values.Length];
                    indexLookup = new Dictionary<int, int>(values.Length);
                    for (int i = 0; i < values.Length; ++i)
                    {
                        values[i] = (int)_methods[i];
                        popupLabels[i] = new GUIContent(ObjectNames.NicifyVariableName(_methods[i].ToString()));
                        indexLookup[values[i]] = i;
                    }
                }
            }

            /// <summary> The collection of toggle options that will be selected from for display </summary>
            private readonly Dictionary<System.Type, OptionCache> TOGGLE_METHODS = new Dictionary<System.Type, OptionCache>
            {
                // Default handler for unknown types
                { typeof(object), new OptionCache(new ToggleMethod[0]) },

                { typeof(GameObject),                   new OptionCache(ToggleMethod.ObjectActive)                                  },
                { typeof(Component),                    new OptionCache(ToggleMethod.ComponentEnabled)                              },
                { typeof(UnityEngine.UI.Selectable),    new OptionCache(ToggleMethod.Interactable, ToggleMethod.ComponentEnabled)   }
            };

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // *	Operation
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            /// <summary> Display the target element within the UI </summary>
            /// <param name="_position"> The position within the inspector to show the values </param>
            /// <param name="_property"> The property containing the information that is to be set </param>
            /// <param name="_label"> The label that has been assigned to the property to show </param>
            public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
            {
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(_position, _label, _property);

                // Get the properties that are to be displayed
                SerializedProperty objProp = _property.FindPropertyRelative(nameof(ToggleTarget.target));
                SerializedProperty typeProp = _property.FindPropertyRelative(nameof(ToggleTarget.toggleType));

                // Get the areas to show the elements
                Rect objFieldRect = new Rect(_position.x, _position.y, _position.width * .8f, _position.height);
                Rect tglPopupRect = new Rect(_position.x + _position.width * .8f, _position.y, _position.width * .2f, _position.height);

                // Remove the indentation
                int prevIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // Track the cache of display options that are in use
                OptionCache? activeCache = null;

                // Offer a basic object field to handle the assignment
                UnityEngine.Object newObject = EditorGUI.ObjectField(objFieldRect, _label, objProp.objectReferenceValue, typeof(UnityEngine.Object), true);
                if (newObject != objProp.objectReferenceValue)
                {
                    // Make sure that the object type is usable
                    if (newObject && !IsTypeUsable(newObject))
                    {
                        newObject = null;
                    }

                    // Stash the object reference
                    objProp.objectReferenceValue = newObject;

                    // Setup the default toggle type
                    if (!newObject)
                    {
                        typeProp.intValue = -1;
                    }
                    else
                    {
                        activeCache = GetOptionsForType(newObject.GetType());
                        typeProp.intValue = (activeCache.HasValue ?
                            (activeCache.Value.values.Length > 0 ? activeCache.Value.values[0] : -1) :
                            -1
                        );
                    }
                }

                // Check that there is an object to process
                if (newObject)
                {
                    // If there is no active cache data, get it
                    if (!activeCache.HasValue)
                    {
                        activeCache = GetOptionsForType(newObject.GetType());
                    }

                    // If there is no active cache something went wrong
                    if (!activeCache.HasValue)
                    {
                        EditorGUI.LabelField(tglPopupRect, "No Cache Data");
                    }
                    else
                    {
                        int prevInd = activeCache.Value.indexLookup.TryGetValue(typeProp.intValue, out int _index) ? _index : -1;
                        int curInd = EditorGUI.Popup(tglPopupRect, prevInd, activeCache.Value.popupLabels);
                        if (curInd != prevInd)
                        {
                            typeProp.intValue = (curInd >= 0 && curInd < activeCache.Value.values.Length ?
                                activeCache.Value.values[curInd] :
                                -1
                            );
                        }
                    }
                }

                // Reset the indentation
                EditorGUI.indentLevel = prevIndent;
                EditorGUI.EndProperty();
            }

            /// <summary> Traverse up the type hierarchy to find the options to use for this type </summary>
            /// <param name="_type"> The type that is to have the options found for </param>
            /// <returns> Returns the cache options to use for the type or null if unable to find </returns>
            private OptionCache? GetOptionsForType(System.Type _type)
            {
                do
                {
                    if (TOGGLE_METHODS.TryGetValue(_type, out var _cache))
                    {
                        return _cache;
                    }
                    _type = _type.BaseType;
                } while (_type != null);
                return null;
            }
        }
#endif
    }

    /// <summary> Provide additional functionality for collections of mapping objects </summary>
    public static class GenericMappingUtility
    {
        /// <summary> Retrieve the value from the mapping with the supplied key </summary>
        /// <typeparam name="TKey"> The key that will be associated with the value </typeparam>
        /// <typeparam name="TValue"> The value that will be associated with the key </typeparam>
        /// <param name="_mappings"> The collection of mapping objects that are to be searched </param>
        /// <param name="_key"> The key of the value that is to be retrieved </param>
        /// <param name="_default"> The default value to be returned if unable to find </param>
        /// <returns> Returns the stored value or _default if unable to find </returns>
        public static TValue Get<TKey, TValue>(this IList<GenericMapping<TKey, TValue>> _mappings, TKey _key, TValue _default = default)
        {
            if (_mappings == null)
            {
                return _default;
            }
            for (int i = 0; i < _mappings.Count; ++i)
            {
                if (object.Equals(_key, _mappings[i].key))
                {
                    return _mappings[i].value;
                }
            }
            return _default;
        }

        /// <summary> Retrieve the first value from the mapping that matches the predicate </summary>
        /// <typeparam name="TKey"> The key that will be associated with the value </typeparam>
        /// <typeparam name="TValue"> The value that will be associated with the key </typeparam>
        /// <param name="_mappings"> The collection of mapping objects that are to be searched </param>
        /// <param name="_predicate"> Callback for determining the value to be returned </param>
        /// <param name="_default"> The default value to be returned if unable to find </param>
        /// <returns> Returns the stored value or _default if unable to find </returns>
        public static TValue Get<TKey, TValue>(this IList<GenericMapping<TKey, TValue>> _mappings, System.Func<GenericMapping<TKey, TValue>, bool> _predicate, TValue _default = default)
        {
            if (_mappings == null)
            {
                return _default;
            }
            for (int i = 0; i < _mappings.Count; ++i)
            {
                if (_predicate(_mappings[i]))
                {
                    return _mappings[i].value;
                }
            }
            return _default;
        }

        /// <summary> Convert an list collection of mapping objects into a dictionary lookup object </summary>
        /// <typeparam name="TKey"> The key that will be associated with the value </typeparam>
        /// <typeparam name="TValue"> The value that will be associated with the key </typeparam>
        /// <param name="_mappings"> The collection that is to be stored in a dictionary </param>
        /// <returns> Returns a dictionary with the collection values keyed in </returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IList<GenericMapping<TKey, TValue>> _mappings)
        {
            Dictionary<TKey, TValue> lookup = new Dictionary<TKey, TValue>(_mappings != null ? _mappings.Count : 0);
            if (_mappings != null)
            {
                for (int i = 0; i < _mappings.Count; ++i)
                {
                    if (object.Equals(_mappings[i].key, null))
                    {
                        Debug.LogWarningFormat("Invalid key value at index {0} in generic mapping. Can't have null key value in dictionary", i);
                        continue;
                    }
                    lookup[_mappings[i].key] = _mappings[i].value;
                }
            }
            return lookup;
        }

        /// <summary> Convert an enumerable collection of mapping objects into a dictionary lookup object </summary>
        /// <typeparam name="TKey"> The key that will be associated with the value </typeparam>
        /// <typeparam name="TValue"> The value that will be associated with the key </typeparam>
        /// <param name="_mappings"> The collection that is to be stored in a dictionary </param>
        /// <returns> Returns a dictionary with the collection values keyed in </returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<GenericMapping<TKey, TValue>> _mappings)
        {
            Dictionary<TKey, TValue> lookup = new Dictionary<TKey, TValue>();
            if (_mappings != null)
            {
                foreach (GenericMapping<TKey, TValue> pair in _mappings)
                {
                    if (object.Equals(pair.key, null))
                    {
                        Debug.LogWarningFormat("Invalid key value in generic mapping. Can't have null key value in dictionary");
                        continue;
                    }
                    lookup[pair.key] = pair.value;
                }
            }
            return lookup;
        }

        /// <summary> Enable the collection of objects that are stored under the specified key value while disabling the rest </summary>
        /// <typeparam name="TKey"> The type of value that is used for the key </typeparam>
        /// <param name="_mappings"> The collection of mapping objects that are to be processed </param>
        /// <param name="_toActivate"> The key value that is to be activated </param>
        /// <param name="_persistStates"> Flags if objects that are in multiple groups that would be disabled and then re-enabled should be skipped </param>
        /// <returns> Returns true if an activate-able collection was found and toggled </returns>
        public static bool SetActiveToggle<TKey>(this IList<ToggleMapping<TKey>> _mappings, TKey _toActivate, bool _persistStates = false)
        {
            int toActivate = -1;
            if (_mappings != null)
            {
                // Need to identify active ones before we can disable anything
                if (_persistStates)
                {
                    // Find the set of objects that need to be activated
                    HashSet<GameObject> activated = new HashSet<GameObject>();
                    for (int i = 0; i < _mappings.Count; ++i)
                    {
                        if (object.Equals(_toActivate, _mappings[i].key))
                        {
                            toActivate = i;
                            if (_mappings[i].value != null)
                            {
                                activated.UnionWith(_mappings[i].value);
                            }
                            break;
                        }
                    }
                    
                    // Disable the other objects
                    for (int i = 0; i < _mappings.Count; ++i)
                    {
                        if (i != toActivate)
                        {
                            if (_mappings[i].value != null)
                            {
                                foreach (GameObject obj in _mappings[i].value)
                                {
                                    if (obj && obj.activeSelf && !activated.Contains(obj))
                                    {
                                        obj.SetActive(false);
                                    }
                                }
                            }
                        }
                    }
                }

                // Figure it out as we go
                else
                {
                    // Deactivate all of the objects
                    for (int i = 0; i < _mappings.Count; ++i)
                    {
                        if (_mappings[i] == null)
                        {
                            continue;
                        }
                        else if (object.Equals(_toActivate, _mappings[i].key))
                        {
                            toActivate = i;
                        }
                        else
                        {
                            if (_mappings[i].value != null)
                            {
                                foreach (GameObject obj in _mappings[i].value)
                                {
                                    if (obj && obj.activeSelf)
                                    {
                                        obj.SetActive(false);
                                    }
                                }
                            }
                        }
                    }                    
                }
            }

            // Activate the set of objects
            if (toActivate != -1)
            {
                if (_mappings[toActivate].value != null)
                {
                    foreach (GameObject obj in _mappings[toActivate].value)
                    {
                        if (obj && !obj.activeSelf)
                        {
                            obj.SetActive(true);
                        }
                    }
                }
            }
            return (toActivate != -1);
        }

        /// <summary> Enable the collection of objects that are stored under the specified key value while disabling the rest </summary>
        /// <typeparam name="TKey"> The type of value that is used for the key </typeparam>
        /// <param name="_mappings"> The collection of mapping objects that are to be processed </param>
        /// <param name="_toActivate"> The key value that is to be activated </param>
        /// <param name="_persistStates"> Flags if objects that are in multiple groups that would be disabled and then re-enabled should be skipped </param>
        /// <returns> Returns true if an activate-able collection was found and toggled </returns>
        public static bool SetActiveToggle<TKey>(this IList<ComplexToggleMapping<TKey>> _mappings, TKey _toActivate, bool _persistStates = false)
        {
            int toActivate = -1;
            if (_mappings != null)
            {
                // Need to identify active ones before we can disable anything
                if (_persistStates)
                {
                    // Find the set of objects that need to be activated
                    HashSet<UnityEngine.Object> activated = new HashSet<UnityEngine.Object>();
                    for (int i = 0; i < _mappings.Count; ++i)
                    {
                        if (object.Equals(_toActivate, _mappings[i].key))
                        {
                            toActivate = i;
                            if (_mappings[i].value != null)
                            {
                                activated.UnionWith(_mappings[i].value.Where(x => x.Valid).Select(x => x.Target));
                            }
                            break;
                        }
                    }

                    // Disable the other objects
                    for (int i = 0; i < _mappings.Count; ++i)
                    {
                        if (i != toActivate)
                        {
                            if (_mappings[i].value != null)
                            {
                                for (int ii = 0; ii < _mappings[i].value.Length; ++ii)
                                {
                                    _mappings[i].value[ii].Enabled = false;
                                }
                            }
                        }
                    }
                }

                // Figure it out as we go
                else
                {
                    // Deactivate all of the objects
                    for (int i = 0; i < _mappings.Count; ++i)
                    {
                        if (_mappings[i] == null)
                        {
                            continue;
                        }
                        else if (object.Equals(_toActivate, _mappings[i].key))
                        {
                            toActivate = i;
                        }
                        else
                        {
                            if (_mappings[i].value != null)
                            {
                                for (int ii = 0; ii < _mappings[i].value.Length; ++ii)
                                {
                                    _mappings[i].value[ii].Enabled = false;
                                }
                            }
                        }
                    }
                }
            }

            // Activate the set of objects
            if (toActivate != -1)
            {
                if (_mappings[toActivate].value != null)
                {
                    for (int i = 0; i < _mappings[toActivate].value.Length; ++i)
                    {
                        _mappings[toActivate].value[i].Enabled = true;
                    }
                }
            }
            return (toActivate != -1);
        }

        /// <summary> Raise a callback event for the value with the specified key </summary>
        /// <typeparam name="TKey"> The key type for the pairing mapping object </typeparam>
        /// <typeparam name="TValue"> The value type for the pairing mapping object </typeparam>
        /// <param name="_mappings"> The collection of mapping objects that are to be processed </param>
        /// <param name="_key"> The key item that is to be found for raising the callback </param>
        /// <param name="_callback"> The callback action that is to be raised </param>
        /// <param name="_nullCheck"> [Optional] Flags if the value should be checked for a null value, and only raise the callback if not null </param>
        /// <returns> Returns true if the item was found and the callback raised </returns>
        public static bool ForValue<TKey, TValue>(this IList<GenericMapping<TKey, TValue>> _mappings, TKey _key, System.Action<TValue> _callback, bool _nullCheck = true)
        {
            if (_mappings == null)
            {
                return false;
            }
            for (int i = 0; i < _mappings.Count; ++i)
            {
                if (object.Equals(_key, _mappings[i].key))
                {
                    if (!_nullCheck || !object.Equals(null, _mappings[i].value))
                    {
                        _callback?.Invoke(_mappings[i].value);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }

    /// <summary> Base point for mapping objects to inherit from to be displayed in Inspector properly </summary>
    public abstract class GenericMappingBase
    {
#if UNITY_EDITOR
        /// <summary> A simple property drawer for displaying a Generic Mapping object without any displayed labels </summary>
        [CustomPropertyDrawer(typeof(GenericMappingBase), true)]
        private sealed class GenericMappingDrawer : PropertyDrawer
        {
            /*----------Variables----------*/
            //CONST

            /// <summary> Store basic representations of the main property elements for displaying within the inspector </summary>
            private static readonly GUIContent KEY_LABEL, VAL_LABEL;

            /*----------Functions----------*/
            //STATIC

            /// <summary> Initialise the readonly content elements for display </summary>
            static GenericMappingDrawer()
            {
                KEY_LABEL = new GUIContent("Key");
                VAL_LABEL = new GUIContent("Value");
            }

            //PUBLIC

            /// <summary> Determine the height that is needed to display the the mapping object values </summary>
            /// <param name="_property"> The property that is being displayed </param>
            /// <param name="_label"> The label that has been assigned to the property </param>
            /// <returns> Returns the height to be used for displaying the property </returns>
            public sealed override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
            {
                // Retrieve the properties that are to be displayed
                SerializedProperty keyProp = _property.FindPropertyRelative("key");
                SerializedProperty valProp = _property.FindPropertyRelative("value");

                // Get the height of the properties to determine how they should be drawn
                float keyHeight = (keyProp != null ? EditorGUI.GetPropertyHeight(keyProp, true) : EditorGUIUtility.singleLineHeight),
                      valHeight = (valProp != null ? EditorGUI.GetPropertyHeight(valProp, true) : EditorGUIUtility.singleLineHeight);

                // If both of these are the same height, display them next to each other. Otherwise, draw the 'key' on top
                return (Mathf.Approximately(keyHeight, valHeight) ? keyHeight : keyHeight + valHeight);
            }

            /// <summary> Display the elements of the property within the designated area on the inspector area </summary>
            /// <param name="_position"> The position within the inspector that the property should be drawn to </param>
            /// <param name="_property"> The property that is to be displayed within the inspector </param>
            /// <param name="_label"> The label that has been assigned to the property </param>
            public sealed override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
            {
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(_position, _label, _property);

                // Retrieve the properties that are to be displayed
                SerializedProperty keyProp = _property.FindPropertyRelative("key");
                SerializedProperty valProp = _property.FindPropertyRelative("value");

                // Store the values that will be used to display the properties
                Rect keyArea, valArea; GUIContent keyContent = GUIContent.none, valContent = GUIContent.none;

                // If the height of the key is the same as the height assigned for position, they can display on the same line
                float keyHeight = (keyProp != null ? EditorGUI.GetPropertyHeight(keyProp, true) : EditorGUIUtility.singleLineHeight);
                float valueHeight = (valProp != null ? EditorGUI.GetPropertyHeight(valProp, true) : EditorGUIUtility.singleLineHeight);
                if (Mathf.Approximately(keyHeight, valueHeight))
                {
                    // Use half of the specified area each
                    keyArea = new Rect(_position.x, _position.y, _position.width * .5f, _position.height);
                    valArea = new Rect(_position.x + _position.width * .5f, _position.y, _position.width * .5f, _position.height);
                }

                // Otherwise, there needs to be some ordering 
                else
                {
                    // Use proper labels for the individual elements
                    keyContent = KEY_LABEL; valContent = VAL_LABEL;

                    // Stack the positions on over the other
                    keyArea = EditorGUI.IndentedRect(new Rect(_position.x, _position.y, _position.width, keyHeight));
                    valArea = EditorGUI.IndentedRect(new Rect(_position.x, _position.y + keyHeight, _position.width, _position.height - keyHeight));
                }

                // Display the properties options
                if (keyProp != null)
                {
                    EditorGUI.PropertyField(keyArea, keyProp, keyContent, true);
                }
                else
                {
                    EditorGUI.LabelField(keyArea, "'key' Property Not Found");
                }
                if (valProp != null)
                {
                    EditorGUI.PropertyField(valArea, valProp, valContent, true);
                }
                else
                {
                    EditorGUI.LabelField(valArea, "'value' Property Not Found");
                }

                // End the prefab-able section
                EditorGUI.EndProperty();
            }
        }
#endif
    }
}
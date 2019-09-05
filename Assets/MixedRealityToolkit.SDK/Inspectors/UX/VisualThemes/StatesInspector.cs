﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(States))]
    public class StatesInspector : UnityEditor.Editor
    {
        protected States instance;
        protected SerializedProperty stateList;

        protected virtual void OnEnable()
        {
            instance = (States)target;

            stateList = serializedObject.FindProperty("stateList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorUIUtility.DrawTitle("States");
            InspectorUIUtility.DrawNotice("Manage state configurations to drive Interactables or Transitions");

            SerializedProperty stateModelClassName = serializedObject.FindProperty("StateModelClassName");
            SerializedProperty assemblyQualifiedName  = serializedObject.FindProperty("AssemblyQualifiedName");

            var stateModelTypes = typeof(InteractableStateModel).GetAllSubClassesOf();
            var stateModelClassNames = stateModelTypes.Select(t => t.Name).ToArray();
            int id = Array.IndexOf(stateModelClassNames, stateModelClassName.stringValue);
            int newId = EditorGUILayout.Popup("State Model", id, stateModelClassNames);
            if (id != newId)
            {
                Type newType = stateModelTypes[newId];
                stateModelClassName.stringValue = newType.Name;
                assemblyQualifiedName.stringValue = newType.AssemblyQualifiedName;
            }

            int bitCount = 0;
            for (int i = 0; i < stateList.arraySize; i++)
            {
                if (i == 0)
                {
                    bitCount += 1;
                }
                else
                {
                    bitCount += bitCount;
                }

                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    SerializedProperty stateItem = stateList.GetArrayElementAtIndex(i);

                    SerializedProperty name = stateItem.FindPropertyRelative("Name");
                    SerializedProperty activeIndex = stateItem.FindPropertyRelative("ActiveIndex");
                    SerializedProperty bit = stateItem.FindPropertyRelative("Bit");
                    SerializedProperty index = stateItem.FindPropertyRelative("Index");

                    activeIndex.intValue = i;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        string[] stateEnums = GetStateOptions();
                        int enumIndex = Array.IndexOf(stateEnums, name.stringValue);

                        int newEnumIndex = EditorGUILayout.Popup(name.stringValue + " (" + bitCount + ")", enumIndex, stateEnums);

                        name.stringValue = stateEnums[newEnumIndex];
                        index.intValue = newEnumIndex;

                        InspectorUIUtility.SmallButton(new GUIContent(InspectorUIUtility.Minus, "Remove State"), i, RemoveState);
                    }

                    // assign the bitcount based on location in the list
                    bit.intValue = bitCount;
                }
            }

            InspectorUIUtility.FlexButton(new GUIContent("+", "Add Theme Property"), 0, AddState);

            serializedObject.ApplyModifiedProperties();
        }

        protected void AddState(int index, SerializedProperty prop = null)
        {
            stateList.InsertArrayElementAtIndex(stateList.arraySize);
        }

        protected void RemoveState(int index, SerializedProperty prop = null)
        {
            stateList.DeleteArrayElementAtIndex(index);
        }

        /// <summary>
        /// Get a list of state names
        /// </summary>
        /// <returns></returns>
        protected string[] GetStateOptions()
        {
            return Enum.GetNames(typeof(InteractableStates.InteractableStateEnum));
        }
    }
#endif
}

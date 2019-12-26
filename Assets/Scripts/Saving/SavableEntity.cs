using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using RPG.Core;
using System;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SavableEntity : MonoBehaviour
    {
        [SerializeField] string m_UUID = "";
        static Dictionary<string, SavableEntity> GlobalIDLookup = new Dictionary<string, SavableEntity>();

        public string GetUniqueID()
        {
            return m_UUID;
        }
        
        //Save state if object on save
        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()]= saveable.CaptureState();
            }
            return state;
        }

        //Restore the state of the object on load
        public void RestoreState(object state)
        {
            Dictionary<string, object> statedict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typestring = saveable.GetType().ToString();
                if (statedict.ContainsKey(typestring))
                {
                    saveable.RestoreState(statedict[typestring]);
                }
            }
        }

//Only execute in editor.  Don't do in a built game.
#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("m_UUID");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }
            GlobalIDLookup[property.stringValue] = this;
        }
#endif
        private bool IsUnique(string canidateID)
        {
            //Is ID Unique
            if (!GlobalIDLookup.ContainsKey(canidateID)) return true;

            //Is non-unique ID mine
            if (GlobalIDLookup[canidateID] == this) return true;

            //Check if it has been deleted
            if (GlobalIDLookup[canidateID]== null)
            {
                //Remove
                GlobalIDLookup.Remove(canidateID);
                return true;
            }

            return false;
        }
    }
}

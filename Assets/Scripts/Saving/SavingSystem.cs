using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    //Core saving functionality
    public class SavingSystem : MonoBehaviour
    {
        [SerializeField] string m_DefaultSaveFileFolder = "SaveGames";

        public IEnumerator LoadLastScene(string saveFileName)
        {
            Dictionary<string, object> state = LoadFile(saveFileName);
            //get default index for scence
            int index = SceneManager.GetActiveScene().buildIndex;

            //Check to see if we have a last scence saved in the save file
            if (state.ContainsKey("LastSceneBuildIndex"))
            {
                //Set index to last scene
                index = (int)state["LastSceneBuildIndex"];
            }

            yield return SceneManager.LoadSceneAsync(index);
            RestoreState(state);
        }


        //Save the game to a savefile
        public void Save(string saveFileName)
        {
            Dictionary<string, object>  state = LoadFile(saveFileName);
            CaptureState(state);
            saveFile(saveFileName, state);
        }

        //Load a game from the savefile
        public void Load(string saveFileName)
        {
            RestoreState(LoadFile(saveFileName));
        }
        
        public void Delete(string saveFileName)
        {
            File.Delete(GetPathForSaveFile(saveFileName));
        }

        private Dictionary<string, object> LoadFile(string saveFileName)
        {
            string path = GetPathForSaveFile(saveFileName);

            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            {//Protect against leaving function prior to closing the file
                using (FileStream filestream = File.Open(path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (Dictionary<string, object>)formatter.Deserialize(filestream);
                }
            }
        }

        private void saveFile(string saveFileName, object state)
        {
            string path = GetPathForSaveFile(saveFileName);

            //Protect against leaving function prior to closing the file
            using (FileStream filestream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                //Use Unity serialize system
                formatter.Serialize(filestream, state);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach(SavableEntity savable in FindObjectsOfType<SavableEntity>())
            {
                state[savable.GetUniqueID()] = savable.CaptureState();
            }

            state["LastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
            {
                string id = savable.GetUniqueID();
                if (state.ContainsKey(id))
                {
                    savable.RestoreState(state[id]);
                } 
            }
        }

        //Get the full save path for the save file.
        private string GetPathForSaveFile(string savefile)
        {
            return Path.Combine(Application.persistentDataPath, savefile+".sav");
        }
    }
}

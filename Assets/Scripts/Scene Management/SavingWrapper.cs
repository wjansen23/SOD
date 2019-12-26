using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    //Game specific saving functionality
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float m_FadeInTime = .25f;
        const string DEFAULT_SAVE_FILENAME = "defaultSave";

        private void Awake()
        {
            StartCoroutine(LoadLastScence());
        }
        public IEnumerator LoadLastScence()
        {
            //Makes sure all awakes are completed
            yield return GetComponent<SavingSystem>().LoadLastScene(DEFAULT_SAVE_FILENAME);

            //find Fader
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(m_FadeInTime);
        }

        private void Update()
        {
            //Is user trying to save to default save
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            //Is using trying load default save
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            //Is using trying load default save
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteSave();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(DEFAULT_SAVE_FILENAME);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(DEFAULT_SAVE_FILENAME);
        }

        public void DeleteSave()
        {
            GetComponent<SavingSystem>().Delete(DEFAULT_SAVE_FILENAME);
        }
    }
}

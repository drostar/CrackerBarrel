using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Foundation.Databinding;

namespace CrackerBarrel
{
    public class SaveDialogViewModel : ObservableBehaviour
    {
        public bool IsValidName { get { return !string.IsNullOrEmpty(SaveName); } }

        private string _saveName;
        public string SaveName
        {
            get { return _saveName; }
            set {
                if (value == _saveName)
                    return;
                _saveName = value;
                RaiseBindingUpdate(nameof(SaveName), _saveName);
                RaiseBindingUpdate(nameof(IsValidName), IsValidName);
            }
        }
                
        public DialogResult UserResponse { get; set; }

        private Action<DialogResult> callback;

        public void Open(Action<DialogResult> userResponse)
        {
            callback = userResponse;
            gameObject.SetActive(true);
        }

        public void Save()
        {
            var r = new DialogResult() { Name = SaveName };
            UserResponse = r;
            close();
            callback?.Invoke(r);
        }

        public void Cancel()
        {
            var r = new DialogResult() { Canceled = true };
            UserResponse = r;
            close();
            callback?.Invoke(r);
        }

        private void close()
        {
            gameObject.SetActive(false);
        }
    }

}
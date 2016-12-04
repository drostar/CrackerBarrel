using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using Foundation.Databinding;

namespace CrackerBarrel
{
    public class LoadDialogViewModel : MonoBehaviour
    {
        public ObservableCollection<SavedBoardViewModel> AvailableBoards { get; set; } = new ObservableCollection<SavedBoardViewModel>();

        public DialogResult UserResponse { get; set; }
        private Action<DialogResult> callback;

        public void Open(string[] availableItems, Action<DialogResult> userResponse)
        {
            refreshAvailableItems(availableItems);

            callback = userResponse;
            gameObject.SetActive(true);
        }

        public void CloseWithResponse(string name)
        {
            var r = new DialogResult() { Name = name };
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

        private void refreshAvailableItems(IEnumerable<string> availableItems)
        {
            AvailableBoards.Clear();
            foreach (var item in availableItems)
            {
                AvailableBoards.Add(new SavedBoardViewModel() { Name = item });
            }
        }
    }

    public class SavedBoardViewModel : ObservableBehaviour
    {
        public string Name { get; set; }

        public void Select()
        {
            GetComponentInParent<LoadDialogViewModel>().CloseWithResponse(Name);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GenericSelectionUI
{
    public class SelectionUI<T> : MonoBehaviour where T:ISelectableItem
    {
        List<T> items;
        protected int selection = 0;
        float selectionTime = 0;
        [SerializeField] float selectionBuffer = 0.2f;


        public event Action<int> OnSelected;
        public event Action OnBack;
        public void SetItems(List<T> items)
        {
            this.items = items;
            UpdateSelectionInUI();
        }

        public virtual void HandleUpdate()
        {
            UpdateSelectionTimer();
            int prevSelection = selection;

            HandleListSelection();

            selection = SelectionClamp(selection, 0, items.Count - 1);

            if (selection != prevSelection)
            {
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
                UpdateSelectionInUI();
            }

            if (Input.GetButtonDown("Selection"))
            {
                OnSelected?.Invoke(selection);
            }
            else if (Input.GetButtonDown("GoBack") || Input.GetKeyDown(KeyCode.Return))
            {
                OnBack?.Invoke();
            }
        }

        void HandleListSelection()
        {
            var v = Input.GetAxis("Vertical");
            if (selectionTime == 0 && Mathf.Abs(v) > selectionBuffer)
            {
                selection += -(int)(Mathf.Sign(v));
                selectionTime = selectionBuffer;
            }
                

        }

        void HandleGridSelection()
        {

        }

        private void UpdateSelectionInUI()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetSelected(i == selection);
            }
        }

        void UpdateSelectionTimer()
        {
            if (selectionTime > 0)
            {
                selectionTime -= Time.deltaTime;
                selectionTime = Mathf.Clamp(selectionTime, 0, selectionBuffer);
            }
        }

        private int SelectionClamp(int n, int min, int max)
        {
            if (max <= min || n > max)
                return max;
            if (n < min)
                return min;
            return n;
        }

    }
}


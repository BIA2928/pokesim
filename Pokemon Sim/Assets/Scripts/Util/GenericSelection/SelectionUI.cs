using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GenericSelectionUI
{
    public enum SelectionType
    {
        LinearList, Grid
    }
    public class SelectionUI<T> : MonoBehaviour where T:ISelectableItem
    {

        List<T> items;
        protected int selection = 0;
        float selectionTime = 0;
        [SerializeField] float selectionBuffer = 0.2f;
        SelectionType selectionType;
        int gridWidth = 2;

        public event Action<int> OnSelected;
        public event Action OnBack;
        public void SetItems(List<T> items)
        {
            this.items = items;
            items.ForEach(i => i.Init());
            UpdateSelectionInUI();
        }

        public void ClearItems()
        {
            items?.ForEach(i => i.Clear());
            this.items = null;
        }

        public void SetSelectionSettings(SelectionType selectionType, int gridWidth)
        {
            this.selectionType = selectionType;
            this.gridWidth = gridWidth;
        }

        public virtual void HandleUpdate()
        {
            UpdateSelectionTimer();
            int prevSelection = selection;

            if (selectionType == SelectionType.LinearList)
                HandleListSelection();
            else
                HandleGridSelection();

            selection = MinClamp(selection, 0, items.Count - 1);

            if (selection != prevSelection)
            {
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
                UpdateSelectionInUI();
            }

            if (Input.GetButtonDown("Selection"))
            {
                if (items.Count != 0)
                {
                    AudioManager.i.PlaySFX(AudioID.UISelect);
                    OnSelected?.Invoke(selection);
                }
                    
            }
            else if (Input.GetButtonDown("GoBack") || Input.GetKeyDown(KeyCode.Return))
            {
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
                OnBack?.Invoke();
            }
        }

        void HandleListSelection()
        {
            var v = Input.GetAxisRaw("Vertical");
            if (selectionTime == 0 && Mathf.Abs(v) > selectionBuffer)
            {
                selection += -(int)(Mathf.Sign(v));
                selectionTime = selectionBuffer;
            }


        }

        void HandleGridSelection()
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            if (selectionTime == 0 && (Mathf.Abs(vertical) > selectionBuffer || Mathf.Abs(horizontal) > selectionBuffer))
            {
                if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
                    selection += (int)(Mathf.Sign(horizontal));
                else
                    selection += -(int)(Mathf.Sign(vertical)) * gridWidth;
                    
                selectionTime = selectionBuffer;

            }
        }

        protected virtual void UpdateSelectionInUI()
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

        private int MaxClamp(int n, int min, int max)
        {
            if (max <= min || n > max)
                return max;
            if (n < min)
                return min;
            return n;
        }

        private int MinClamp(int n, int min, int max)
        {
            if (min >= max || n < min)
                return min;
            if (n > max)
                return max;
            return n;
        }

    }
}


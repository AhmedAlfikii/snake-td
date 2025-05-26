using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit;
using UnityEngine.Events;

namespace TafraKit.UI
{
    /// <summary>
    /// Highlighets elements in a horizontal or vertical layout group by increasing their size.
    /// </summary>
    public class SelectableCellChain : MonoBehaviour
    {
        #region Private Serialized Fields
        [Tooltip("Should cell deselection be allowed?")]
        [SerializeField] private bool allowCellDeselection = true;
        [Tooltip("Should there be a maximum of one cell selected at a time? if true, selecting a new cell will deselet the current selected one.")]
        [SerializeField] private bool oneCellAtATime = true;
        [Tooltip("The index of the cell that will be automatically opened on start.")]
        [SerializeField] private int defaultCell = -1;

        [Header("Initializatation")]
        [SerializeField] private bool initializeAtLateStart = true;

        [Header("SFX")]
        [SerializeField] private SFXClip onCellSelected;
        #endregion

        #region Public Events
        public UnityEvent<int> OnCellAboutToBeSelected = new UnityEvent<int>();
        public UnityEvent<int> OnCellSelected = new UnityEvent<int>();
        public UnityEvent<int> OnCellDeselected = new UnityEvent<int>();
        #endregion

        #region Private Fields
        private HorizontalOrVerticalLayoutGroup layoutController;
        private RectTransform myRT;
        private List<ChainCell> cells = new List<ChainCell>();
        private bool isHorizontal;
        private int lastSelectedCellIndex = -1;
        private bool suppressNextCellSelection;
        #endregion

        #region MonoBehaviour Messages
        IEnumerator Start()
        {
            if (initializeAtLateStart)
                yield return Yielders.EndOfFrame;

            myRT = GetComponent<RectTransform>();
            layoutController = GetComponent<HorizontalOrVerticalLayoutGroup>();

            isHorizontal = layoutController is HorizontalLayoutGroup;

            if (transform.childCount > 0)
            {
                int cellIndex = 0;
                //Get all the children (cells) of the layout group.
                for (int i = 0; i < transform.childCount; i++)
                {
                    ChainCell c = transform.GetChild(i).GetComponent<ChainCell>();

                    if (c)
                    {
                        c.Initialize(this, isHorizontal, cellIndex);
                        cells.Add(c);

                        cellIndex++;
                    }
                }
            }

            if (defaultCell > -1)
                SelectCell(defaultCell);
        }
        #endregion

        #region Public Functions
        public void SelectCell(int cellIndex)
        {
            if (cellIndex == lastSelectedCellIndex)
                return;

            OnCellAboutToBeSelected?.Invoke(cellIndex);

            if(suppressNextCellSelection)
            {
                suppressNextCellSelection = false;
                return;
            }

            ChainCell cell = cells[cellIndex];

            cell.SetAsSelected();

            if (oneCellAtATime && lastSelectedCellIndex != -1)
                DeselectCell(lastSelectedCellIndex);

            lastSelectedCellIndex = cellIndex;

            SFXPlayer.Play(onCellSelected);

            OnCellSelected?.Invoke(cellIndex);
        }

        public void DeselectCell(int cellIndex)
        {
            if (cellIndex != lastSelectedCellIndex)
                return;

            ChainCell cell = cells[cellIndex];

            cell.SetAsDeselected();

            if (cellIndex == lastSelectedCellIndex)
                lastSelectedCellIndex = -1;

            OnCellDeselected?.Invoke(cellIndex);
        }

        //Selects a cell if it's deselected, and deselects a cell if it's selected.
        public void SwitchCell(int cellIndex)
        {
            if (lastSelectedCellIndex != cellIndex)
                SelectCell(cellIndex);
            else
                DeselectCell(cellIndex);
        }

        public void CellAction(int cellIndex)
        {
            if (!allowCellDeselection && cells[cellIndex].IsSelected)
                return;

                SwitchCell(cellIndex);
        }

        public void DeselectLastSelected()
        {
            if (lastSelectedCellIndex > -1)
                DeselectCell(lastSelectedCellIndex);
        }

        public void SetDefaultCell(int cellIndex)
        {
            defaultCell = cellIndex;
        }
        public void SuppressNextCellSelection()
        {
            suppressNextCellSelection = true;
        }
        #endregion
    }
}
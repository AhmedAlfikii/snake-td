using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TafraKit;
using UnityEngine.Events;
using System;

namespace ZUtilities
{
    [RequireComponent(typeof(ScrollRect))]
    public class PagedScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        #region Classes, Structs & Enums
        public enum ScrollDirection { Horizontal, Vertical }
        #endregion

        #region Private Serialized Fields
        [SerializeField] private int pagesCount = 5;
        [SerializeField] private ScrollDirection scrollDirection;
        [Tooltip("Switch to the next/previous page if this percentage of the current page has passed by.")]
        [Range(0, 1)] [SerializeField] private float pageChangePercentage = 0.25f;
        [Tooltip("The duration in seconds the scroll will take to move to the desired page after the player has finished dragging it.")]
        [SerializeField] private float changingPageDuration = 0.25f;
        [Tooltip("Should the scroll snap to the determined start page number on awake?")]
        #endregion

        #region Public Events
        public UnityEvent<int> OnPageChanged = new UnityEvent<int>();
        #endregion

        #region Private Properties
        private float scrollValue
        {
            get
            {
                if (scrollDirection == ScrollDirection.Horizontal)
                    return scrollRect.horizontalNormalizedPosition;
                else
                    return scrollRect.verticalNormalizedPosition;
            }
            set
            {
                if (scrollDirection == ScrollDirection.Horizontal)
                    scrollRect.horizontalNormalizedPosition = value;
                else
                    scrollRect.verticalNormalizedPosition = value;
            }
        }
        #endregion

        #region Private Fields
        private bool initialized;
        private ScrollRect scrollRect;
        private float pageWidth;
        private int curPageIndex;
        private IEnumerator movingToCurPageEnum;
        #endregion

        #region MonoBehaviour Messages
        void Awake()
        {
            if (!initialized)
                Initialize();
        }
        #endregion

        #region Private Functions
        IEnumerator MovingToCurrentPage()
        {
            float startTime = Time.time;
            float endTime = startTime + changingPageDuration;

            float startScrollValue = scrollValue;
            float endScrollValue = curPageIndex * pageWidth;

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / changingPageDuration;

                t = MotionEquations.EaseOut(t, 2);

                scrollValue = Mathf.Lerp(startScrollValue, endScrollValue, t);

                yield return null;
            }
        }
        #endregion

        #region Callbacks
        public void OnEndDrag(PointerEventData eventData)
        {
            float passedPages = scrollValue / pageWidth;

            float passedPagePercentage = passedPages - curPageIndex;

            int direction = (int)Mathf.Sign(passedPagePercentage);

            passedPagePercentage = Mathf.Abs(passedPagePercentage);

            int targetPageIndex = curPageIndex;

            //If the passed page percentage (regardless of its direction) is greater than or equal to the "page change percentage", then set the next/previous page as the current page.
            if (passedPagePercentage >= pageChangePercentage)
            {
                int pages = direction;

                //If more than one page have passed, then check if the "page change percentage" have passed of the last page, if it did, then move to it, if not, then move to the page before it.
                if (passedPagePercentage > 1)
                {
                    float friction = passedPagePercentage - (int)passedPagePercentage;

                    if (friction >= pageChangePercentage)
                    {
                        if (direction > 0)
                            pages = Mathf.CeilToInt(passedPagePercentage * direction);
                        else
                            pages = Mathf.FloorToInt(passedPagePercentage * direction);
                    }
                    else
                    {
                        if (direction > 0)
                            pages = Mathf.FloorToInt(passedPagePercentage * direction);
                        else
                            pages = Mathf.CeilToInt(passedPagePercentage * direction);
                    }
                }

                targetPageIndex = Mathf.Clamp(targetPageIndex + pages, 0, pagesCount - 1);
            }

            GoToPage(targetPageIndex);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (movingToCurPageEnum != null)
                StopCoroutine(movingToCurPageEnum);
        }
        #endregion

        #region Public Functions
        public void Initialize(int pages = -1)
        {
            if (pages > -1)
                pagesCount = pages;

            scrollRect = GetComponent<ScrollRect>();

            pageWidth = 1 / (float)(pagesCount - 1);
            
            initialized = true;
        }

        public void ChangePagesCount(int count)
        {
            if(!initialized)
                Initialize(count);

            pagesCount = count;
            pageWidth = 1 / (float)(pagesCount - 1);

            curPageIndex = Mathf.Clamp(curPageIndex, 0, count - 1);

            scrollValue = curPageIndex * pageWidth;
        }

        public void GoToPage(int pageIndex, bool immediate = false)
        {
            bool pageChanged = pageIndex != curPageIndex;

            if (pageIndex < pagesCount && pageIndex >= 0)
                curPageIndex = pageIndex;
            else
            {
                Debug.LogError("The page index is out of range. Make sure it's greater than or equal to zero and less than the total pages count.");
                return;
            }

            if (movingToCurPageEnum != null)
                StopCoroutine(movingToCurPageEnum);

            if (!immediate && gameObject.activeInHierarchy)
            {
                movingToCurPageEnum = MovingToCurrentPage();

                StartCoroutine(movingToCurPageEnum);
            }
            else
                scrollValue = curPageIndex * pageWidth;

            if(pageChanged)
                OnPageChanged?.Invoke(curPageIndex);
        }

        public void GoToNextPage()
        {
            if (curPageIndex + 1 < pagesCount)
                GoToPage(curPageIndex + 1);
            else
                return;
        }
        public void GoToPreviousPage()
        {
            if(curPageIndex - 1 >= 0)
                GoToPage(curPageIndex - 1);
            else
                return;
        }
        public void GoToLastPage()
        {
            GoToPage(pagesCount - 1);
        }
        public bool IsOnFirstPage()
        { 
            return curPageIndex == 0;
        }
        public bool IsOnLastPage()
        {
            return curPageIndex == pagesCount - 1;
        }

        #endregion
    }
}
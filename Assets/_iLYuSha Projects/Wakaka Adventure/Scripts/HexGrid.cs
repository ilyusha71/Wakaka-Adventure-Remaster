﻿/* * * * * * * * * * * * * * * * * * * * *
 * 
 *    Title: "六角網格實例"
 * 
 *    Dsecription:
 *                 功能: 記錄網格資訊（Trap = -99, Treasure = -1, General = 0, Special = 1~6）
 *                           顯示對應圖示
 * 
 *     Author: iLYuSha
 *     
 *     Date: 2018.03.24
 *     
 *     Modify: 
 *                  1. 刪除 isDisappear 參數以及Update方法，改用DoTween實現陷阱特效
 *     
 * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace WakakaAdventureSpace
{
    public delegate void FirstTryEventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : System.EventArgs;
    public class FirstTryEventArgs : System.EventArgs
    {
        public readonly HexGrid firstTry;
        public readonly int firstIndex;

        public FirstTryEventArgs(HexGrid firstTry, int firstIndex)
        {
            this.firstTry = firstTry;
            this.firstIndex = firstIndex;
        }
    }

    public class HexGrid : MonoBehaviour
    {
        public HexGrid[] Neighbor;
        public int[] NeighborR;
        public List<int> NeighborIndex;
        [HideInInspector]
        public int indexGrid;
        public int gridInfo; // 網格資訊
        [HideInInspector]
        public Image suspected;
        [HideInInspector]
        public Vector3 gridCoordinate;
        [HideInInspector]
        public Image gridImage;
        [HideInInspector]
        public PolygonCollider2D gridCollider;
        [HideInInspector]
        public CanvasGroup gridCanvas;
        [HideInInspector]
        public HexGridManager manager;
        public int indexTheme;
        public ThemeManager themeManager;
        public bool isExplored; // 判斷此區域是否已經探索
        public bool isNumerical; // 特殊圖樣數值化
        public bool isStable; // 陷阱不會清除周圍區域
        public bool isSuspected; // 疑似寶藏

        public event FirstTryEventHandler<FirstTryEventArgs> FirstTryEvent;
        bool buttonDown;

        private float alphaUnknown = 0.73f;
        private float alphaExplore = 0.84f;
        private float alphaExcavation = 0.93f;

        void Awake()
        {
            suspected = transform.GetChild(0).GetComponent<Image>();
            gridImage = GetComponent<Image>();
            gridCollider = GetComponent<PolygonCollider2D>();
            gridCollider.SetPath(0, HexagonMesh.vertex);
            gridCanvas = GetComponent<CanvasGroup>();
        }
        public void Reset()
        {
            suspected.enabled = false;
            gridImage.sprite = themeManager.iconUnknownTool.sprite;
            gridCollider.enabled = true;
            gridCanvas.DOKill();
            gridCanvas.alpha = alphaUnknown;
            gridInfo = 0;
            isExplored = false;
            isSuspected = false;
        }
        void OnMouseOver()
        {
            if (isExplored)
                return;

            //buttonDown = true;
            //if (!HexGridManager.firstTry)
            //    FirstTryEvent(this, new FirstTryEventArgs(this, indexGrid));
            //else
            //    Working();
            //buttonDown = false;



#if !UNITY_ANDROID && !UNITY_IOS
            if (Input.GetMouseButtonDown(0))
            {
                buttonDown = true;
                if (!HexGridManager.firstTry)
                    FirstTryEvent(this, new FirstTryEventArgs(this, indexGrid));
                else
                    Research();
                buttonDown = false;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                buttonDown = true;
                PlaceFlag();
                buttonDown = false;
            }
#else
            if (Input.GetMouseButtonDown(0))
            {
                buttonDown = true;
                if (!HexGridManager.firstTry)
                    FirstTryEvent(this, new FirstTryEventArgs(this, indexGrid));
                else
                    Working();
                buttonDown = false;
            }
#endif
        }
        public void MissClickEvent() // 禁止觸發網格事件
        {
            gridCollider.enabled = false; // 關閉網格Collider
        }
        public void Working()
        {
            if (isExplored)
                return;
            switch (manager.adventureMode)
            {
                case AdventureMode.Explore: Explore(); break;
                case AdventureMode.Tool: UseTool(); break;
                case AdventureMode.Flag: PlaceFlag(); break;
            }
        }
        public void Research()
        {
            if (!isExplored)
            {
                isExplored = true;
                gridCanvas.DOKill();
                gridCanvas.alpha = alphaExplore;

                if (gridInfo == -99)
                    IsTreasure();
                else if (gridInfo == -1)
                    IsTrap();
                else if (gridInfo == 0)
                    IsGeneral();
                else
                    IsSpecial();
            }
        } // 研究（密室逃脫版本：結合《探索》與《工具》方法）
        public void Explore() // 探索（探險隊）
        {
            if (!isExplored)
            {
                isExplored = true;
                gridCanvas.DOKill();
                gridCanvas.alpha = alphaExplore;

                if (gridInfo == -99 || gridInfo == -1)
                    IsTrap();
                else if (gridInfo == 0)
                    IsGeneral();
                else
                    IsSpecial();
            }
        }
        void UseTool() // 使用工具（考古隊 or 無人機）
        {
            if (gridInfo == -99)
                IsTreasure();
            else if (gridInfo == -1)
                IsTrap();
            else
                IsUnknown();
            gridCanvas.alpha = alphaExcavation;
        }
        void PlaceFlag() // 放置旗標
        {
            gridImage.sprite = gridImage.sprite != themeManager.spriteGobi[20] ? themeManager.spriteGobi[20] : themeManager.iconUnknownTool.sprite;
        }

        public void Disappear()
        {
            if (gridImage.sprite != themeManager.iconTitleTreasure.sprite)
            {
                isExplored = false;
                gridCanvas.DOFade(0, 2.37f).OnComplete(() => Clear());
            }
        }
        void Clear()
        {
            if (gridImage.sprite != themeManager.spriteGobi[20])
                gridImage.sprite = themeManager.iconUnknownTool.sprite;
            gridCollider.enabled = true;
            gridCanvas.alpha = alphaUnknown;
            isExplored = false;
        }

        /* 網格揭示 */
        void IsGeneral()
        {
            gridImage.sprite = themeManager.iconGeneralExplore.sprite;
            manager.ExploreNeigbor(gridCoordinate);
            if (buttonDown)
                manager.AudioPlay(2);
        }
        void IsSpecial()
        {
            if (!isNumerical) // 圖像化提示
                gridImage.sprite = themeManager.theme[indexTheme].specialGroup.sprite[gridInfo];
            else // 數字提示
                gridImage.sprite = themeManager.theme[indexTheme].specialGroup.sprite[gridInfo + 6];

            if (buttonDown)
                manager.AudioPlay(3);
        }
        void IsTrap()
        {
            gridImage.sprite = themeManager.iconTitleTrap.sprite;
            if (!isStable) // 清除周圍
            {
                Disappear();
                manager.ClearNeigbor(gridCoordinate);
            }
            manager.IntoTrap();
            if (buttonDown)
                manager.AudioPlay(1);
        }
        void IsUnknown()
        {
            gridImage.sprite = themeManager.iconUnknownTool.sprite;
        }
        void IsTreasure()
        {
            isExplored = true;
            gridCanvas.DOKill();
            gridImage.sprite = themeManager.iconTitleTreasure.sprite;
            manager.GetTarget();
            if (buttonDown)
                manager.AudioPlay(0);
        }

        // Help Method
        public void Numerical()
        {
            isNumerical = !isNumerical;
            if (gridInfo >= 1 && gridInfo <= 6)
            {
                if (isExplored)
                {
                    if (isNumerical)
                    {
                        transform.localRotation = Quaternion.identity;
                        gridImage.sprite = themeManager.theme[indexTheme].specialGroup.sprite[gridInfo + 6];
                    }
                    else
                    {
                        transform.localRotation *= Quaternion.Euler(0, 0, Random.Range(0, 6) * 60);
                        gridImage.sprite = themeManager.theme[indexTheme].specialGroup.sprite[gridInfo];
                    }
                }
            }
        }
    }
}
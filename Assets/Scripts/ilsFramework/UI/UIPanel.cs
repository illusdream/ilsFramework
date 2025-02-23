using System;
using UnityEngine;

namespace ilsFramework
{
    [Serializable]
    public abstract class UIPanel
    {
        public GameObject UIPanelObject { get; set; }
        
        public Canvas Canvas { get; set; }
        public CanvasGroup UIPanelCanvasGroup { get; set; }
        
        
        public virtual void InitUIPanel() { }
        
        public virtual void Open()
        {
            UIPanelCanvasGroup.alpha = 1;
            UIPanelCanvasGroup.blocksRaycasts = true;
            UIPanelCanvasGroup.interactable = true;
        }

        public virtual void Close()
        {
            UIPanelCanvasGroup.alpha = 0;
            UIPanelCanvasGroup.blocksRaycasts = false;
            UIPanelCanvasGroup.interactable = false;
        }
        
        public virtual void Pause()
        {
            UIPanelCanvasGroup.blocksRaycasts = true;
            UIPanelCanvasGroup.interactable = true;
        }

        public virtual void Resume()
        {
            UIPanelCanvasGroup.blocksRaycasts = false;
            UIPanelCanvasGroup.interactable = false;
        }

        public virtual void Update()
        {
            
        }

        public virtual void LateUpdate()
        {
            
        }

        public virtual void FixedUpdate()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }
    }
}
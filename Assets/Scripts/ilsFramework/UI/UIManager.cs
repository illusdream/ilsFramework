using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework
{
    public class UIManager : ManagerSingleton<UIManager>,IManager,IAssemblyForeach
    {
        GameObject UIRoot;
            
        GameObject Bottom;
        GameObject Lower;
        GameObject Normal;
        GameObject Upper;
        GameObject Top;
        GameObject Debug;


        /// <summary>
        /// UI层级间的SortOrder间隔
        /// </summary>
        private int _UILayerInterval = 100;
        
        private Dictionary<Type, UIPanelSetting> uiPanelSettings;
        [ShowInInspector]
        private Dictionary<Type, UIPanel> uiPanels;

        private List<(Type, UIPanel)> needAddToDic;
        private List<Type> needRemoveFromDic;
        
        public void Init()
        {
            uiPanelSettings = new Dictionary<Type, UIPanelSetting>();
            uiPanels= new Dictionary<Type, UIPanel>();
            
            needAddToDic = new List<(Type, UIPanel)>();
            needRemoveFromDic = new List<Type>();
            
            InitUIBaseFramework();
        }
        public void ForeachCurrentAssembly(Type[] types)
        {
            foreach (var type in types)
            {
                if (typeof(UIPanel).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    var attrs = type.GetCustomAttributes(typeof(UIPanelSetting), false);
                    UIPanelSetting setting = null;
                    if (attrs.Length == 1)
                    {
                        setting = (UIPanelSetting)attrs[0];
                    }
                    else
                    {
                        setting = new UIPanelSetting(EUILayer.Normal, 0,false, EAssetLoadMode.Resources, "ilsFramework/Prefab/UI/BasePanel");
                    }
                    uiPanelSettings.TryAdd(type, setting);
                }
            }
        }
        public void Update()
        {
            foreach (var tuple in needAddToDic)
            {
                uiPanels[tuple.Item1]= tuple.Item2;
            }

            foreach (var needRemove in needRemoveFromDic)
            {
                if (uiPanels.TryGetValue(needRemove,out var panel))
                {
                    panel.OnDestroy();
                    GameObject.Destroy(panel.UIPanelObject);
                }
                uiPanels.Remove(needRemove);
                
            }

            foreach (var uiPanel in uiPanels)
            {
                uiPanel.Value.Update();
            }
        }

        public void LateUpdate()
        {
            foreach (var uiPanel in uiPanels)
            {
                uiPanel.Value.LateUpdate();
            }
        }

        public void FixedUpdate()
        {
            foreach (var uiPanel in uiPanels)
            {
                uiPanel.Value.FixedUpdate();
            }
        }

        public void OnDestroy()
        {
            foreach (var uiPanel in uiPanels)
            {
                uiPanel.Value.OnDestroy();
                GameObject.Destroy(uiPanel.Value.UIPanelObject);
            }
            uiPanels.Clear();
        }

        public void OnDrawGizmos()
        {
            
        }

        public void OnDrawGizmosSelected()
        {
           
        }

        public void InitUIBaseFramework()
        {
            UIRoot = new GameObject("UIRoot");
            UIRoot.layer = LayerMask.NameToLayer("UI");
            UIRoot.transform.parent = ContainerObject.transform;
            
            Bottom = new GameObject("Bottom");
            Bottom.layer = LayerMask.NameToLayer("UI");
            Bottom.transform.parent = UIRoot.transform;
            
            Lower = new GameObject("Lower");
            Lower.layer = LayerMask.NameToLayer("UI");
            Lower.transform.parent = UIRoot.transform;
            
            Normal = new GameObject("Normal");
            Normal.layer = LayerMask.NameToLayer("UI");
            Normal.transform.parent = UIRoot.transform;
            
            Upper = new GameObject("Upper");
            Upper.layer = LayerMask.NameToLayer("UI");
            Upper.transform.parent = UIRoot.transform;
            
            Top = new GameObject("Top");
            Top.layer = LayerMask.NameToLayer("UI");
            Top.transform.parent = UIRoot.transform;
            
            Debug = new GameObject("Debug");
            Debug.layer = LayerMask.NameToLayer("UI");
            Debug.transform.parent = UIRoot.transform;
        }

        public (Transform, int) GetUILayerInfo(EUILayer layer)
        {
            switch (layer)
            {
                case EUILayer.Bottom:
                    return (Bottom.transform, (int)EUILayer.Bottom * _UILayerInterval);
                case EUILayer.Lower:
                    return (Lower.transform, (int)EUILayer.Lower * _UILayerInterval);
                case EUILayer.Normal:
                    return (Normal.transform, (int)EUILayer.Normal * _UILayerInterval);
                case EUILayer.Upper:
                    return (Upper.transform, (int)EUILayer.Upper * _UILayerInterval);
                case EUILayer.Top:
                    return (Top.transform, (int)EUILayer.Top * _UILayerInterval);
                case EUILayer.Debug:
                    return (Debug.transform, (int)EUILayer.Debug * _UILayerInterval);
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
        }
        
        public T LoadUIPanel<T>() where T : UIPanel
        {
            Type type = typeof(T);
            if (uiPanels.TryGetValue(type,out var value))
            {
                return value as T;
            }

            if (uiPanelSettings.TryGetValue(type,out var setting))
            {
                UIPanel instance = Activator.CreateInstance<T>();
                
                //加载基础部分
                var uiPanelGameObject = AssetManager.Instance.Load<GameObject>(setting.AssetLoadMode, setting.LoadAssetStr);
                
                if (uiPanelGameObject)
                {
                    var goInstance=  GameObject.Instantiate(uiPanelGameObject);
                    instance.UIPanelObject = goInstance;
                    if (goInstance.TryGetComponent<CanvasGroup>(out var canvasGroup))
                        instance.UIPanelCanvasGroup = canvasGroup;
                    else
                        throw new ArgumentException($"组件CanvasGroup不存在对应资源:{setting.LoadAssetStr}|加载模式:{setting.AssetLoadMode}上");
                    
                    if (goInstance.TryGetComponent<Canvas>(out var canvas))
                        instance.Canvas = canvas;
                    else
                        throw new ArgumentException($"组件Canvas不存在对应资源:{setting.LoadAssetStr}|加载模式:{setting.AssetLoadMode}上");
                    
                    
                    //加载额外的UI组件
                    var needFields = AssemblyUtils.GetAllFieldInfoWithAttribute<AutoUIElement>(type);
                    foreach (var field in needFields)
                    {
                        object uiElement = null;
                        
                        //获取对应地址
                        var objectList = field.Item2.TargetComponentPath.Split("/");
                        Transform childObject = null;
                        if (field.Item2.TargetComponentPath == "")
                        {
                            childObject = goInstance.transform;
                        }
                        for (int i = 0; i < objectList.Length; i++)
                        {
                            if (i == 0)
                            {
                                childObject = goInstance.transform.Find(objectList[i]);
                            }
                            else
                            {
                                if (!childObject)
                                {
                                    continue;
                                }

                                childObject = childObject.Find(objectList[i]);
                            }
                        }


                        if (childObject && childObject.gameObject.TryGetComponent(field.Item1.FieldType, out var elementInstance))
                        {
                            uiElement = elementInstance;
                        }
                        
                        
                        field.Item1.SetValue(instance,uiElement);
                    }
                    
                    var needProperties = AssemblyUtils.GetAllPropertyInfoWithAttribute<AutoUIElement>(type);
                    foreach (var property in needProperties)
                    {
                        if (property.Item1.GetSetMethod(nonPublic:true) == null)
                        {
                            continue;
                        }
                        object uiElement = null;
                        
                        //获取对应地址
                        var objectList = property.Item2.TargetComponentPath.Split("/");
                        Transform childObject = null;
                        if (property.Item2.TargetComponentPath == "")
                        {
                            childObject = goInstance.transform;
                        }
                        else
                        {
                            for (int i = 0; i < objectList.Length; i++)
                            {
                                if (i == 0)
                                {
                                    childObject = uiPanelGameObject.transform.Find(objectList[i]);
                                }
                                else
                                {
                                    if (!childObject)
                                    {
                                        continue;
                                    }
                                    childObject = childObject.Find(objectList[i]);
                                }
                            }
                        }

                        if (childObject && childObject.gameObject.TryGetComponent(property.Item1.PropertyType, out var elementInstance))
                        {
                            uiElement = elementInstance;
                        }
                        
                        
                        property.Item1.SetValue(instance,uiElement);
                    }


                    var LayerInfo = GetUILayerInfo(setting.UILayer);
                    goInstance.transform.parent = LayerInfo.Item1;
                    
                    instance.Canvas.sortingOrder = LayerInfo.Item2 + setting.LayerOffest;
                }
                needAddToDic.Add((type, instance));
                instance.InitUIPanel();
                instance.Close();
                return instance as T;
            }

            return null;
        }

        public T GetUIPanel<T>() where T : UIPanel
        {
            if (uiPanels.TryGetValue(typeof(T),out var result) && result is T panel)
            {
                return panel;
            }
            else
            {
                return LoadUIPanel<T>();;
            }
        }

        public void UnLoadUIPanel<T>() where T : UIPanel
        {
            if (uiPanels.ContainsKey(typeof(T)))
            {
                needRemoveFromDic.Remove(typeof(T));
            }
        }

    }
    
}
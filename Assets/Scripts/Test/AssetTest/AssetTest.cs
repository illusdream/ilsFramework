using System.Collections;
using System.Collections.Generic;
using ilsFramework;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using AssetConfig = ilsFramework.Core.AssetConfig;

public class AssetTest : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public AssetReference<Sprite> testReference;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Button]
    public void LoadSprite()
    {
        var config = Config.GetConfig<AssetConfig>();
       // spriteRenderer.sprite =;
       spriteRenderer.sprite =Asset.Load(testReference);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;

    private List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    void Start()
    {
        if (parallaxCamera == null)
        {
            Camera.main.TryGetComponent(out parallaxCamera);
        }

        SetLayers();

        if (parallaxCamera != null)
        {
            parallaxCamera.onCameraTranslate += Move;
        }
    }

    void OnDestroy()
    {
        if (parallaxCamera != null)
        {
            parallaxCamera.onCameraTranslate -= Move;
        }
    }

    void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer =
                transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.name = "Layer-" + i;
                parallaxLayers.Add(layer);
            }
        }
    }

    void Move(float delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            if (layer != null)
            {
                layer.Move(delta);
            }
        }
    }
}
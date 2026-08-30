using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.8f;

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;

        newPos.x -= delta * parallaxFactor;

        transform.localPosition = newPos;
    }
}
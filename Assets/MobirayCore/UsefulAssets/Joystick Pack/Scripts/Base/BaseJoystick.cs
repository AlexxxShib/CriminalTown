using UnityEngine;

public class BaseJoystick : MonoBehaviour
{
    public virtual float Horizontal { get; protected set; }
    public virtual float Vertical { get; protected set; }
    
    
#if UNITY_2021
    public Vector2 Direction => new(Horizontal, Vertical);

    public Vector3 Direction3D => new(Horizontal, 0, Vertical);
#endif

#if UNITY_2020
    public Vector2 Direction => new Vector2(Horizontal, Vertical);

    public Vector3 Direction3D => new Vector3(Horizontal, 0, Vertical);
#endif

    public bool HasInput => Direction.magnitude > 0.1f;
}
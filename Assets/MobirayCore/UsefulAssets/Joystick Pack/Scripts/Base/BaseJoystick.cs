using UnityEngine;

public class BaseJoystick : MonoBehaviour
{
    public virtual float Horizontal { get; protected set; }
    public virtual float Vertical { get; protected set; }
    
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }
}
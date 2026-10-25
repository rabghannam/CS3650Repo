using UnityEngine;

/// <summary>
/// Abstract base entity. Matches the UML "GameObject" abstract class:
/// position + polymorphic Update/Render.
/// </summary>
public abstract class GameObjectBase : MonoBehaviour
{
    public Vector2 position
    {
        get => transform.position;
        set => transform.position = new Vector3(value.x, value.y, transform.position.z);
    }

    /// <summary>Frame-rate independent update.</summary>
    public abstract void CustomUpdate(float deltaTime);

    /// <summary>Visual rendering hook (hit effects, flashes, etc.).</summary>
    public virtual void Render() { }

    protected virtual void Update()
    {
        CustomUpdate(Time.deltaTime); // frame-rate independent
    }
}
using UnityEngine;

/// <summary>
/// US-001: reads WASD and arrow keys at the same time.
/// Both control schemes are OR-ed so they never conflict.
/// </summary>
public class InputHandler : MonoBehaviour
{
    public struct InputState { public Vector2 moveDirection; }

    private InputState state;

    public bool isMoveUp() => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    public bool isMoveDown() => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    public bool isMoveLeft() => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    public bool isMoveRight() => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

    public InputState readKeyboard()
    {
        Vector2 dir = Vector2.zero;
        if (isMoveUp()) dir.y += 1f;
        if (isMoveDown()) dir.y -= 1f;
        if (isMoveLeft()) dir.x -= 1f;
        if (isMoveRight()) dir.x += 1f;
        state = new InputState { moveDirection = dir.normalized };
        return state;
    }
}
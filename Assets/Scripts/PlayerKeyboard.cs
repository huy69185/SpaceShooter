using UnityEngine;

public class PlayerKeyboard : MonoBehaviour
{
    private bool _inputMovePressed;
    private float _inputRotation;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) _inputMovePressed = true;
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) _inputMovePressed = false;
        if (Input.GetKey(KeyCode.Space)) Player.Instance.InputShootPressed = true;
        if (Input.GetKeyUp(KeyCode.Space)) Player.Instance.InputShootPressed = false;

        _inputRotation = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (Player.Instance.Lifes < 0)
        {
            if (Player.Instance.Rb.velocity != Vector2.zero) Player.Instance.Rb.velocity = Vector2.zero;
            return;
        }

        if (PauseManager.Instance.Paused || !StartManager.Instance.GameStarted) return;

        Move();
        Rotate();
    }

    private void Move()
    {
        if (_inputMovePressed)
        {
            if (!Player.Instance.MovedFirstTime)
            {
                Player.Instance.MovedFirstTime = true;
            }

            float angle = (Player.Instance.Sprite.transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
            Player.Instance.Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Player.Instance.Rb.velocity = Player.Instance.Direction * Player.Instance.MoveSpeed * Time.fixedDeltaTime;

            if (Player.Instance.InertiaScale != Player.Instance.MaxInertiaScale)
            {
                Player.Instance.InertiaScale = Player.Instance.MaxInertiaScale;
            }

            Player.Instance.ShowEngineThrusts();
        }
        else
        {
            Player.Instance.Inertia();
            Player.Instance.HideEngineThrusts();
        }
    }

    private void Rotate()
    {
        float rotation = _inputRotation * Player.Instance.RotationSpeed * -1;

        Player.Instance.Sprite.transform.Rotate(0, 0, rotation * Time.fixedDeltaTime);
    }
}

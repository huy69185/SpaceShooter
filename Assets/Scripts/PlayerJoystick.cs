using UnityEngine;

public class PlayerJoystick : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Joystick _joystick;

    private bool _inputShootPressed;

    private void Update()
    {
        if (_inputShootPressed)
            Player.Instance.InputShootPressed = true;
        else
            Player.Instance.InputShootPressed = false;
    }

    private void FixedUpdate()
    {
        if (Player.Instance.Lifes < 0)
        {
            if (Player.Instance.Rb.velocity != Vector2.zero) Player.Instance.Rb.velocity = Vector2.zero;
            return;
        }

        if (PauseManager.Instance.Paused || !StartManager.Instance.GameStarted) return;

        Rotate();
        Move();
    }

    private void Rotate()
    {
        if (_joystick.Direction == Vector2.zero) return;

        float rotationInDegrees = Mathf.Atan2(_joystick.Direction.y, _joystick.Direction.x) * Mathf.Rad2Deg + 270;
        Quaternion rotationInQuaternion = Quaternion.Euler(0, 0, rotationInDegrees);

        Player.Instance.Sprite.transform.rotation = Quaternion.RotateTowards(Player.Instance.Sprite.transform.rotation, rotationInQuaternion, Player.Instance.RotationSpeed * Time.fixedDeltaTime);
    }

    private void Move()
    {
        if (_joystick.Direction != Vector2.zero)
        {
            if (!Player.Instance.MovedFirstTime)
            {
                Player.Instance.MovedFirstTime = true;
            }

            float rotationInDegrees = Player.Instance.Sprite.transform.eulerAngles.z + 90;
            float rotationInRadians = rotationInDegrees * Mathf.Deg2Rad;
            Player.Instance.Direction = new Vector2(Mathf.Cos(rotationInRadians), Mathf.Sin(rotationInRadians));

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

    public void ShootButton()
    {
        if (!_inputShootPressed)
            _inputShootPressed = true;
        else
            _inputShootPressed = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public event Action<PlayerController> Dead;

    [SerializeField]
    private Rigidbody2D _Rig;
    [SerializeField]
    private float _Speed = 1200, _JumpPower, _HangTimeMax, _GroundCheckRadius;
    [SerializeField]
    private Transform _GroundCheck;
    [SerializeField]
    private LayerMask _GroundLayer;
    [SerializeField]
    private int _Player;
    [SerializeField]
    private TextMeshProUGUI _Score;
    [SerializeField]
    private ShotIndicator _ShotIndicator;
    [SerializeField]
    private Animator _Animator;

    public float Energy => _Energy;

    private Vector3 _180 = new Vector3(0, 180, 0);
    private Vector2 _Move;
    private bool _JumpPressed, _IsGrounded, _HasDoubleJump, _IsAttacking, _IsFalling, _IsChargeing;
    private float _HangTime, _JumpPowerCurrent;

    private string _Horizontal, _Vertical;
    private KeyCode _Jump, _Restart, _Attack, _Fall, _Charge;

    public bool _IsActive = false;
    private Vector3 _StartPos;
    private int _Kills = 0;

    private float _Energy = 0;

    public void Kill()
    {
        _IsActive = false;
        _Kills = 0;
        gameObject.SetActive(false);
        Dead?.Invoke(this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Shot")
        {
            return;
        }

        if(collision.gameObject.name == "Hat")
        {
            return;
        }

        var col = collision.gameObject.GetComponentInParent<PlayerController>();

        if(col == null)
        {
            return;
        }

        if(collision.transform.position.y > transform.position.y)
        {
            Kill();
        }
    }


    public void Respawn()
    {
        if(!gameObject.activeInHierarchy)
        {
            transform.position = _StartPos;
            gameObject.SetActive(true);
            _IsActive = true;
            _Score.SetText($"");
        }
    }

    public void AddKill()
    {
        _Kills++;
        _Score.SetText($"Kills {_Kills}");
    }

    public void Init(int player)
    {
        if(player < 1)
        {
            _Horizontal = "Horizontal";
            _Vertical = "Vertical";
            _Jump = KeyCode.W;
            _Charge = KeyCode.S;
            _Restart = KeyCode.Escape;
            _Attack = KeyCode.Space;
        }
        else
        {
            _Horizontal = "Debug Horizontal";
            _Vertical = "Debug Vertical";
            _Jump = KeyCode.UpArrow;
            _Charge = KeyCode.DownArrow;
            _Restart = KeyCode.KeypadMinus;
            _Attack = KeyCode.Keypad0;
        }

        _StartPos = transform.position;
    }

    private void Start()
    {
        Init(_Player);
    }

    private void Update()
    {
        if(!_IsActive)
        {
            return;
        }

        if(Input.GetKeyDown(_Restart))
        {
            transform.position = _StartPos;
        }

        if (Input.GetKeyUp(_Jump))
        {
            _JumpPressed = false;
        }

        _IsGrounded = Physics2D.OverlapCircle(_GroundCheck.position, _GroundCheckRadius, _GroundLayer);

        if(_IsGrounded)
        {
            _Rig.constraints = RigidbodyConstraints2D.FreezeRotation;
            _Rig.gravityScale = 3;
            _HasDoubleJump = false;
            _IsFalling = false;
            _HangTime = _HangTimeMax;
            _JumpPowerCurrent = _JumpPower;
        }

        if(!_IsGrounded && !_IsAttacking)
        {
            if(_Energy <= 1)
            {
                _Energy += 0.001f;
            }
        }

        _Move = new Vector2(Input.GetAxisRaw(_Horizontal), Input.GetAxisRaw(_Vertical));
        
        if (_IsGrounded && Input.GetKeyDown(_Jump))
        {
            _JumpPressed = true;
        }

        if (!_HasDoubleJump && !_IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {

        }

        if(_Move.x == 0)
        {
            _Animator.SetBool("IsWalking", false);
        }
        else
        {
            _Animator.SetBool("IsWalking", true);
        }

        if(_Move.x > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }else if(_Move.x < 0)
        {
            transform.eulerAngles = _180;
        }

        if(_Rig.velocity.y > 0)
        {
            _Animator.SetBool("IsHovering", true);
        }
        else
        {
            _Animator.SetBool("IsHovering", false);
        }

        if(!_IsGrounded && Input.GetKeyDown(_Attack))
        {
            if (_IsChargeing)
            {
                return;
            }

            _Rig.gravityScale = 0.01f;
            _Rig.velocity = Vector2.zero;
            _Rig.constraints = RigidbodyConstraints2D.FreezePositionX;
            _IsAttacking = true;
            _ShotIndicator.SetActive(true);
        }

        if(_IsAttacking && Input.GetKeyUp(_Attack) || _IsAttacking && _Energy < 0)
        {
            if(_IsChargeing)
            {
                return;
            }

            _IsAttacking = false;
            _Rig.gravityScale = 10;
            _Rig.constraints = RigidbodyConstraints2D.FreezeRotation;
            _ShotIndicator.SetActive(false);
            _ShotIndicator.Shoot();
        }

        if(!_IsAttacking && !_IsGrounded && Input.GetKeyDown(_Fall))
        {
            _Rig.gravityScale = 8;
            _Rig.velocity = Vector2.zero;
            _IsFalling = true;
        }

        if (_IsAttacking && _Energy > 0)
        {
            _Energy -= 0.001f;
            _ShotIndicator.Rotate(Input.GetAxisRaw(_Vertical));
        }

        if (!_JumpPressed && !_IsAttacking && Input.GetKeyDown(_Charge))
        {
            _IsChargeing = true;
            _Rig.gravityScale = 0;
            _Rig.velocity = Vector2.zero;
            _Animator.SetBool("IsCharging", _IsChargeing);
        }

        if(Input.GetKeyUp(_Charge) && _IsChargeing)
        {
            _IsChargeing = false;
            _Rig.gravityScale = 3;
            _Animator.SetBool("IsCharging", _IsChargeing);
        }
        
    }

    private void FixedUpdate()
    {
        if (_IsFalling)
        {
            _IsFalling = false;
            _Rig.constraints = RigidbodyConstraints2D.FreezePositionX;
            _Rig.velocity = new Vector2(_Move.x * _Speed * Time.deltaTime, -Vector2.up.y * _JumpPowerCurrent * 1.3f);
        }
        else
        {
            if(_IsChargeing)
            {
                return;
            }

            _Rig.velocity = new Vector2(_Move.x * _Speed * Time.deltaTime, _Rig.velocity.y);
        }

        if (_IsChargeing)
        {
            _JumpPressed = false;
            return;
        }

        if(_IsAttacking)
        {
            _JumpPressed = false;
            return;
        }

        if (_JumpPressed)
        {
            
            if(_HangTime > 0)
            {
                _HangTime -= Time.deltaTime;
                _Rig.velocity = new Vector2(_Rig.velocity.x, Vector2.up.y * _JumpPowerCurrent);
            }
            else
            {
                _JumpPressed = false;
                _Rig.velocity = new Vector2(_Move.x * _Speed * Time.deltaTime, _Rig.velocity.y);
            }
        }
        else
        {
            _Rig.velocity = new Vector2(_Move.x * _Speed * Time.deltaTime, _Rig.velocity.y);
        }
    }
}

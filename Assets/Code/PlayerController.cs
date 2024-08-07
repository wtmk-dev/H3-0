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
    private Animator _Animator;

    public float Energy => _Energy;

    private Vector3 _180 = new Vector3(0, 180, 0);
    private Vector2 _Move;
    private bool _JumpPressed, _IsGrounded, _IsAttacking;
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
        }
    }

    public void AddKill()
    {
        _Kills++;
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
        Init(0);
        _IsActive = true;
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
        }

        if(!_IsGrounded && !_IsAttacking)
        {
            if(_Energy <= 1)
            {
                _Energy += 0.001f;
            }
        }

        var x = Input.GetAxisRaw(_Horizontal);
        var y = Input.GetAxisRaw(_Vertical);

        if (x != 0)
        {
            _Animator.SetBool("Run", true);
        }
        else
        {
            _Animator.SetBool("Run", false);
        }

        if(y != 0)
        {
            _Animator.SetBool("Jump", true);
        }
        else
        {
            _Animator.SetBool("Jump", false);
        }

        _Move = new Vector2(x, y);
        
        if (_IsGrounded && Input.GetKeyDown(_Jump))
        {
            _JumpPressed = true;
        }

        if(_Move.x > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }else if(_Move.x < 0)
        {
            transform.eulerAngles = _180;
        }

    }

    private void FixedUpdate()
    {
        _Rig.velocity = new Vector2(_Move.x * _Speed * Time.deltaTime, _Rig.velocity.y);

        if (_JumpPressed)
        {
            _Rig.velocity = new Vector2(_Rig.velocity.x, Vector2.up.y * _JumpPower);

            if (_HangTime <= _HangTimeMax)
            {
                _HangTime += Time.deltaTime;
            }
            else
            {
                _HangTime = 0f;
                _JumpPressed = false;
                _Animator.SetBool("Jump", false);
            }
        }
        else
        {
            _Rig.velocity = new Vector2(_Move.x * _Speed * Time.deltaTime, _Rig.velocity.y);
        }
    }
}

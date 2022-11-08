using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotIndicator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _Componets;
    [SerializeField]
    private GameObject _Shot;
    private Vector3 _RotateVector;
    private float _RoationMax = 0.4f, _RotationMin = -0.7f, _CurrentShootingTime = 0.3f;

    private bool _IsShooting;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void Start()
    {
        _RotateVector = new Vector3(0f, 0f, 0f);    
    }

    private void Update()
    {
        if(_IsShooting)
        {
            _CurrentShootingTime -= Time.deltaTime;

            if(_CurrentShootingTime < 0)
            {
                _IsShooting = false;
                _Shot.SetActive(false);
            }
        }
    }

    public void SetActive(bool isActive)
    {
        for(int i = 0; i < _Componets.Count; i++)
        {
            _Componets[i].SetActive(isActive);
        }
    }

    public void Shoot()
    {
        _CurrentShootingTime = 0.1f;
        _Shot.SetActive(true);
        _IsShooting = true;
    }

    public void Rotate(float value)
    {
        _RotateVector.z = value * 1.9f;

        if (transform.localRotation.z < _RotationMin && value < 0)
        {
            return;
        }

        if (transform.localRotation.z > _RoationMax && value > 0)
        {
            return;
        }

        transform.Rotate(_RotateVector);
    }
}

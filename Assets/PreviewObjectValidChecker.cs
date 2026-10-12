using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObjectValidChecker : MonoBehaviour
{
    //Code from:
    //https://youtu.be/CYk1mEcvhqQ?si=3niEYx68QiubQM5c
    public bool IsValid { get; private set; } = true;
    [SerializeField] private List<Collider> _collidingObjects = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("invalidTag"))
        {
            _collidingObjects.Add(other);
            IsValid = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("invalidTag"))
        {
            _collidingObjects.Remove(other);
            IsValid = _collidingObjects.Count <= 0;
        }
    }
}
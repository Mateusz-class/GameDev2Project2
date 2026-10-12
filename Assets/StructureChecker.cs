using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class StructureChecker : MonoBehaviour
{
    [SerializeField] public int Points = 10;
    [SerializeField] public StructureSystem selfstructureSystem;
    [SerializeField] public GameObject SelfTextMeshPro;
    [SerializeField] public TMP_Text SelfText;
    private Camera playerCamera;
    [SerializeField] private List<TMP_Text> _collidingObjects = new List<TMP_Text>();
    public void Start()
    {
        playerCamera = Camera.main;
    }
    public void Update()
    {
        SelfTextMeshPro.transform.LookAt(SelfTextMeshPro.transform.position - playerCamera.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<StructureSystem>(out StructureSystem otherstructureSystem))
        {
            if (other.transform.GetChild(0).TryGetComponent<TMP_Text>(out TMP_Text OtherTextMeshPro))
            {
                _collidingObjects.Add(OtherTextMeshPro);
                CheckStructureType(otherstructureSystem, true, OtherTextMeshPro);
            }
            
        }
        UpdateSelfUI();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<StructureSystem>(out StructureSystem otherstructureSystem))
        {
            if (other.transform.GetChild(0).TryGetComponent<TMP_Text>(out TMP_Text OtherTextMeshPro))
            {
                _collidingObjects.Remove(OtherTextMeshPro);
                CheckStructureType(otherstructureSystem, false, OtherTextMeshPro);
            }

        }
        UpdateSelfUI();
    }

    private void CheckStructureType(StructureSystem otherstructureSystem, bool IsEntering, TMP_Text OtherTextMeshPro)
    {
        switch (selfstructureSystem.structureType)
        {
            case StructureSystem.StructureType.Forest:
                if (otherstructureSystem.structureType == StructureSystem.StructureType.Forest ||
                    otherstructureSystem.structureType == StructureSystem.StructureType.Bandit ||
                    otherstructureSystem.structureType == StructureSystem.StructureType.Town)
                {
                    UpdateStructureInfo(true, IsEntering, OtherTextMeshPro);
                }
                else
                {
                    UpdateStructureInfo(false, IsEntering, OtherTextMeshPro);
                }
                break;
            case StructureSystem.StructureType.Defense:
                if (otherstructureSystem.structureType == StructureSystem.StructureType.Town)
                {
                    UpdateStructureInfo(true, IsEntering, OtherTextMeshPro);
                }
                else
                {
                    UpdateStructureInfo(false, IsEntering, OtherTextMeshPro);
                }
                break;
            case StructureSystem.StructureType.Town:
                if (otherstructureSystem.structureType == StructureSystem.StructureType.Forest ||
                otherstructureSystem.structureType == StructureSystem.StructureType.Town ||
                otherstructureSystem.structureType == StructureSystem.StructureType.Defense)
                {
                    UpdateStructureInfo(true, IsEntering, OtherTextMeshPro);
                }
                else
                {
                    UpdateStructureInfo(false, IsEntering, OtherTextMeshPro);
                }
                break;
            case StructureSystem.StructureType.Graveyard:
                if (otherstructureSystem.structureType == StructureSystem.StructureType.Forest ||
                otherstructureSystem.structureType == StructureSystem.StructureType.Graveyard ||
                otherstructureSystem.structureType == StructureSystem.StructureType.Bandit)
                {
                    UpdateStructureInfo(true, IsEntering, OtherTextMeshPro);
                }
                else
                {
                    UpdateStructureInfo(false, IsEntering, OtherTextMeshPro);
                }
                break;
            case StructureSystem.StructureType.Bandit:
                if (otherstructureSystem.structureType == StructureSystem.StructureType.Forest ||
                otherstructureSystem.structureType == StructureSystem.StructureType.Bandit ||
                otherstructureSystem.structureType == StructureSystem.StructureType.Graveyard)
                {
                    UpdateStructureInfo(true, IsEntering, OtherTextMeshPro);
                }
                else
                {
                    UpdateStructureInfo(false, IsEntering, OtherTextMeshPro);
                }
                break;
            default:
                break;
        }
    }
    private void UpdateStructureInfo(bool SameStructure, bool IsEntering, TMP_Text OtherTextMeshPro)
    {
        if (SameStructure)
        {
            if (IsEntering)
            {
                Points += 10;
                OtherTextMeshPro.text = "+10";
                OtherTextMeshPro.color = Color.green;
            }
            else
            {
                Points -= 10;
                OtherTextMeshPro.text = "";
            }
        }
        else
        {
            if (IsEntering)
            {
                Points -= 30;
                OtherTextMeshPro.text = "-30";
                OtherTextMeshPro.color = Color.red;
            }
            else
            {
                Points += 30;
                OtherTextMeshPro.text = "";
            }

        }
    }
    public void UpdateSelfUI()
    {
        SelfText.text = Points.ToString();
        if(Points > 0)
        {
            SelfText.color = Color.green;
        }
        else
        {
            SelfText.color = Color.red;
        }
    }
    private void OnDestroy()
    {
        foreach(TMP_Text OtherTexts in _collidingObjects)
        {
            OtherTexts.text = "";
        }
    }
}

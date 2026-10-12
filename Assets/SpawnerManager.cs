using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SpawnerManager : MonoBehaviour
{
    //Heavily modified code from:
    //https://youtu.be/CYk1mEcvhqQ?si=3niEYx68QiubQM5c
    [Header("Miscellaneous")]
    [SerializeField] public int TotalPoints;
    [SerializeField] public TMP_Text TotalPointsText;
    [SerializeField] public TMP_Text CurrentStructText;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public GameObject LoseUI;
    [SerializeField] public GameObject WinUI;

    [Header("Placement Parameters")]
    [SerializeField] private Camera playerCamera;
    public bool IsValidTerrain { get; private set; } = true;
    private int CurrentStructureNumber = 0;
    [SerializeField] public List<StructureAsset> StructureAssetList = new List<StructureAsset>();

    [Header("Preview Material")]
    [SerializeField] private Material previewMaterial;
    [SerializeField] private Material previewMaterialLight;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;

    [Header("Raycast Parameters")]
    [SerializeField] private float raycastDistance;
    private GameObject _previewObject = null;
    private GameObject _placedObject = null;
    private Vector3 _currentPlacementPosition = Vector3.zero;
    private bool _inPlacementMode = true;
    private bool _validPreviewState = false;

    private void Start()
    {
        ShuffleList(StructureAssetList);
        CurrentStructureNumber = 0;
        NextStructure();
        UpdateCurrentStructUI();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceStructure();
        }
        else if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            RestartGame();
        }

        if (_inPlacementMode)
        {
            UpdateCurrentPlacementPosition();

            if (CanPlaceObject())
                SetValidPreviewState();
            else
                SetInvalidPreviewState();
        }
    }

    private void UpdateCurrentPlacementPosition()
    {
        if (_previewObject == null)
            return;
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform.gameObject.layer == 7)
            {
                IsValidTerrain = true;
            }
            else
            {
                IsValidTerrain = false;
            }
            _currentPlacementPosition = hit.point;
        }

        _previewObject.transform.position = _currentPlacementPosition;
    }

    private bool CanPlaceObject()
    {
        if (_previewObject == null)
            return false;

        return _previewObject.GetComponentInChildren<PreviewObjectValidChecker>().IsValid && IsValidTerrain;
    }

    private void SetValidPreviewState()
    {
        previewMaterial.color = validColor;
        previewMaterialLight.color = validColor;
        _validPreviewState = true;
    }

    private void SetInvalidPreviewState()
    {
        previewMaterial.color = invalidColor;
        previewMaterialLight.color = invalidColor;
        _validPreviewState = false;
    }

    public void NextStructure()
    {
        if (StructureAssetList.Count == 0)
        return;
        if(CurrentStructureNumber == StructureAssetList.Count)
        {
            _inPlacementMode = false;
            EndGame();
            return;
        }
            
        Quaternion rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);
        _previewObject = Instantiate(StructureAssetList[CurrentStructureNumber].previewObjectPrefab, _currentPlacementPosition, rotation, transform);
        _inPlacementMode = true;
    }
    public void PlaceStructure()
    {
        if (!_inPlacementMode || !_validPreviewState)
            return;
        audioSource.Play();
        Quaternion rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);
        _placedObject = Instantiate(StructureAssetList[CurrentStructureNumber].placeableObjectPrefab, _currentPlacementPosition, rotation, transform);
        _placedObject.transform.GetChild(0).GetChild(0).gameObject.transform.LookAt(
            _placedObject.transform.GetChild(0).GetChild(0).gameObject.transform.position - playerCamera.transform.position);
        if (_previewObject.TryGetComponent<StructureChecker>(out StructureChecker structureChecker))
        {
            TotalPoints += structureChecker.Points;
            UpdateMainPointsUI();
        }
        Destroy(_previewObject);
        CurrentStructureNumber++;
        UpdateCurrentStructUI();
        NextStructure();
    }
    public void UpdateMainPointsUI()
    {
        TotalPointsText.text = $"Points: {TotalPoints.ToString()} / 350";
    }
    public void UpdateCurrentStructUI()
    {
        CurrentStructText.text = $"Struct: {CurrentStructureNumber.ToString()} / {StructureAssetList.Count.ToString()}";
    }
    public void EndGame()
    {
        if(TotalPoints >= 350)
        {
            WinUI.SetActive(true);
        }
        else
        {
            LoseUI.SetActive(true);
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    //Modified ShuffleList code from:
    //https://medium.com/@occasoftware/how-to-shuffle-a-list-in-unity-267940dba432
    public void ShuffleList(List<StructureAsset> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            StructureAsset temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
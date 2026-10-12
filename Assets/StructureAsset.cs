using UnityEngine;

[CreateAssetMenu(fileName = "StructureAssetData", menuName = "Asset/StructureAssetData")]
public class StructureAsset : ScriptableObject
{
    [SerializeField] public GameObject placeableObjectPrefab;
    [SerializeField] public GameObject previewObjectPrefab;
}

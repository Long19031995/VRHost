using UnityEngine;

[CreateAssetMenu(fileName = "HandPose", menuName = "ScriptableObject/HandPose")]
public class HandPose : ScriptableObject
{
    public Vector3[] posOffsets;
    public Quaternion[] rotOffsets;
}

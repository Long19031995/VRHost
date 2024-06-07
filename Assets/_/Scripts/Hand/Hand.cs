using UnityEditor;
using UnityEngine;

public enum HandType
{
    LeftHand,
    RightHand
}

public class Hand : MonoBehaviour
{
    [SerializeField] private HandType type;
    [SerializeField] private Transform root;
    [SerializeField] private Transform[] fingers;
    [SerializeField] private HandPose poseSave;

    private HandPose poseTarget;

    private void Update()
    {
        if (poseTarget != null)
        {
            var i = 0;
            foreach (var finger in fingers)
            {
                var position = Vector3.Lerp(finger.position, root.TransformPoint(poseTarget.posOffsets[i]), Time.deltaTime * 20);
                finger.position = position;

                var rotation = Quaternion.Lerp(finger.rotation, root.rotation * poseTarget.rotOffsets[i], Time.deltaTime * 20);
                finger.rotation = rotation;
                i++;
            }
        }
    }

    public void SetPoseTarget(HandPose pose)
    {
        poseTarget = pose;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Hand))]
public class HandEditor : Editor
{
    private Hand hand;

    private void OnEnable()
    {
        hand = (Hand)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        if (GUILayout.Button("Save Pose", GUILayout.Height(30)))
        {
            var poseSave = serializedObject.FindProperty("poseSave").objectReferenceValue as HandPose;
            var property = serializedObject.FindProperty("fingers");
            var fingers = new Transform[property.arraySize];
            for (int i = 0; i < property.arraySize; i++)
            {
                fingers[i] = property.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
            }
            var root = serializedObject.FindProperty("root").objectReferenceValue as Transform;
            SavePose(poseSave, fingers, root);
        }
    }

    public void SavePose(HandPose poseSave, Transform[] fingers, Transform root)
    {
        poseSave.posOffsets = new Vector3[fingers.Length];
        poseSave.rotOffsets = new Quaternion[fingers.Length];

        var i = 0;
        foreach (var finger in fingers)
        {
            poseSave.posOffsets[i] = root.InverseTransformPoint(finger.position);
            poseSave.rotOffsets[i] = Quaternion.Inverse(root.rotation) * finger.rotation;
            i++;
        }

        EditorUtility.SetDirty(poseSave);
    }

    //public void GetFingerFromHumanoid(Animator animator)
    //{
    //    if (!animator.isHuman) return;

    //    if (type == HandType.LeftHand)
    //    {
    //        GetFingerFromHumanoid(animator, 26, 38);
    //    }
    //    else
    //    {
    //        GetFingerFromHumanoid(animator, 39, 53);
    //    }
    //}

    //private void GetFingerFromHumanoid(Animator animator, int from, int to)
    //{
    //    fingers = new Transform[to - from + 1];

    //    for (int i = from; i <= to; i++)
    //    {
    //        fingers[i - from] = animator.GetBoneTransform((HumanBodyBones)i);
    //    }
    //}
}
#endif

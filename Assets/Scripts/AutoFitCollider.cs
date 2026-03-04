//using UnityEngine;

//[ExecuteAlways]
//public class AutoFitCollider : MonoBehaviour
//{
//    void Start()
//    {
//        FitCollider();
//    }

//    [ContextMenu("Fit Box Collider")]
//    public void FitCollider()
//    {
//        BoxCollider col = GetComponent<BoxCollider>();
//        if (!col)
//            col = gameObject.AddComponent<BoxCollider>();

//        Renderer[] renderers = GetComponentsInChildren<Renderer>();
//        if (renderers.Length == 0) return;

//        Bounds bounds = renderers[0].bounds;
//        foreach (Renderer r in renderers)
//            bounds.Encapsulate(r.bounds);

//        col.center = transform.InverseTransformPoint(bounds.center);
//        col.size = bounds.size;
//    }
//}

using UnityEngine;

[ExecuteAlways]
public class AutoFitCollider : MonoBehaviour
{
    void Start()
    {
        // Tránh tự động chạy ở Edit Mode để không sinh lỗi khi khởi tạo Component
        if (Application.isPlaying)
        {
            FitCollider();
        }
    }

    [ContextMenu("Fit Box Collider")]
    public void FitCollider()
    {
        BoxCollider col = GetComponent<BoxCollider>();

        // Cú pháp chuẩn và an toàn hơn so với "!col"
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        // 1. Gán lại tâm (đổi từ World -> Local space)
        col.center = transform.InverseTransformPoint(bounds.center);

        // 2. SỬA LỖI: Tính toán lại Size cho đúng tỷ lệ Scale (Local space)
        col.size = new Vector3(
            bounds.size.x / transform.lossyScale.x,
            bounds.size.y / transform.lossyScale.y,
            bounds.size.z / transform.lossyScale.z
        );
    }
}
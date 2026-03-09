using UnityEngine;

public class PointTrigger : MonoBehaviour
{
    public bool isPickupPoint; // Tích chọn nếu đây là điểm lấy hàng, bỏ tích nếu là điểm giao
    public DeliveryManager manager;

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem ai chạm vào (Người chơi hoặc Xe máy)
        if (other.CompareTag("Player") || other.GetComponent<Rigidbody>() != null)
        {
            if (isPickupPoint)
            {
                manager.DaDenDiemLayHang();
            }
            else
            {
                manager.DaDenDiemGiaoHang();
            }
        }
    }
}
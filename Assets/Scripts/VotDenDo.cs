using UnityEngine;

public class VotDenDo : MonoBehaviour
{
    public TrafficLightController cotDen; // Cổng cắm dây vào cây đèn

    private void OnTriggerEnter(Collider other)
    {
        // Khi Shipper tông qua vạch (Cube tàng hình)
        if (other.CompareTag("Player") && cotDen != null)
        {
            // Báo tin giật gân cho cây cột đèn xử lý!
            cotDen.XuLyVotDen();
        }
    }
}       
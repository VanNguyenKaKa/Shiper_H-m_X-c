using UnityEngine;

public class AmThanhXeMay : MonoBehaviour
{
    public AudioSource loaXeMay;
    public Rigidbody xeRb;
    public AudioClip tiengDongCo; // Tiếng xe chạy "brum brum"

    void Update()
    {
        // Tính tốc độ hiện tại của xe (dựa vào Rigidbody)
        float tocDo = xeRb.linearVelocity.magnitude;

        if (tocDo > 0.5f) // Xe đang lăn bánh
        {
            if (!loaXeMay.isPlaying)
            {
                loaXeMay.clip = tiengDongCo;
                loaXeMay.Play();
            }
            // Hiệu ứng rồ ga: Chạy càng nhanh tiếng ré càng cao (Pitch)
            loaXeMay.pitch = 1f + (tocDo / 30f);
        }
        else
        {
            // Xe dừng lại thì tắt tiếng (hoặc fen có thể đổi thành tiếng gầm gầm nhẹ nhàn rỗi)
            loaXeMay.Stop();
            loaXeMay.pitch = 1f;
        }
    }
}
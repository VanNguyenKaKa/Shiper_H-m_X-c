using UnityEngine;

public class MuiTenNhay : MonoBehaviour
{
    [Header("Cài đặt Nhảy")]
    public float doCao = 0.5f;     // Mũi tên nảy lên xuống bao nhiêu mét
    public float tocDoNhay = 3f;   // Nảy nhanh hay chậm

    [Header("Cài đặt Xoay")]
    public float tocDoXoay = 90f;  // Tốc độ xoay vòng tròn (để 0 nếu không muốn xoay)

    private Vector3 viTriBanDau;

    void Start()
    {
        // Ghi nhớ vị trí ban đầu lúc game vừa bắt đầu
        viTriBanDau = transform.position;
    }

    void Update()
    {
        // 1. Hiệu ứng nhảy lên xuống (Sử dụng sóng Sin mượt mà)
        float toaDoY_Moi = viTriBanDau.y + Mathf.Sin(Time.time * tocDoNhay) * doCao;
        transform.position = new Vector3(transform.position.x, toaDoY_Moi, transform.position.z);

        // 2. Hiệu ứng xoay tròn quanh trục Y
        transform.Rotate(Vector3.up * tocDoXoay * Time.deltaTime, Space.World);
    }
}
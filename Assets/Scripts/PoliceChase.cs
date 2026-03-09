using UnityEngine;
using UnityEngine.SceneManagement;

public class PoliceChase : MonoBehaviour
{
    [Header("--- MỤC TIÊU ---")]
    public Transform shipper;      // Bấm vào Công an, kéo chiếc xe Shipper thả vào ô này
    public float tocDoChay = 15f;  // Tốc độ chạy (Chỉnh to lên nếu Công an chạy quá chậm)

    void Update()
    {
        if (shipper != null)
        {
            // 1. Luôn xoay mặt nhìn về phía Shipper (Khóa trục Y để không bị ngửa mặt lên trời)
            Vector3 diemNhin = new Vector3(shipper.position.x, transform.position.y, shipper.position.z);
            transform.LookAt(diemNhin);

            // 2. Tự động lao tới tọa độ của Shipper
            transform.position = Vector3.MoveTowards(transform.position, shipper.position, tocDoChay * Time.deltaTime);
        }
    }

    // KHI CÔNG AN CHẠM ĐƯỢC VÀO SHIPPER
    private void OnTriggerEnter(Collider other)
    {
        // Nhớ gắn Tag "Player" cho xe Shipper nhé!
        if (other.CompareTag("Player"))
        {
            Debug.Log("GAME OVER! HẢI QUAY XE KHÔNG KỊP!");
            // Lệnh này sẽ tự động load lại màn chơi từ đầu
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
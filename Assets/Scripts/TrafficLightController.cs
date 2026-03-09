using UnityEngine;
using TMPro; // Bắt buộc phải có cái này để dùng chữ TextMeshPro đếm ngược

public class TrafficLightController : MonoBehaviour
{
    [Header("⏱️ Thời gian đèn (giây)")]
    public float redTime = 5f;
    public float yellowTime = 2f;
    public float greenTime = 5f;

    [Header("💡 3 Bóng đèn (kéo object vào đây)")]
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    [Header("🔢 Màn hình đếm ngược")]
    public TextMeshPro textDemNguoc; // Kéo 3D Text hiển thị giây vào đây

    [Header("⚠️ Vùng nguy hiểm & Hệ thống phạt")]
    public BoxCollider dangerZone;
    public GameObject congAn;              // Kéo mô hình Công an vào đây
    public AudioClip amThanhHaiQuayXe;     // Kéo file MP3 "Hải quay xe" vào đây
    private AudioSource audioSource;

    private LightState currentState = LightState.Red;
    private float timer = 0f;
    private float timeRemaining = 0f; // Biến lưu số giây để hiển thị
    private bool daBiPhat = false;    // Biến chặn lỗi phát âm thanh đè lên nhau nhiều lần

    enum LightState { Red, Yellow, Green }

    void Start()
    {
        // Tự động thêm loa phát thanh vào Cột đèn
        audioSource = gameObject.AddComponent<AudioSource>();

        if (redLight == null || yellowLight == null || greenLight == null)
        {
            Debug.LogError("❌ CHƯA GẮN 3 BÓNG ĐÈN!");
            return;
        }

        if (dangerZone != null)
        {
            dangerZone.isTrigger = true;
        }

        // Tắt công an lúc mới vô game
        if (congAn != null) congAn.SetActive(false);

        // Bắt đầu game với đèn Đỏ
        SetLight(LightState.Red, redTime);
        Debug.Log("🔴 ĐÈN ĐỎ - GAME BẮT ĐẦU!");
    }

    void Update()
    {
        timer += Time.deltaTime;
        timeRemaining -= Time.deltaTime;

        // Cập nhật số giây lên màn hình (Làm tròn lên cho đẹp)
        if (textDemNguoc != null)
        {
            textDemNguoc.text = Mathf.CeilToInt(Mathf.Max(0, timeRemaining)).ToString();
        }

        switch (currentState)
        {
            case LightState.Red:
                if (timer >= redTime)
                {
                    SetLight(LightState.Green, greenTime);
                    Debug.Log("🟢 ĐÈN XANH!");
                }
                break;

            case LightState.Green:
                if (timer >= greenTime)
                {
                    SetLight(LightState.Yellow, yellowTime);
                    Debug.Log("🟡 ĐÈN VÀNG!");
                }
                break;

            case LightState.Yellow:
                if (timer >= yellowTime)
                {
                    SetLight(LightState.Red, redTime);
                    Debug.Log("🔴 ĐÈN ĐỎ!");
                }
                break;
        }
    }

    // Hàm tối ưu việc chuyển đèn và đổi màu chữ đếm ngược
    void SetLight(LightState newState, float duration)
    {
        currentState = newState;
        timer = 0f;
        timeRemaining = duration;
        daBiPhat = false; // Đổi đèn thì reset lại trạng thái bắt phạt

        redLight.SetActive(newState == LightState.Red);
        yellowLight.SetActive(newState == LightState.Yellow);
        greenLight.SetActive(newState == LightState.Green);

        // Đổi màu text đếm ngược cho ngầu
        if (textDemNguoc != null)
        {
            if (newState == LightState.Red) textDemNguoc.color = Color.red;
            else if (newState == LightState.Yellow) textDemNguoc.color = Color.yellow;
            else if (newState == LightState.Green) textDemNguoc.color = Color.green;
        }
    }

    // XỬ LÝ KHI SHIPPER CÁN VẠCH
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentState == LightState.Red && !daBiPhat)
        {
            daBiPhat = true; // Chốt đơn phạt, không cho trigger kêu liên tục
            Debug.LogWarning("🚨 VƯỢT ĐÈN ĐỎ! CÔNG AN RA!");

            // 1. Bật loa "Hải quay xe"
            if (amThanhHaiQuayXe != null)
            {
                audioSource.PlayOneShot(amThanhHaiQuayXe);
            }

            // 2. Thả Công an ra rượt đuổi
            if (congAn != null)
            {
                // Cài vị trí công an hiện ra (Tùy chọn: có thể để công an đứng chờ sẵn ở góc ngã tư)
                congAn.SetActive(true);
            }

            // LƯU Ý: Không dùng Time.timeScale = 0f nữa để game chạy tiếp cho cảnh sát rượt!
        }
    }
}
using UnityEngine;
using TMPro;

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
    public TextMeshPro textDemNguoc;

    [Header("⚠️ Vùng nguy hiểm & Hệ thống phạt")]
    public BoxCollider dangerZone;
    public GameObject congAn;
    public AudioClip amThanhHaiQuayXe;
    private AudioSource audioSource;

    private LightState currentState = LightState.Red;
    private float timer = 0f;
    private float timeRemaining = 0f;
    private bool daBiPhat = false;

    enum LightState { Red, Yellow, Green }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // THÊM 2 DÒNG NÀY: Reset lệnh truy nã mỗi khi chơi lại ván mới
        PoliceChase.coNguoiDaBatDuoc = false;
        PoliceChase.dangBiTruyNa = false;

        if (redLight == null || yellowLight == null || greenLight == null)
        {
            Debug.LogError("❌ CHƯA GẮN 3 BÓNG ĐÈN!");
            return;
        }

        if (dangerZone != null)
        {
            dangerZone.isTrigger = true;
        }

        if (congAn != null) congAn.SetActive(false);

        SetLight(LightState.Red, redTime);
    }

    void Update()
    {
        timer += Time.deltaTime;
        timeRemaining -= Time.deltaTime;

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
                }
                break;

            case LightState.Green:
                if (timer >= greenTime)
                {
                    SetLight(LightState.Yellow, yellowTime);
                }
                break;

            case LightState.Yellow:
                if (timer >= yellowTime)
                {
                    SetLight(LightState.Red, redTime);
                }
                break;
        }
    }

    void SetLight(LightState newState, float duration)
    {
        currentState = newState;
        timer = 0f;
        timeRemaining = duration;
        daBiPhat = false;

        redLight.SetActive(newState == LightState.Red);
        yellowLight.SetActive(newState == LightState.Yellow);
        greenLight.SetActive(newState == LightState.Green);

        if (textDemNguoc != null)
        {
            if (newState == LightState.Red) textDemNguoc.color = Color.red;
            else if (newState == LightState.Yellow) textDemNguoc.color = Color.yellow;
            else if (newState == LightState.Green) textDemNguoc.color = Color.green;
        }
    }

    // --- ĐÃ ĐỔI THÀNH HÀM PUBLIC ĐỂ CÁI CUBE GỌI ĐẾN ---
    public void XuLyVotDen()
    {
        if (currentState == LightState.Red && !daBiPhat)
        {
            daBiPhat = true;
            Debug.LogWarning("🚨 VƯỢT ĐÈN ĐỎ! CÔNG AN RA!");

            // 1. Bật loa "Hải quay xe"
            if (amThanhHaiQuayXe != null)
            {
                audioSource.PlayOneShot(amThanhHaiQuayXe);
            }

            // 2. Thả Công an ẩn ra (Nếu có)
            if (congAn != null)
            {
                congAn.SetActive(true);
            }

            // 3. KÍCH HOẠT LỆNH TRUY NÃ TOÀN CẦU CHO TẤT CẢ CÔNG AN
            PoliceChase.dangBiTruyNa = true;
        }
    }
}
using UnityEngine;
using TMPro;
using System.Collections;

public class DeliveryManager : MonoBehaviour
{
    [Header("--- Giao Diện Cố Định ---")]
    public TextMeshProUGUI missionText;
    public TextMeshProUGUI moneyText;

    [Header("--- Phụ Đề (Bong Bóng Chat) ---")]
    public GameObject dialogueFrame;
    public TextMeshProUGUI dialogueText;
    public Transform player;
    public float chieuCaoTrenDau = 1.8f;
    public float lechSangPhai = 0.8f;

    [Header("--- Âm Thanh (Voice & Hiệu ứng) ---")]
    public AudioClip soundTing;         // Tiếng tiền (Dùng chung)
    private AudioSource audioSource;

    [Header("--- Dữ Liệu Giao Nhận ---")]
    public GameObject[] pickupPoints;
    public GameObject[] deliveryPoints;
    public string[] pickupMessages;
    public string[] deliveryMessages;
    public GameObject[] handItems;

    [Header("--- KHÁCH HÀNG NPC ---")]
    public Animator[] khachHangAnimators;
    public GameObject[] khachHangHandItems;
    public AudioClip[] voiceKhachGoi;    // Tiếng khách gọi từ xa
    public float khoangCachChao = 15.0f;
    private bool daChaoKhach = false;

    [Header("--- KỊCH BẢN THOẠI CHO TỪNG ĐƠN ---")]
    public string[] thoaiShipperLucNhan;
    public string[] thoaiChuQuan;
    public string[] thoaiShipperLucGiao;
    public string[] thoaiKhachHang;

    [Header("--- LỒNG TIẾNG (VOICE) CHO TỪNG ĐƠN ---")]
    public AudioClip[] voiceShipperLucNhan; // Tiếng Shipper đọc đơn
    public AudioClip[] voiceChuQuan;        // Tiếng Chủ quán trả lời
    public AudioClip[] voiceShipperLucGiao; // Tiếng Shipper gọi khách
    public AudioClip[] voiceKhachHang;      // Tiếng Khách cảm ơn

    [Header("--- HOẠT ẢNH (ANIMATION) TỪNG ĐƠN ---")]
    public Animator shipperAnimator;
    public string[] animLayHang;
    public string[] animGiaoHang;
    public float thoiGianDienAnim = 2.0f;

    [Header("--- Xe Mới (Phần Thưởng) ---")]
    public GameObject xeSH;
    private bool daMoKhoaSH = false;

    private int tongTien = 0;
    private int currentOrderIndex = 0;
    private Transform nguoiDangNoi;

    enum DeliveryState { DangCho, DiLayHang, DiGiaoHang }
    private DeliveryState currentState = DeliveryState.DangCho;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        dialogueFrame.SetActive(false);
        foreach (GameObject item in handItems) { if (item != null) item.SetActive(false); }
        foreach (GameObject item in khachHangHandItems) { if (item != null) item.SetActive(false); }
        foreach (GameObject p in pickupPoints) { if (p != null) p.SetActive(false); }
        foreach (GameObject d in deliveryPoints) { if (d != null) d.SetActive(false); }
        if (xeSH != null) xeSH.SetActive(false);

        UpdateMoneyUI();
        Invoke("PhatDonHangMoi", 2f);
    }

    void Update()
    {
        if (dialogueFrame.activeSelf && nguoiDangNoi != null && Camera.main != null)
        {
            Vector3 viTriDich = nguoiDangNoi.position + Vector3.up * chieuCaoTrenDau + Camera.main.transform.right * lechSangPhai;
            Vector3 toaDoManHinh = Camera.main.WorldToScreenPoint(viTriDich);

            if (toaDoManHinh.z > 0)
            {
                dialogueFrame.transform.position = toaDoManHinh;
            }
        }

        if (currentState == DeliveryState.DiGiaoHang && player != null)
        {
            if (deliveryPoints.Length > currentOrderIndex && deliveryPoints[currentOrderIndex] != null)
            {
                float khoangCach = Vector3.Distance(player.position, deliveryPoints[currentOrderIndex].transform.position);

                if (khoangCach <= khoangCachChao && !daChaoKhach)
                {
                    daChaoKhach = true;

                    if (khachHangAnimators.Length > currentOrderIndex && khachHangAnimators[currentOrderIndex] != null)
                    {
                        khachHangAnimators[currentOrderIndex].SetTrigger("KhachGoi");
                    }

                    if (voiceKhachGoi.Length > currentOrderIndex && voiceKhachGoi[currentOrderIndex] != null)
                    {
                        audioSource.PlayOneShot(voiceKhachGoi[currentOrderIndex]);
                    }
                }
            }
        }
    }

    void PhatDonHangMoi()
    {
        if (currentOrderIndex >= pickupPoints.Length) currentOrderIndex = 0;
        daChaoKhach = false;

        currentState = DeliveryState.DiLayHang;
        missionText.text = pickupMessages.Length > currentOrderIndex ? pickupMessages[currentOrderIndex] : "Đang chờ đơn hàng mới...";
        pickupPoints[currentOrderIndex].SetActive(true);
    }

    public void DaDenDiemLayHang()
    {
        if (currentState == DeliveryState.DiLayHang)
        {
            StartCoroutine(KichBanLayHang());
        }
    }

    IEnumerator KichBanLayHang()
    {
        currentState = DeliveryState.DangCho;
        missionText.text = "Đang giao tiếp...";

        string cauShipper = (thoaiShipperLucNhan.Length > currentOrderIndex && !string.IsNullOrEmpty(thoaiShipperLucNhan[currentOrderIndex])) ? thoaiShipperLucNhan[currentOrderIndex] : "Em nhận đơn ạ.";
        string cauShop = (thoaiChuQuan.Length > currentOrderIndex && !string.IsNullOrEmpty(thoaiChuQuan[currentOrderIndex])) ? thoaiChuQuan[currentOrderIndex] : "Oke em, đơn của em đây.";

        // 1. Shipper nói & Phát tiếng
        nguoiDangNoi = player;
        dialogueFrame.SetActive(true);
        dialogueText.text = "<color=#00FF00><b>Shipper:</b></color>\n" + cauShipper;
        if (voiceShipperLucNhan.Length > currentOrderIndex && voiceShipperLucNhan[currentOrderIndex] != null)
            audioSource.PlayOneShot(voiceShipperLucNhan[currentOrderIndex]);
        yield return new WaitForSeconds(2.0f);

        // 2. Chạy Anim lấy hàng
        dialogueFrame.SetActive(false);
        if (shipperAnimator != null && animLayHang.Length > currentOrderIndex && !string.IsNullOrEmpty(animLayHang[currentOrderIndex]))
        {
            shipperAnimator.SetTrigger(animLayHang[currentOrderIndex]);
            yield return new WaitForSeconds(thoiGianDienAnim);
        }

        if (handItems.Length > currentOrderIndex && handItems[currentOrderIndex] != null)
            handItems[currentOrderIndex].SetActive(true);

        // 3. Chủ quán nói & Phát tiếng
        nguoiDangNoi = pickupPoints[currentOrderIndex].transform;
        dialogueFrame.SetActive(true);
        dialogueText.text = "<color=#FFFF00><b>Chủ quán:</b></color>\n" + cauShop;
        if (voiceChuQuan.Length > currentOrderIndex && voiceChuQuan[currentOrderIndex] != null)
            audioSource.PlayOneShot(voiceChuQuan[currentOrderIndex]);
        yield return new WaitForSeconds(2.5f);

        dialogueFrame.SetActive(false);
        currentState = DeliveryState.DiGiaoHang;
        missionText.text = deliveryMessages.Length > currentOrderIndex ? deliveryMessages[currentOrderIndex] : "Giao hàng tới điểm đích!";

        pickupPoints[currentOrderIndex].SetActive(false);
        deliveryPoints[currentOrderIndex].SetActive(true);
    }

    public void DaDenDiemGiaoHang()
    {
        if (currentState == DeliveryState.DiGiaoHang)
        {
            StartCoroutine(KichBanGiaoHang());
        }
    }

    IEnumerator KichBanGiaoHang()
    {
        currentState = DeliveryState.DangCho;
        missionText.text = "Đang giao tiếp...";

        string cauShipper = (thoaiShipperLucGiao.Length > currentOrderIndex && !string.IsNullOrEmpty(thoaiShipperLucGiao[currentOrderIndex])) ? thoaiShipperLucGiao[currentOrderIndex] : "Dạ, em giao hàng ạ.";
        string cauKhach = (thoaiKhachHang.Length > currentOrderIndex && !string.IsNullOrEmpty(thoaiKhachHang[currentOrderIndex])) ? thoaiKhachHang[currentOrderIndex] : "Chị cảm ơn em nhé!";

        // 1. Shipper nói & Phát tiếng
        nguoiDangNoi = player;
        dialogueFrame.SetActive(true);
        dialogueText.text = "<color=#00FF00><b>Shipper:</b></color>\n" + cauShipper;
        if (voiceShipperLucGiao.Length > currentOrderIndex && voiceShipperLucGiao[currentOrderIndex] != null)
            audioSource.PlayOneShot(voiceShipperLucGiao[currentOrderIndex]);
        yield return new WaitForSeconds(2.0f);

        // 2. Shipper diễn Anim đưa đồ
        dialogueFrame.SetActive(false);
        if (shipperAnimator != null && animGiaoHang.Length > currentOrderIndex && !string.IsNullOrEmpty(animGiaoHang[currentOrderIndex]))
        {
            shipperAnimator.SetTrigger(animGiaoHang[currentOrderIndex]);
            yield return new WaitForSeconds(thoiGianDienAnim);
        }

        // 3. Chuyền đồ, Khách nhận đồ
        if (handItems.Length > currentOrderIndex && handItems[currentOrderIndex] != null)
            handItems[currentOrderIndex].SetActive(false);

        if (khachHangHandItems.Length > currentOrderIndex && khachHangHandItems[currentOrderIndex] != null)
            khachHangHandItems[currentOrderIndex].SetActive(true);

        if (khachHangAnimators.Length > currentOrderIndex && khachHangAnimators[currentOrderIndex] != null)
            khachHangAnimators[currentOrderIndex].SetTrigger("KhachNhan");

        // 4. Khách hàng cảm ơn & Phát tiếng
        nguoiDangNoi = deliveryPoints[currentOrderIndex].transform;
        dialogueFrame.SetActive(true);
        dialogueText.text = "<color=#00FFFF><b>Khách hàng:</b></color>\n" + cauKhach;
        if (voiceKhachHang.Length > currentOrderIndex && voiceKhachHang[currentOrderIndex] != null)
            audioSource.PlayOneShot(voiceKhachHang[currentOrderIndex]);
        yield return new WaitForSeconds(2.5f);
        dialogueFrame.SetActive(false);

        // 5. Cộng tiền + Kêu Ting
        if (soundTing != null) audioSource.PlayOneShot(soundTing);
        tongTien += 50;
        UpdateMoneyUI();
        missionText.text = "Giao thành công! +50 VNĐ";

        yield return new WaitForSeconds(2.0f);

        deliveryPoints[currentOrderIndex].SetActive(false);
        currentOrderIndex++;

        if (tongTien >= 300 && !daMoKhoaSH)
            Invoke("MoKhoaXeMoi", 1f);
        else
            Invoke("PhatDonHangMoi", 2f);
    }

    void MoKhoaXeMoi()
    {
        daMoKhoaSH = true;
        missionText.text = "🎉 MỞ KHÓA XE SH350i! 🎉";
        if (xeSH != null) xeSH.SetActive(true);
        Invoke("PhatDonHangMoi", 5f);
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = "$ " + tongTien.ToString("N0") + "";
    }
}
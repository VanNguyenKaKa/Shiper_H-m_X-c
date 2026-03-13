using UnityEngine;
using TMPro;
using System.Collections;

public class CuaHangXeMay : MonoBehaviour
{
    [Header("--- MƯỢN KHUNG Ô-VAN CÓ SẴN ---")]
    public GameObject khungOvan;
    public TextMeshProUGUI textOvan;

    [Header("--- BẢNG CHỨA 2 NÚT BẤM ---")]
    public GameObject bangChonMua;

    [Header("--- ÂM THANH LỒNG TIẾNG ---")]
    public AudioSource loaThoai;
    public AudioClip voiceHoiXe;
    public AudioClip voiceCoChu;
    public AudioClip voiceHoiGia;
    public AudioClip voiceGia150;
    public AudioClip voiceChot;
    public AudioClip voiceGiaoXe;
    public AudioClip voiceThieuTien;

    [Header("--- CÀI ĐẶT MUA XE ---")]
    public int giaXe = 150;
    public GameObject xeShPrefab;
    public Transform viTriGiaoXe;


    void Start()
    {
        bangChonMua.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            khungOvan.SetActive(true);
            StartCoroutine(KichBanNoiChuyen());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) DongCuaHang();
    }

    // --- HÀM PHÁT THOẠI CẢI TIẾN (TỰ ĐỘNG NHẢY TRÁI/PHẢI) ---
    IEnumerator PhatThoai(string cauNoi, AudioClip fileGiongNoi)
    {
        khungOvan.SetActive(true);
        textOvan.text = cauNoi;

        // BÍ KÍP ĐÂY: Lấy cái khung và đẩy nó sang 2 bên
        RectTransform rectOvan = khungOvan.GetComponent<RectTransform>();
        if (rectOvan != null)
        {
            if (cauNoi.StartsWith("Shipper"))
            {
                // Shipper nói -> Nằm bên TRÁI
                rectOvan.anchoredPosition = new Vector2(-180f, rectOvan.anchoredPosition.y);
            }
            else
            {
                // Ông chủ nói -> Nằm bên PHẢI
                rectOvan.anchoredPosition = new Vector2(180f, rectOvan.anchoredPosition.y);
            }
        }

        if (fileGiongNoi != null && loaThoai != null)
        {
            loaThoai.clip = fileGiongNoi;
            loaThoai.Play();
            yield return new WaitForSeconds(fileGiongNoi.length);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
    }

    // --- KỊCH BẢN CHÍNH ---
    IEnumerator KichBanNoiChuyen()
    {
        bangChonMua.SetActive(false);

        yield return StartCoroutine(PhatThoai("Shipper: Ông chủ, có bán xe SH350i không?", voiceHoiXe));
        yield return StartCoroutine(PhatThoai("Ông Chủ: Có chứ, SH350i hàng mới về luôn!", voiceCoChu));
        yield return StartCoroutine(PhatThoai("Shipper: Giá bao nhiêu vậy?", voiceHoiGia));
        yield return StartCoroutine(PhatThoai("Ông Chủ: 150 củ nhé cháu trai.", voiceGia150));
        yield return StartCoroutine(PhatThoai("Shipper: Chốt luôn!", voiceChot));

        // Trả khung thoại về chính giữa màn hình trước khi hiện 2 nút
        RectTransform rectOvan = khungOvan.GetComponent<RectTransform>();
        if (rectOvan != null) rectOvan.anchoredPosition = new Vector2(0f, rectOvan.anchoredPosition.y);

        textOvan.text = "Chốt luôn con SH350i với giá " + giaXe + "$ luôn nhỉ?";
        // MỞ KHÓA CHUỘT ĐỂ BẤM NÚT
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        bangChonMua.SetActive(true);
    }

    // --- NÚT BẤM ---
    public void NhanNutChot()
    {
        bangChonMua.SetActive(false);

        // GỌI SANG KHO BẠC ĐỂ QUẸT THẺ (Tự động trừ tiền và cập nhật số trên UI)
        if (DeliveryManager.instance != null && DeliveryManager.instance.KiemTraVaTruTien(giaXe))
        {
            // Mua thành công
            StartCoroutine(XuLyMuaThanhCong());
        }
        else
        {
            // Nghèo, mua thất bại
            StartCoroutine(XuLyMuaThatBai());
        }
    }

    IEnumerator XuLyMuaThanhCong()
    {
        yield return StartCoroutine(PhatThoai("Ông Chủ: Chúc mừng cháu! SH của cháu ở ngoài cửa kìa.", voiceGiaoXe));
        if (xeShPrefab != null && viTriGiaoXe != null) Instantiate(xeShPrefab, viTriGiaoXe.position, viTriGiaoXe.rotation);
        DongCuaHang();
    }

    IEnumerator XuLyMuaThatBai()
    {
        yield return StartCoroutine(PhatThoai("Ông Chủ: Tài khoản của bạn không đủ tiền rồi! Cày tiếp đi cháu.", voiceThieuTien));
        DongCuaHang();
    }

    public void NhanNutBoDi()
    {
        DongCuaHang();
    }

    void DongCuaHang()
    {
        khungOvan.SetActive(false);
        bangChonMua.SetActive(false);
        if (loaThoai != null) loaThoai.Stop();
        StopAllCoroutines();

        // NHỐT CHUỘT LẠI ĐỂ XOAY CAMERA
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
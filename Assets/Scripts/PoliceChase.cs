using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PoliceChase : MonoBehaviour
{
    [Header("--- CÔNG AN ---")]
    public Animator congAnAnim;
    public float tocDoChay = 15f;
    // Đã xóa khoangCachPhatHien! Có bao nhiêu rượt bấy nhiêu!

    [Header("--- NHÂN VẬT (SHIPPER) ---")]
    public Transform nhanVat;
    public Animator nhanVatAnim;
    public Rigidbody nhanVatRb;
    public MonoBehaviour scriptDiBo;

    [Header("--- XE MÁY (MOTORBIKE) ---")]
    public Transform xe;
    public Rigidbody xeRb;
    public MonoBehaviour scriptLaiXe;

    [Header("--- ÂM THANH KỊCH BẢN ---")]
    public AudioClip voiceBoMayLaAi;
    public AudioClip soundBop1;
    public AudioClip soundBop2;
    public AudioClip voiceLaoToet;
    private AudioSource audioSource;

    [Header("--- ĐỒNG BỘ THỜI GIAN (CHỈNH ĐỂ KHỚP) ---")]
    public float thoiGianShipperNoi = 2.0f;
    public float thoiGianVungTayDanh1 = 0.4f;
    public float thoiGianNghiGiua2Dam = 0.5f;
    public float thoiGianVungTayDanh2 = 0.4f;
    public float thoiGianCongAnChui = 2.0f;

    // --- BIẾN QUẢN LÝ ---
    private bool daBatDuoc = false;
    private bool dangLaiXe = false;
    private bool daBatAnimationChay = false;

    // BIẾN TOÀN CẦU: Dùng chung cho 100 ông
    public static bool coNguoiDaBatDuoc = false;
    public static bool dangBiTruyNa = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        // ĐÃ XÓA 2 DÒNG RESET Ở ĐÂY ĐỂ ỔNG KHÔNG BỊ MẤT TRÍ NHỚ NỮA!
    }

    void Update()
    {
        // 1. Nếu có 1 ông tóm được fen rồi -> Mấy ông khác đứng dòm, không chạy nữa
        if (coNguoiDaBatDuoc && !daBatDuoc)
        {
            if (congAnAnim != null) congAnAnim.SetTrigger("CongAnDungLai");
            return;
        }

        // 2. CHẾ ĐỘ 5 SAO: Cứ bị truy nã là TẤT CẢ lao vào rượt!
        if (dangBiTruyNa && nhanVat != null && !daBatDuoc)
        {
            // Bật dáng chạy 1 lần
            if (!daBatAnimationChay && congAnAnim != null)
            {
                congAnAnim.SetTrigger("CongAnChay");
                daBatAnimationChay = true;
            }

            // Ép vị trí chạy lao tới Shipper
            Vector3 diemNhin = new Vector3(nhanVat.position.x, transform.position.y, nhanVat.position.z);
            transform.LookAt(diemNhin);
            transform.position = Vector3.MoveTowards(transform.position, diemNhin, tocDoChay * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !daBatDuoc && !coNguoiDaBatDuoc)
        {
            coNguoiDaBatDuoc = true;
            daBatDuoc = true;
            if (xe != null && nhanVat.IsChildOf(xe)) { dangLaiXe = true; } else { dangLaiXe = false; }
            StartCoroutine(KichBanTauHai());
        }
    }

    IEnumerator KichBanTauHai()
    {
        if (scriptLaiXe != null) scriptLaiXe.enabled = false;
        if (scriptDiBo != null) scriptDiBo.enabled = false;

        if (nhanVat != null)
        {
            CharacterController cc = nhanVat.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
        }

        if (xeRb != null) { xeRb.linearVelocity = Vector3.zero; xeRb.angularVelocity = Vector3.zero; }
        if (nhanVatRb != null) { nhanVatRb.linearVelocity = Vector3.zero; nhanVatRb.angularVelocity = Vector3.zero; }

        if (nhanVat != null)
        {
            Vector3 nhinNhanVat = new Vector3(nhanVat.position.x, transform.position.y, nhanVat.position.z);
            transform.LookAt(nhinNhanVat);
            Vector3 nhinCongAn = new Vector3(transform.position.x, nhanVat.position.y, transform.position.z);
            nhanVat.LookAt(nhinCongAn);
        }

        if (congAnAnim != null) congAnAnim.SetTrigger("CongAnDungLai");

        if (nhanVatAnim != null)
        {
            nhanVatAnim.SetFloat("InputX", 0f);
            nhanVatAnim.SetFloat("InputZ", 0f);
            nhanVatAnim.SetTrigger("ShipperBoMayLaAi");
        }
        if (voiceBoMayLaAi != null) audioSource.PlayOneShot(voiceBoMayLaAi);
        yield return new WaitForSeconds(thoiGianShipperNoi);

        if (congAnAnim != null) congAnAnim.SetTrigger("CongAnDanh");
        yield return new WaitForSeconds(thoiGianVungTayDanh1);
        if (soundBop1 != null) audioSource.PlayOneShot(soundBop1);

        yield return new WaitForSeconds(thoiGianNghiGiua2Dam);

        if (congAnAnim != null) congAnAnim.SetTrigger("CongAnDanh2");
        yield return new WaitForSeconds(thoiGianVungTayDanh2);
        if (soundBop2 != null) audioSource.PlayOneShot(soundBop2);

        if (nhanVatAnim != null) nhanVatAnim.SetTrigger("ShipperTe");

        if (nhanVat != null)
        {
            if (dangLaiXe)
            {
                nhanVat.SetParent(null);
                if (xeRb != null)
                {
                    Vector3 huongDayXe = (xe.position - transform.position).normalized;
                    xeRb.AddForce(huongDayXe * 150f, ForceMode.Impulse);
                }
            }
            if (nhanVatRb != null) nhanVatRb.isKinematic = true;
        }

        yield return new WaitForSeconds(1.5f);

        if (congAnAnim != null) congAnAnim.SetTrigger("CongAnLaoToet");
        if (voiceLaoToet != null) audioSource.PlayOneShot(voiceLaoToet);

        yield return new WaitForSeconds(thoiGianCongAnChui);

        Debug.Log("GAME OVER!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Bắt buộc phải có để dùng IENumerator (Kịch bản thời gian)

public class PoliceChase : MonoBehaviour
{
    [Header("--- MỤC TIÊU ---")]
    public Transform shipper;
    public float tocDoChay = 15f;

    [Header("--- HOẠT ẢNH (ANIMATOR) ---")]
    public Animator congAnAnim;    // Kéo chú Công an vào đây
    public Animator shipperAnim;   // Kéo nhân vật Shipper vào đây

    [Header("--- ÂM THANH KỊCH BẢN ---")]
    public AudioClip voiceBoMayLaAi; // Tiếng: "Mày biết bố mày là ai không?"
    public AudioClip soundBop;       // Tiếng: "Bốp!"
    public AudioClip voiceLaoToet;   // Tiếng: "Láo toét!"
    private AudioSource audioSource;

    private bool daBatDuoc = false;  // Biến ghi nhớ để không bị chạy kịch bản 2 lần

    void Start()
    {
        // Tự động thêm loa phát thanh cho Công an
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Nếu chưa bắt được thì cứ nhắm thẳng Shipper mà rượt
        if (shipper != null && !daBatDuoc)
        {
            Vector3 diemNhin = new Vector3(shipper.position.x, transform.position.y, shipper.position.z);
            transform.LookAt(diemNhin);
            transform.position = Vector3.MoveTowards(transform.position, shipper.position, tocDoChay * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !daBatDuoc)
        {
            daBatDuoc = true; // Đánh dấu là đã tóm được, NGỪNG RƯỢT ĐUỔI!

            // Bắt đầu chạy đoạn phim cắt cảnh
            StartCoroutine(KichBanTauHai(other.gameObject));
        }
    }

    IEnumerator KichBanTauHai(GameObject xeShipper)
    {
        // 1. ĐÓNG BĂNG XE SHIPPER LẠI (Không cho chạy trốn nữa)
        Rigidbody rbXe = xeShipper.GetComponentInParent<Rigidbody>();
        if (rbXe != null) rbXe.isKinematic = true; // Khóa cứng vật lý của xe

        // Xoay mặt Công an nhìn thẳng vào Shipper
        Vector3 nhinShipper = new Vector3(shipper.position.x, transform.position.y, shipper.position.z);
        transform.LookAt(nhinShipper);

        // 2. SHIPPER CHỈ TAY VÀ NÓI
        if (shipperAnim != null) shipperAnim.SetTrigger("ShipperBoMayLaAi");
        if (voiceBoMayLaAi != null) audioSource.PlayOneShot(voiceBoMayLaAi);

        // Đợi 2.5 giây cho Shipper nói xong (Bạn có thể chỉnh số này cho khớp file ghi âm)
        yield return new WaitForSeconds(2.5f);

        // 3. CÔNG AN VUNG TAY ĐÁNH
        if (congAnAnim != null) congAnAnim.SetTrigger("CongAnDanh");

        // Đợi 0.4 giây cho cái tay vung trúng mặt rồi mới phát tiếng "BỐP"
        yield return new WaitForSeconds(0.4f);
        if (soundBop != null) audioSource.PlayOneShot(soundBop);

        // Đợi 1 giây để thu tay về
        yield return new WaitForSeconds(1.0f);

        // 4. CÔNG AN CHỈ TAY VÀ CHỬI
        if (congAnAnim != null) congAnAnim.SetTrigger("CongAnLaoToet");
        if (voiceLaoToet != null) audioSource.PlayOneShot(voiceLaoToet);

        // Đợi 2 giây cho Công an nói xong
        yield return new WaitForSeconds(2.0f);

        // 5. GAME OVER VÀ CHƠI LẠI
        Debug.Log("GAME OVER! MÀN TẤU HÀI KẾT THÚC!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
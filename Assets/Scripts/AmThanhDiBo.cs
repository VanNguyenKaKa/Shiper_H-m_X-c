using UnityEngine;

public class AmThanhDiBo : MonoBehaviour
{
    public AudioSource loaCuaShipper;
    public AudioClip tiengDiBo;
    public AudioClip tiengChayBo;

    [Header("Nút bấm để chạy (Ví dụ: LeftShift)")]
    public KeyCode nutChay = KeyCode.LeftShift;

    void Update()
    {
        // Kiểm tra xem có đang bấm phím di chuyển không (W A S D)
        float diChuyen = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));

        if (diChuyen > 0.1f)
        {
            // Nếu có bấm phím Shift -> Đang chạy. Không bấm -> Đi bộ
            bool dangChay = Input.GetKey(nutChay);
            AudioClip tiengCanPhat = dangChay ? tiengChayBo : tiengDiBo;

            // Nếu Loa đang phát sai tiếng hoặc đang tắt -> Bật lên
            if (loaCuaShipper.clip != tiengCanPhat || !loaCuaShipper.isPlaying)
            {
                loaCuaShipper.clip = tiengCanPhat;
                loaCuaShipper.Play();
            }
        }
        else
        {
            // Đứng im thì tắt loa
            loaCuaShipper.Stop();
        }
    }
}
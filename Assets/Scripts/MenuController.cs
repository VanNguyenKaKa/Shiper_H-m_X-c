using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có dòng này để chuyển Scene

public class MenuController : MonoBehaviour
{
    [Header("Kéo cái Panel Bảng Hướng Dẫn vào đây")]
    public GameObject bangHuongDan;

    public void NhanNutPlay()
    {
        // Ghi đúng Y CHANG tên Scene map game của fen vào đây
        // Tui soi mấy ảnh trước thấy fen đặt tên nó là "MapSence"
        SceneManager.LoadScene("MapSence");
    }

    public void MoHuongDan()
    {
        if (bangHuongDan != null) bangHuongDan.SetActive(true); // Hiện bảng lên
    }

    public void DongHuongDan()
    {
        if (bangHuongDan != null) bangHuongDan.SetActive(false); // Giấu bảng đi
    }
}
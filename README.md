# Chat GPT - UIT
*Get instant answers, find creative inspiration, learn something new.*

![Logo Chat GPT - UIT](./ChatGPT/Assets/uitai.png)


## Mục lục
[I. Mở đầu](#Modau)

[II. Mô tả](#Mota)

> [1. Ý tưởng](#Ytuong)
>
> [2. Công nghệ](#Congnghe)
>
> [3. Đối tượng sử dụng](#Doituongsudung)
>
> [4. Mục tiêu](#Muctieu)
>
> [5. Tính năng](#Tinhnang)
>
> [6. Hướng dẫn sử dụng](#Huongdansudung)

[III. Tác giả](#Tacgia)

[IV. Người hướng dẫn](#Nguoihuongdan)

[V. Tổng kết](#Tongket)


<!-- MỞ ĐẦU -->
<div id="Modau"></div>

## I. Mở đầu
Trong thời đại công nghệ phát triển mạnh mẽ, nhu cầu học ngoại ngữ, đặc biệt là tiếng Anh ngày càng gia tăng. Để đáp ứng nhu cầu này, nhóm đã quyết định tạo ra một ứng dụng hỗ trợ học tiếng Anh, dựa trên API của ChatGPT. Ứng dụng này sẽ tận dụng công nghệ AI tiên tiến để cung cấp một trải nghiệm học tập cá nhân hóa và hiệu quả, giúp người học cải thiện kỹ năng ngôn ngữ của mình một cách toàn diện.


<!-- MÔ TẢ -->
<div id="Mota"></div>

## II. Mô tả

<!-- Ý TƯỞNG -->
<div id="Ytuong"></div>

### 1. Ý tưởng
* Để nâng cao trải nghiệm người dùng, nhóm sử dụng công nghệ WPF cùng ngôn ngữ XAML mang lại một giao diện người dùng mới, hiện đại và dễ sử dụng, đảm bảo rằng ngôn ngữ lập trình dễ tiếp cận và thân thiện với người dùng, giúp việc thiết kế và điều chỉnh GUI trở nên dễ dàng hơn.

* Ứng dụng này cũng được xây dựng trên mô hình MVVM, cho phép phân chia rõ ràng giữa giao diện người dùng và xử lý logic, tăng cường khả năng tái sử dụng và linh hoạt trong việc cập nhật giao diện mà không cần thay đổi quá nhiều trong mã nguồn, đẩy nhanh tốc độ phát triển và thuận tiện trong việc bảo trì cũng như nâng cấp.

* Áp dụng SQL Database kết hợp với LiteDatabase (NoSQL), một hệ thống quản lý cơ sở dữ liệu mạnh mẽ, giúp việc đồng bộ hóa dữ liệu trên nhiều thiết bị của người dùng, đảm bảo dữ liệu luôn nhất quán và chính xác.


<div id="Congnghe"></div>

### 2. Công nghệ
* Hệ thống API: WPF - Mô hình MVVM, API ChatGPT, API Google Translate
* IDE: Visual Studio 2022
* Database: SQL Server, LiteDatabase (NoSQL)
* Công cụ quản lý: Git, GitHub


<div id="Doituongsudung"></div>

### 3. Đối tượng sử dụng
* Người muốn học tiếng anh (thi chứng chỉ tiếng Anh, nâng cao kĩ năng ngôn ngữ, làm việc ở nước ngoài, ...)
* Dành cho mọi đối tượng muốn sử dụng Chat GPT như một công cụ học tập, làm việc


<div id="Muctieu"></div>

### 4. Mục tiêu

 * <strong>Ứng dụng thực tế</strong>
 
    * Ứng dụng này được thiết kế để đáp ứng nhu cầu học tiếng Anh, với sự ổn định cao và giao diện thân thiện, dễ sử dụng. Được phát triển để thay thế cho các ứng dụng cũ và các phương pháp truyền thống, ứng dụng này giúp giảm thiểu rắc rối và sai sót trong quá trình học, trở thành lựa chọn ưa thích cho người học tiếng Anh.


 * <strong>Yêu cầu ứng dụng</strong>
 
    * Ứng dụng cần tích hợp các tính năng tiêu chuẩn của một công cụ học tiếng Anh, đồng thời mở rộng với các tính năng mới, hỗ trợ tối đa cho người dùng trong việc tự học và quản lý quá trình học của mình. Tính năng tự động hóa và cải thiện quy trình học sẽ giúp người dùng tiết kiệm thời gian và nâng cao hiệu quả.
    
    * Ứng dụng phải đảm bảo tính chính xác và an toàn trong việc quản lý dữ liệu cá nhân, cung cấp các báo cáo và thông tin cập nhật một cách nhanh chóng và chính xác.
    
    * Dễ dàng tra cứu và tìm kiếm thông tin liên quan đến tiến trình học, nguồn tài liệu, và lịch sử học tập của người dùng.
    
    * Cải thiện và lập kế hoạch học tập phải dễ dàng, chính xác để đáp ứng nhu cầu học tập.
  
    * Ứng dụng phải tương thích với nhiều hệ điều hành khác nhau và hoạt động ổn định, tránh gây lỗi hay xung đột trong quá trình sử dụng. Việc mở rộng và nâng cấp sau này cũng cần được đảm bảo dễ dàng thực hiện.


<div id="Tinhnang"></div>

### 5. Tính năng
* Với đối tượng muốn học tiếng Anh:
   * Khi bật chế độ `Advance`, Chat GPT sẽ trở thành một trợ lí đắc lực cho người dùng, phục vụ cho việc học tiếng Anh, giải đáp các thắc mắc, đưa ra những bài tập với đa dạng *chủ đề, band* điểm chuẩn trong chứng chỉ quốc tế IELTS với bốn kĩ năng cần thiết (nghe, nói, đọc, viết) và hỗ trợ dịch từ ngữ *Anh-Việt* ở mọi nơi trên thiết bị.
   * Khi bôi đen một đoạn văn bản ở bất kì nơi đâu, hệ thống sẽ có hai `option` cho người dùng lựa chọn:
      * `trans` - Dịch đoạn văn bản được chọn `selected text` (mọi ngôn ngữ) sang tiếng Việt
      * `spk` - Đọc đoạn văn bản được chọn bằng tiếng Anh 
* Với đối tượng muốn sử dụng cho học tập, công việc:
   * Khi tắt chế độ "Advance", Chat GPT sẽ trả lời các câu hỏi về đa dạng chủ đề khác nhau như sửa lỗi trong lập trình, sáng tạo làm thơ, soạn nhạc,... phục vụ tối đa nhu cầu của người dùng.
<div id="Huongdansudung"></div>

### 6. Hướng dẫn sử dụng
* **Bước 1**: Clone repository về thiết bị
   ```
   git clone https://github.com/bichsonnhat/ChatAI.git
   ```
* **Bước 2**: Mở file `LoginChat` trong `SQL Server`, lần lượt thực hiện `execute` các câu truy vấn trong file. 
* **Bước 3**: Open project trong Visual Studio 2022, vào `Tools` -> `Connect to Database`, nhập *server name* trong `SQL Server` và lựa chọn `database name` là `LoginChat`
* **Bước 4**: Vào phần `LoginWindow.xaml.cs`, thay đổi `NhatBS` thành *sever name* của thiết bị trong SQL Server
* **Bước 5**: Chạy và màn hình `login` hiện lên, nhập key = `admin` (hoặc các key đã thực thi trong `SQL Server` trước đó) để sử dụng ứng dụng.
<!-- TÁC GIẢ -->
<div id="Tacgia"></div>

## III. Tác giả

* [Bích Sơn Nhật](https://github.com/bichsonnhat)

* [Lê Anh Khôi](https://github.com/AKaLee-IK27)

* [Vũ Hoàng Quân](https://github.com/vhq3404)


<!-- NGƯỜI HƯỚNG DẪN -->
<div id="Nguoihuongdan"></div>

## IV. Người hướng dẫn
* Giảng viên: Nguyễn Tấn Toàn


<!-- TỔNG KẾT -->
<div id="Tongket"></div>

## V. Tổng kết
Sản phẩm này là thành quả của quá trình làm việc chăm chỉ và hợp tác của các thành viên trong nhóm. Trải qua quá trình này, các thành viên trong nhóm đã tích lũy được nhiều kiến thức và kỹ năng chuyên môn về lập trình thực tế, nâng cao hiểu biết về lập trình và rút ra những bài học quý giá cho sự phát triển trong tương lai.

Nhóm cũng xin gửi lời cảm ơn chân thành và sâu sắc tới giảng viên hướng dẫn, thầy Nguyễn Tấn Toàn, đã đồng hành và hỗ trợ nhóm trong suốt quá trình thực hiện đồ án, giúp đạt được kết quả như ngày hôm nay.

---

<p align="right"><a href="#Top">Quay lại đầu trang</a></p>
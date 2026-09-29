# TSHOP – Hướng dẫn cài đặt

Xây dựng website bán quần áo thời trang được xây dựng bằng ASP.NET MVC và sử dụng Microsoft SQL Server để lưu trữ dữ liệu.

## 1. Yêu cầu môi trường

- Windows
- Visual Studio có hỗ trợ ASP.NET MVC
- .NET Framework / .NET phù hợp với phiên bản của project
- Microsoft SQL Server
- SQL Server Management Studio (SSMS)
- Trình duyệt web: Google Chrome, Microsoft Edge hoặc Firefox

## 2. Cấu trúc bộ source

```text
📁 NguyenHongThien_DH52004489
│
├── 📁 SourceCode
│   ├── 📁 WebBanHang
│   └── 📁 NhanVienGiaoHang
│
├── 📁 Database
│   ├── 🗄️ Tshop.bak
│   └── 🗄️ Tshop.sql
│
└── 📄 README.md
```

- `SourceCode`: mã nguồn website TSHOP.
- `Database/Tshop.bak`: file backup database, dùng để Restore nhanh.
- `Database/Tshop.sql`: script SQL dự phòng, có thể dùng để tạo lại database.
- `Readme`: tài liệu hướng dẫn cài đặt chi tiết.

## 3. Khôi phục database

### Cách 1 – Restore file `.bak` (khuyến nghị)

1. Mở SQL Server Management Studio (SSMS).
2. Kết nối đến SQL Server.
3. Chuột phải `Databases` → `Restore Database...`.
4. Chọn `Device`.
5. Nhấn `...` → `Add`.
6. Chọn file `Tshop.bak`.
7. Kiểm tra tên database là `Tshop`.
8. Nhấn `OK` để thực hiện Restore.
9. Kiểm tra `Databases → Tshop` và `Tables`.

### Cách 2 – Chạy file `.sql`

Nếu không sử dụng file `.bak`:

1. Mở SSMS.
2. Chọn `New Query`.
3. Mở file `Tshop.sql`.
4. Kiểm tra script tạo database `Tshop`.
5. Nhấn `Execute (F5)`.
6. Kiểm tra database `Tshop` sau khi chạy xong.

> Chỉ cần sử dụng **một trong hai cách** trên. Không cần Restore `.bak` và chạy `.sql` trên cùng một database.

## 4. Cấu hình kết nối database

Mở source code bằng Visual Studio và tìm file cấu hình kết nối database, thường là `appsettings.json` hoặc file cấu hình tương ứng của project.

Ví dụ:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=Tshop;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Các thông tin cần kiểm tra:

- `Server`: tên SQL Server trên máy.
- `Database`: phải là `Tshop`.
- `Trusted_Connection=True`: sử dụng Windows Authentication.
- `TrustServerCertificate=True`: hỗ trợ kết nối trong môi trường phát triển.

Nếu SQL Server sử dụng instance khác, thay `Server` cho phù hợp, ví dụ:

```text
Server=.\SQLEXPRESS;
```

> Không sao chép nguyên connection string trên nếu tên SQL Server của máy khác. Chỉ cần thay phần `Server` cho đúng với môi trường cài đặt.

## 5. Mở và chạy project

1. Mở Visual Studio.
2. Chọn `Open a project or solution`.
3. Mở file `TSHOP.sln` trong thư mục `SourceCode`.
4. Chờ Visual Studio tải project và các package cần thiết.
5. Chọn đúng Startup Project.
6. Chọn `Build → Build Solution` để kiểm tra project.
7. Nhấn `F5` hoặc `Ctrl + F5` để chạy website.

Nếu website hiển thị trang chủ và dữ liệu sản phẩm được tải từ database thì hệ thống đã được cài đặt thành công.

## 6. Kiểm tra hệ thống sau khi chạy

Có thể kiểm tra nhanh các chức năng:

- Trang chủ.
- Danh sách sản phẩm.
- Chi tiết sản phẩm.
- Đăng ký / đăng nhập.
- Giỏ hàng.
- Đặt hàng.
- Khuyến mãi.
- Quản lý đơn hàng.
- Quản lý khách hàng.
- Quản lý sản phẩm.
- Đổi trả hàng.

## 7. Tài khoản kiểm thử

Điền tài khoản thực tế của project vào bảng dưới đây trước khi bàn giao:

| Vai trò | Tài khoản | Mật khẩu |
|---|---|---|
| Admin | `0123456789` | `123456` |

## 8. Các lỗi thường gặp

### Không kết nối được database

Kiểm tra:

- SQL Server đang chạy.
- Database `Tshop` đã được Restore.
- Tên `Server` trong connection string chính xác.
- Phương thức xác thực SQL Server phù hợp.

### Không hiển thị dữ liệu

Kiểm tra:

- Database có dữ liệu hay chưa.
- Connection string có trỏ đến `Tshop` hay không.
- Các bảng cần thiết có tồn tại hay không.
- Project có kết nối đúng `DbContext` hay không.

### Project không Build được

Thực hiện:

```text
Build → Rebuild Solution
```

Sau đó kiểm tra cửa sổ `Error List` để xác định lỗi.

## 9. Quy trình cài đặt nhanh

```text
Restore Tshop.bak
        ↓
Mở WebBanHang.sln
        ↓
Cấu hình Connection String
        ↓
Build Solution
        ↓
Run Project
        ↓
Đăng nhập tài khoản kiểm thử
        ↓
Kiểm tra hệ thống
```

## 10. Lưu ý

- Không cần Restore `.bak` và chạy `.sql` đồng thời.
- Tên database cần thống nhất là `Tshop` nếu connection string đang sử dụng tên này.
- Connection string cần được điều chỉnh theo SQL Server trên máy cài đặt.
- File `.bak` là phương án khôi phục database được khuyến nghị.
- File `.sql` được cung cấp như phương án dự phòng và để kiểm tra/cập nhật cấu trúc dữ liệu.

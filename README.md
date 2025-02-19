# 🚀 Phân Tích Dự Án Game SpaceShooter

## 1️⃣ Giới Thiệu Dự Án
**SpaceShooter** là một trò chơi arcade 2D, nơi người chơi điều khiển phi thuyền để bắn thiên thạch, thu thập sao và nâng cao điểm số. Người chơi cần tránh va chạm với thiên thạch để không bị mất máu và có thể sử dụng các vật phẩm hỗ trợ như túi cứu thương và túi đạn để tăng sức mạnh.

## 2️⃣ Mục Tiêu Dự Án
- Xây dựng một game arcade đơn giản nhưng thú vị.
- Phát triển kỹ năng lập trình C# trong Unity.
- Cải thiện khả năng xử lý va chạm và vật lý trong game.
- Áp dụng hiệu ứng hình ảnh và âm thanh để tăng trải nghiệm người chơi.

## 3️⃣ Phân Tích Các Thành Phần Chính

### 🎮 3.1. Hệ Thống Điều Khiển
- **Di chuyển**: Dùng các phím **Mũi tên (Arrow Keys)** để điều khiển phi thuyền.
- **Bắn**: Nhấn **Space** để bắn đạn hoặc laser.
- **Tương tác với vật phẩm**: Khi phi thuyền chạm vào các vật phẩm đặc biệt, chúng sẽ có hiệu ứng tương ứng.

### 🚀 3.2. Cơ Chế Game
- **Thiên thạch xuất hiện ngẫu nhiên** và rơi từ trên xuống.
- **Thu thập sao** để tăng điểm.
- **Túi cứu thương** giúp hồi máu.
- **Túi đạn** giúp nâng cấp vũ khí thành laser mạnh hơn.
- **Điểm cao nhất** sẽ hiển thị khi vào game.
- **Mất máu khi va chạm thiên thạch**, hết 3 máu thì thua.

### 💥 3.3. Xử Lý Va Chạm
- **Sử dụng Collider2D** để kiểm tra va chạm giữa các vật thể.
- **OnTriggerEnter2D()** để xác định khi phi thuyền va chạm với thiên thạch hoặc thu thập vật phẩm.
- **Giảm máu khi va chạm** với thiên thạch.
- **Cập nhật UI máu và điểm số** khi có thay đổi trạng thái.

### 🏆 3.4. Hệ Thống Điểm Số và Game Over
- **Điểm số được cập nhật liên tục** khi bắn thiên thạch hoặc thu thập sao.
- **Hiển thị điểm cao nhất** mỗi lần bắt đầu game.
- **Màn hình Game Over** khi hết máu.

### 🎨 3.5. Hình Ảnh và Hiệu Ứng
- **Sprite 2D** được sử dụng để tạo các đối tượng trong game.
- **Hiệu ứng nổ khi va chạm** với thiên thạch.
- **Hiệu ứng bắn laser và đạn thường**.
- **Hiệu ứng thu thập vật phẩm** giúp làm nổi bật trải nghiệm chơi.

### 🔊 3.6. Âm Thanh
- **Âm thanh bắn súng** khi phi thuyền bắn.
- **Âm thanh va chạm** khi trúng thiên thạch.
- **Âm thanh thu thập sao hoặc vật phẩm**.
- **Nhạc nền nhẹ nhàng** giúp tăng trải nghiệm chơi game.

## 4️⃣ Quy Trình Phát Triển
### 📌 4.1. Cài Đặt Môi Trường
- Cài đặt **Unity Hub** và Unity Editor.
- Sử dụng **Visual Studio** hoặc **Rider** để lập trình C#.
- Cấu hình project theo template **2D**.

### 🏗️ 4.2. Xây Dựng Game
- **Tạo các đối tượng game (Player, Asteroids, Stars, Items)**.
- **Viết script điều khiển phi thuyền (PlayerController.cs)**.
- **Xử lý cơ chế bắn súng và va chạm**.
- **Tạo hệ thống UI điểm số và máu**.
- **Thiết lập hiệu ứng và âm thanh**.

### 🛠️ 4.3. Kiểm Tra và Cải Thiện
- **Tối ưu hóa hiệu suất** bằng cách giảm tải tài nguyên không cần thiết.
- **Kiểm tra va chạm, tốc độ game, điều chỉnh độ khó**.
- **Cân bằng gameplay** để tạo trải nghiệm hấp dẫn.




// ============================================================
// FILE: auth.js
// CHỨC NĂNG: Quản lý logic Đăng nhập, Đăng ký, Lưu Token
// ============================================================

const API_BASE_URL = '/api/auth'; // Đường dẫn gốc đến API Auth

// --- 1. HÀM ĐĂNG NHẬP ---
async function login() {
    // Lấy giá trị từ ô nhập liệu trong login.html
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    // Kiểm tra sơ bộ: Nếu trống thì báo lỗi
    if (!email || !password) {
        alert("Vui lòng nhập Email và Mật khẩu!");
        return;
    }

    try {
        // Gọi API Login lên Server
        const response = await fetch(`${API_BASE_URL}/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json' // Báo cho server biết mình gửi JSON
            },
            body: JSON.stringify({ email, password }) // Đóng gói dữ liệu gửi đi
        });

        // Nếu đăng nhập thành công (Server trả về 200 OK)
        if (response.ok) {
            const data = await response.json(); // Giải mã dữ liệu JSON trả về

            // QUAN TRỌNG: Lưu Token và Email vào bộ nhớ trình duyệt (LocalStorage)
            // Để các trang khác (như Dashboard) có thể dùng lại
            localStorage.setItem('authToken', data.token);
            localStorage.setItem('userEmail', data.email);

            // Chuyển hướng ngay lập tức sang trang chủ (Dashboard)
            window.location.href = 'index.html';
        } else {
            // Nếu thất bại (401 Unauthorized), hiển thị thông báo lỗi
            alert("Đăng nhập thất bại! Kiểm tra lại email hoặc mật khẩu.");
        }
    } catch (error) {
        // Lỗi kết nối (mất mạng, server sập...)
        console.error("Lỗi:", error);
        alert("Không thể kết nối đến Server.");
    }
}

// --- 2. HÀM ĐĂNG KÝ ---
async function register() {
    // Lấy dữ liệu từ form đăng ký
    const email = document.getElementById('reg-email').value;
    const password = document.getElementById('reg-password').value;
    const confirmPassword = document.getElementById('reg-confirm').value;

    // Kiểm tra dữ liệu nhập vào
    if (!email || !password) return alert("Vui lòng nhập đủ thông tin!");
    if (password !== confirmPassword) return alert("Mật khẩu xác nhận không khớp!");

    try {
        // Gọi API Register lên Server
        const response = await fetch(`${API_BASE_URL}/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        const data = await response.json();

        if (response.ok) {
            // Đăng ký thành công -> Báo user và chuyển về trang Login
            alert("Đăng ký thành công! Hãy đăng nhập ngay.");
            window.location.href = 'login.html';
        } else {
            // Đăng ký thất bại (ví dụ: Email trùng)
            alert("Lỗi: " + (data.message || "Đăng ký thất bại"));
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối Server.");
    }
}

// --- 3. HÀM ĐĂNG XUẤT ---
function logout() {
    // Xóa Token khỏi bộ nhớ -> Coi như hết phiên đăng nhập
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');

    // Đá văng về trang Login
    window.location.href = 'login.html';
}

// --- 4. HÀM KIỂM TRA ĐĂNG NHẬP (Dùng ở trang Dashboard) ---
function checkAuth() {
    const token = localStorage.getItem('authToken');

    // Nếu không tìm thấy Token -> Chưa đăng nhập -> Đá về Login
    if (!token) {
        window.location.href = 'login.html';
        return null; // Dừng hàm, trả về null
    }

    return token; // Trả về token để dùng gọi API
}
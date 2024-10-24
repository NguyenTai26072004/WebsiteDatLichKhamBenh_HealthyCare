function validateForm() {
    // Lấy các giá trị từ form
    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();
    const confirmPassword = document.getElementById('confirmPassword').value.trim();
    const email = document.getElementById('email').value.trim();
    const phone = document.getElementById('phone').value.trim();
    const birthDate = document.getElementById('birthDate').value;
    const gender = document.querySelector('input[name="gender"]:checked');
    const errorMessage = [];

    // Kiểm tra tên đăng nhập
    if (username.length < 8) {
        errorMessage.push('Tên đăng nhập phải tối thiểu 8 ký tự.');
    }

    // Kiểm tra mật khẩu
    const passwordPattern = /^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[!@#$%^&*])/; // Có ít nhất 1 chữ, 1 số và 1 ký tự đặc biệt
    if (password.length < 8) {
        errorMessage.push('Mật khẩu phải tối thiểu 8 ký tự.');
    } else if (!passwordPattern.test(password)) {
        errorMessage.push('Mật khẩu phải chứa ít nhất 1 ký tự chữ, 1 số và 1 ký tự đặc biệt.');
    } else if (password !== confirmPassword) {
        errorMessage.push('Mật khẩu không khớp.');
    }

    // Kiểm tra định dạng email
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email)) {
        errorMessage.push('Email không đúng định dạng.');
    }

    // Kiểm tra số điện thoại
    const phonePattern = /^\d{10,15}$/; // Kiểm tra số điện thoại phải là từ 10 đến 15 chữ số
    if (!phonePattern.test(phone)) {
        errorMessage.push('Số điện thoại phải là từ 10 đến 15 số.');
    }

    // Kiểm tra ngày sinh
    if (!birthDate) {
        errorMessage.push('Vui lòng chọn ngày sinh.');
    }

    // Kiểm tra giới tính
    if (!gender) {
        errorMessage.push('Vui lòng chọn giới tính.');
    }

    // Hiển thị thông báo lỗi
    const formError = document.getElementById('formError');
    if (errorMessage.length > 0) {
        formError.innerHTML = errorMessage.join('<br>');
        return false; // Ngăn không cho gửi form
    }

    formError.innerHTML = ''; // Reset thông báo lỗi
    return true; // Cho phép gửi form
}

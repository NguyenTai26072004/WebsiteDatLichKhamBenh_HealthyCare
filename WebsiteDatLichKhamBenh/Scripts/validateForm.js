function validateForm(event) {
    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();
    const confirmPassword = document.getElementById('confirmPassword').value.trim();
    const email = document.getElementById('email').value.trim();
    const phone = document.getElementById('phone').value.trim();
    const birthDate = document.getElementById('birthDate').value;
    const gender = document.querySelector('input[name="gender"]:checked');

    // Xóa các lỗi cũ
    document.getElementById('usernameError').textContent = '';
    document.getElementById('passwordError').textContent = '';
    document.getElementById('confirmPasswordError').textContent = '';
    document.getElementById('emailError').textContent = '';
    document.getElementById('phoneError').textContent = '';
    document.getElementById('birthDateError').textContent = '';
    document.getElementById('genderError').textContent = '';

    let isValid = true;

    // Kiểm tra tên đăng nhập
    if (!username) {
        document.getElementById('usernameError').textContent = 'Tên đăng nhập không được để trống.';
        isValid = false;
    } else if (username.length < 8) {
        document.getElementById('usernameError').textContent = 'Tên đăng nhập phải tối thiểu 8 ký tự.';
        isValid = false;
    }

    // Kiểm tra mật khẩu
    if (!password) {
        document.getElementById('passwordError').textContent = 'Mật khẩu không được để trống.';
        isValid = false;
    } else if (password.length < 8) {
        document.getElementById('passwordError').textContent = 'Mật khẩu phải tối thiểu 8 ký tự.';
        isValid = false;
    }

    if (!confirmPassword) {
        document.getElementById('confirmPasswordError').textContent = 'Vui lòng xác nhận mật khẩu.';
        isValid = false;
    } else if (password !== confirmPassword) {
        document.getElementById('confirmPasswordError').textContent = 'Mật khẩu không khớp.';
        isValid = false;
    }

    // Kiểm tra định dạng email
    if (!email) {
        document.getElementById('emailError').textContent = 'Email không được để trống.';
        isValid = false;
    } else {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(email)) {
            document.getElementById('emailError').textContent = 'Email không đúng định dạng.';
            isValid = false;
        }
    }

    // Kiểm tra số điện thoại
    if (!phone) {
        document.getElementById('phoneError').textContent = 'Số điện thoại không được để trống.';
        isValid = false;
    } else {
        const phonePattern = /^\d{10,15}$/;
        if (!phonePattern.test(phone)) {
            document.getElementById('phoneError').textContent = 'Số điện thoại phải là từ 10 đến 15 số.';
            isValid = false;
        }
    }

    // Kiểm tra ngày sinh
    if (!birthDate) {
        document.getElementById('birthDateError').textContent = 'Vui lòng chọn ngày sinh.';
        isValid = false;
    }

    // Kiểm tra giới tính
    if (!gender) {
        document.getElementById('genderError').textContent = 'Vui lòng chọn giới tính.';
        isValid = false;
    }

    // Ngăn gửi form nếu có lỗi
    if (!isValid) {
        event.preventDefault();
    }

    return isValid; // Trả về isValid để kiểm tra cuối cùng
}

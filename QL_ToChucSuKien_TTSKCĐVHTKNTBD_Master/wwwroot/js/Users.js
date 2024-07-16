
function openModal(url) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            $('#createUserModalContent').html(response);
            $('#createUserModal').modal('show');
        },
        error: function () {
            alert('Có lỗi xảy ra khi tải form.');
        }
    });
}

function ajaxPost(url, data) {
    $.ajax({
        url: url,
        type: 'POST', // Chắc chắn rằng bạn đang gửi phương thức POST
        data: data,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.success) {
                alert(result.message);
                $('#createUserModal').modal('hide');
                window.location.reload();
            } else {
                if (result.errors && result.errors.length > 0) {
                    var errors = result.errors.join('<br>');
                    alert('Đã xảy ra lỗi:<br>' + errors);
                } else {
                    console.log('Dữ liệu lỗi không xác định:', result);
                    alert('Đã xảy ra lỗi không xác định.');
                }
            }
        },

        error: function () {
            alert('Đã xảy ra lỗi khi gửi yêu cầu.');
        }
    });
}


function openModal(url) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            $('#createEventModalContent').html(response);
            $('#createEventModal').modal('show');
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
                $('#createEventModal').modal('hide');
                window.location.reload();
            } else {
                var errors = result.errors.join('<br>');
                alert('Đã xảy ra lỗi:<br>' + errors);
            }
        },
        error: function () {
            alert('Đã xảy ra lỗi khi gửi yêu cầu.');
        }
    });
}

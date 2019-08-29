$(function () {
    if (window.location.indexOf('/recurring') !== -1) {
        alert('recurring!');
    }

    $('<form id="jumpForm" action="/quick-jump">' +
        '<div class= "input-group form-group">' +
          '<input name="jobid" type="text" class="form-control" placeholder="Quick Jump by Job Id...">' +
          '<span class="input-group-btn">' +
            '<button id="searchBtn" class="btn btn-default" type="submit">' +
              '<span class="glyphicon glyphicon-share-alt"></span>' +
            '</button>' +
          '</span>' +
        '</div>' +
      '</form>').insertBefore('.page-header');

    $('#jumpForm').submit(function (event) {
        event.preventDefault();

        var url = $(this).attr('action');
        var data = $(this).serialize();

        $('#jumpForm input').attr('disabled', 'disabled');
        $('#jumpForm button').attr('disabled', 'disabled').html('<span class="spinner"></span>');

        $.ajax({
                type: 'POST',
                url: url,
                data: data,
                dataType: 'json'
            })
            .done(function (data) {
                if (data.location && data.location[0] === '/') {
                    window.location.href = data.location;
                }
            })
            .fail(function (data) {
                $('#jumpForm .input-group').addClass('has-warning');
                $('#jumpForm input[name="jobid"]').attr('placeholder', 'Not found').val(null);

                setTimeout(function () {
                    $('#jumpForm .input-group').removeClass('has-warning');
                    $('#jumpForm input[name="jobid"]').attr('placeholder', 'Quick Jump by Job Id...');
                }, 2500);
            })
            .always(function () {
                $('#jumpForm input').removeAttr('disabled');
                $('#jumpForm button').removeAttr('disabled').html('<span class="glyphicon glyphicon-share-alt"></span>');
            });
    });
});

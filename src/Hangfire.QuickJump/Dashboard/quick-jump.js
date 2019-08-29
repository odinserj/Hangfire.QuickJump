$(function () {
    if (window.location.href.indexOf('/recurring') !== -1) {
        $('.js-jobs-list table tr td:nth-child(2)').each(function () {
            var jobId = $(this).text();
            $(this).html('<a href="' + Hangfire.config.pathBase + '/recurring/details/' + encodeURI(jobId) + '/">' + jobId + '</a>');
        });
    }

    $(this).on('click', '.recurring-job-command', function (e) {
        var $this = $(this);
        var confirmText = $this.data('confirm');
        var jobId = $this.data('id');

        if (!confirmText || confirm(confirmText)) {
            $this.prop('disabled');
            var loadingDelay = setTimeout(function () {
                $this.button('loading');
            }, 100);

            $.post($this.data('url'), { 'jobs[]': jobId }, function () {
                clearTimeout(loadingDelay);
                window.location.reload();
            });
        }

        e.preventDefault();
    });

    $('<form id="jumpForm" action="' + Hangfire.config.pathBase + '/quick-jump">' +
        '<div class= "input-group form-group">' +
          '<input name="jobid" type="text" class="form-control" placeholder="Quick Jump by Job, Recurring Job or Batch Id...">' +
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
                    window.location.href = encodeURI(data.location);
                }
            })
            .fail(function (data) {
                $('#jumpForm .input-group').addClass('has-warning');
                $('#jumpForm input[name="jobid"]').attr('placeholder', 'Not found').val(null);

                setTimeout(function () {
                    $('#jumpForm .input-group').removeClass('has-warning');
                    $('#jumpForm input[name="jobid"]').attr('placeholder', 'Quick Jump by Job, Recurring Job or Batch Id...');
                }, 2500);
            })
            .always(function () {
                $('#jumpForm input').removeAttr('disabled');
                $('#jumpForm button').removeAttr('disabled').html('<span class="glyphicon glyphicon-share-alt"></span>');
            });
    });
});

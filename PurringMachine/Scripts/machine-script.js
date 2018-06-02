$(document).ready(function () {
    $('.square').each(function () {
        $(this).height($(this).width());
    });
});

$('#run').on('click', function () {
    $.get({
        url: '/Home/RunMachine',
        success: function (retval) {
            console.log(retval.Data);
        }
    });
});

$('#reset').on('click', function () {
    $.get({
        url: '/Home/ResetMachine',
        success: function (data, textStatus, xhr) {
            console.log("success: " + xhr.status);
        }
    });
});

$('#nextStep').on('click', function () {
    $.get({
        url: '/Home/NextStep',
        success: function (data, textStatus, xhr) {
            console.log("success: " + xhr.status);
        }
    });
});
$(document).ready(function () {
    $('.square').each(function () {
        $(this).height($(this).width());
    });
});
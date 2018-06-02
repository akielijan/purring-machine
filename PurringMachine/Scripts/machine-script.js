$(document).ready(function () {
    $('#instructions-table').editableTableWidget();

    $('table td').on('validate', function (evt, newValue) {
        var statePattern = /^[a-zA-Z0-9]*$/;
        var alphabetPattern = /^[a-zA-Z0-9#]$/;
        var instructionPattern = /^[a-zA-Z0-9#]\s(\S)+\s(L|R|N)$/;

        if ($(this).hasClass("state")) {
            if (!statePattern.test(newValue)) {
                $(this).addClass("invalid");
            }
            else {
                $(this).removeClass("invalid");
            }
        }
        else if ($(this).hasClass("alphabet")) {
            if (!alphabetPattern.test(newValue)) {
                $(this).addClass("invalid");
            }
            else {
                $(this).removeClass("invalid");
            }
        }
        else {
            if (!instructionPattern.test(newValue)) {
                $(this).addClass("invalid");
            }
            else {
                $(this).removeClass("invalid");
            }
        }
    });

    $('.square').each(function () {
        $(this).height($(this).width());
    });

    $("#addRowButton").on("click", function () {
        var table = document.getElementById("instructions-table");
        var cols = table.rows[0].cells.length;
        var rows = table.rows.length;

        var newRow = table.insertRow(rows);

        for (var i = 0; i < cols; i++) {
            var newCell = newRow.insertCell(0);
            newCell.innerHTML = "-";
        }

        $('#instructions-table').editableTableWidget();
    });

    $("#addColButton").on("click", function () {
        var table = document.getElementById("instructions-table");
        var cols = table.rows[0].cells.length;
        var rows = table.rows.length;

        for (var i = 0; i < rows; i++) {
            var newCell = table.rows[i].insertCell(cols);
            newCell.innerHTML = "-";
        }

        $('#instructions-table').editableTableWidget();
    });
});

$('#run').on('click', function () {
    $.get({
        url: '/Home/RunMachine',
        success: function (data, textStatus, xhr) {
            console.log(data.Data);
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
            console.log(data.Data);
            //if (!data.Success) -> do smth when machine has stopped
        }
    });
});

$('#saveSettings').on('click', function () {
    var instructionList = parseTable('#instructions-table');
    $.post({
        url: '/Home/SaveSettings',
        data: {
            instructions: instructionList, fromLeft: true },
        success: function (data, textStatus, xhr) {
            console.log(data.Data);
        }
    });
});

function parseTable(tableSelector) {
    var table = $(tableSelector);
    //todo: populate data
    var list = [];
    for (var i = 0; i < table.length; i++) {
        list.push({
            "symbol": 'a',
            "state": "q0",
            "symbolToWrite": 'b',
            "nextState": "q1",
            "movement": "L"
        });
    }
    return list;
}
$(document).ready(function () {
    $('#instructions-table').editableTableWidget();

    $('#inputData').on('change', function(e) {
        var tapePattern = /^[a-zA-Z0-9#]+$/;

        if (!tapePattern.test($(this).val())) {
            $(this).addClass("invalid");
        }
        else {
            $(this).removeClass("invalid");
        }
    });

    $('table td').on('validate', function (evt, newValue) {
        var statePattern = /^[a-zA-Z0-9]*$/;
        var alphabetPattern = /^[a-zA-Z0-9#]$/;
        var instructionPattern = /^[a-zA-Z0-9#]\s(\S){1,4}\s(L|R|N)$/;

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

        newRow.cells[0].classList.add("alphabet");

        $('#instructions-table').editableTableWidget();
    });

    $("#addColButton").on("click", function () {
        var table = document.getElementById("instructions-table");
        var cols = table.rows[0].cells.length;
        var rows = table.rows.length;
        
        var width = $('#instructions-table').width();
        var newWidth = width + 50;
        $('#instructions-table').width(newWidth);

        for (var i = 0; i < rows; i++) {
            var newCell = table.rows[i].insertCell(cols);
            newCell.innerHTML = "-";
        }

        table.rows[0].cells[cols].classList.add("state");

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
    var instructionList = parseTableRows('#instructions-table');
    var fromLeft = $("input:radio[name='radio-group']:checked").val(); //todo: actually get the value from the page
    var inputData = $('#inputData').val(); //todo: actually get the value of the input
    var d = {
        "instructions": JSON.stringify(instructionList),
        "input": inputData,
        "fromLeft": (fromLeft === "left")
    };
    console.log(d);
    $.post({
        url: '/Home/SaveSettings',
        dataType: "json",
        data: d,
        success: function (data, textStatus, xhr) {
            console.log(data.Data);
        }
    });
});

function parseTableRows(tableSelector) {
    var table = document.getElementById("instructions-table");
    var tableRows = $(tableSelector + " tr"); //table tr selector
    var list = [];
    var colCount = table.rows[0].cells.length;
    tableRows.each(function () {
        if (!this.rowIndex) return; // skip first row
        var row = this;
        var symbol = row.cells[0].innerText;
        for (var i = 1; i < colCount; ++i) { //because first column is <th> instead of <td>
            var state = tableRows[0].cells[i].innerText;
            var instr = row.cells[i].innerText.split(" ");
            list.push({
                "symbol": symbol,
                "state": state,
                "symbolToWrite": instr[0],
                "nextState": instr[1],
                "movement": instr[2]
            });
        }
    });
    console.log(list);
    return list;
}
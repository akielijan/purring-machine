$(document).ready(function () {
    $('#instructions-table').editableTableWidget();

    $("#logo").on("click", function () {
        var audio = $('audio')[0];
        if (audio.paused == false) {
            audio.pause();
        } else {
            audio.play();
        }
    });

    $('#inputData').on('change', function(e) {
        var tapePattern = /^[a-zA-Z0-9#]+$/;

        if (!tapePattern.test($(this).val())) {
            $(this).addClass("invalid");
        }
        else {
            $(this).removeClass("invalid");
        }
    });

    $('table td').on('validate', validateInstruction);

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

        $('table td').on('validate', validateInstruction);
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

        $('table td').on('validate', validateInstruction);
        $('#instructions-table').editableTableWidget();
    });
});

$('#run').on('click', function () {
    $.get({
        url: '/Home/RunMachine',
        success: function (data, textStatus, xhr) {
            $('#fullTape').text(data.Data);
            updateTape();
        }
    });
});

$('#reset').on('click', function () {
    $.get({
        url: '/Home/ResetMachine',
        success: function (data, textStatus, xhr) {
            console.log("success: " + xhr.status);
            updateTape();
        }
    });
});

$('#nextStep').on('click', function () {
    $.get({
        url: '/Home/NextStep',
        success: function (data, textStatus, xhr) {
            $('#fullTape').text(data.Data);
            updateTape();
            if (!data.Success)
                $('#modalStopped').modal('toggle');
        }
    });
});

function updateTape() {
    var characters = 5;
    $.get({
        url: '/Home/GetTapeData?n=' + characters, //todo: setup parameter somewhere else
        success: function (data, textStatus, xhr) {
            var newTapeData = data.Data;
            for (var i = 0; i < characters; ++i) {
                $('#tapeSymbol' + i).text(newTapeData[i]);
            }
        }
    });

    $.get({
        url: '/Home/GetFinishStatusStyleClassName',
        success: function (data, textStatus, xhr) {
            var className = data.Data;
            var middleTapeElement = $('#middleTapeElement');

            middleTapeElement.removeClass("rainbow-div");
            middleTapeElement.removeClass("neutral-div");
            middleTapeElement.removeClass("negative-div");
            middleTapeElement.removeClass("positive-div");

            middleTapeElement.addClass(className);
        }
    });
}

$('#saveSettings').on('click', function () {
    if ($('.invalid').length > 0) {
        $('#modalInvalid').modal('toggle');
        $('.invalid')[0].focus();
        return;
    }
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
            location.href = '/Home/Index';
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
        if (symbol === '-') return; //remove row

        for (var i = 1; i < colCount; ++i) { 
            var state = tableRows[0].cells[i].innerText;
            if (state === '-') continue; //skip column

            var cellData = row.cells[i].innerText;
            if (cellData === '-') continue; //skip instruction

            var instrData = cellData.split(" ");
            var obj = {
                "symbol": symbol,
                "state": state,
                "symbolToWrite": instrData[0],
                "nextState": instrData[1],
                "movement": instrData[2]
            }
            list.push(obj);
        }
    });
    console.log(list);
    return list;
}

function validateInstruction(evt, newValue) {
    var statePattern = /^[a-zA-Z0-9]{1,4}|-$/;
    var alphabetPattern = /^[a-zA-Z0-9#]|-$/;
    var instructionPattern = /^[a-zA-Z0-9#]\s(\S){1,4}\s(L|R|N)|-$/;

    var pattern;

    if ($(this).hasClass("state")) {
        pattern = statePattern;
        
    } else if ($(this).hasClass("alphabet")) {
        pattern = alphabetPattern;
    } else {
        pattern = instructionPattern;
    }

    if (!pattern.test(newValue)) {
        $(this).addClass("invalid");
    } else {
        $(this).removeClass("invalid");
    }
}
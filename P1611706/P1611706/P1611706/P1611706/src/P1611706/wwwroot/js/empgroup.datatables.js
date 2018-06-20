
var ready;
var time = setInterval(removeStyle, 100);
var empGroupTable;
var checkGroup = [];
$(document).ready(function () {
    empGroupTable = $('#empGroup-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
            "url": "api/EmpGroupApi",
            "type": 'GET'
        },
        "columns": [
           { "data": "employeeNo" },
           { "data": "employeeName" },
           { "data": "groupName" }
        ],
        "columnDefs": [
            {

                "render": function (data, type, row) {
                    return '<button class="btn btn-primary" type="button" onclick="btnEdit(\'' + row.employeeNo + '\', \'' + row.employeeName + '\', \'' + row.groupName + '\');"><i class="glyphicon glyphicon-pencil"></i>Edit</button>';

                },
                "targets": 3
            }
        ]
    });
    
    $("#empGroup-table_filter").css("display", "none");

});

//移除table style屬性
function removeStyle() {
	var style = $('#empGroup-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#empGroup-table').removeAttr("style");
}

function searchEmpGroup() {

    //var data = JSON.stringify(objectifyForm($(searchForm).serializeArray()));
    var data = "{ 'EmployeeNo': '" + $('#EmployeeNo').val() + "', 'EmployeeName': '" + $('#EmployeeName').val() + "' }";
    empGroupTable.search(data).draw();
    
}

function btnEdit(employeeNo, employeeName, groupName)
{
    if (employeeNo == "null")
        employeeNo = "";
    if (employeeName == "null")
        employeeName = "";
    if (groupName == "null")
        groupName = "";
    BootstrapDialog.show({
        title: '編輯-' + employeeNo,
        message:'<div class="input-group">' +
                    '<span class="input-group-addon">員工編號</span>' +
                    '<input type="text" class="form-control" value="' + employeeNo + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">姓名</span>' +
                    '<input type="text" class="form-control" value="' + employeeName + '" disabled >' +
                '</div>' +
                '</br>' +
                '群組權限' +
                '<table id="group-table" class="table  table-bordered table-hover">' +
                    '<thead>' +
                    '<tr>' +
                        '<th width="50">勾選</th>' +
                        '<th>群組代碼</th>' +
                        '<th>群組名稱</th>' +
                    '</tr>' +
                    '</thead>' +
                    '<tbody>' +
                    '</tbody>' +
                '</table>',
        onshown: function (dialog) {
            createProgram(groupName);
            $('#group-table_paginate').css("display", "none");

        },
        buttons: [{
            label: '確認',
            cssClass: 'btn-primary',
            action: function (dialog) {
                var strGroupID = "";
                for (var i = 0; i < checkGroup.length; i++)
                {
                    if (i == 0)
                        strGroupID += checkGroup[i];
                    else
                        strGroupID = strGroupID + "," + checkGroup[i] ;
                }
                var empGroup = { employeeNo: employeeNo, groupID: strGroupID };
                $.ajax({
                    type: 'Put',
                    url: 'api/EmpGroupApi',
                    data: JSON.stringify(empGroup),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'text',
                    success: function (data) {
                        if (data == "success") {
                            //alert("編輯成功!!");
                            dialog.close();
                            empGroupTable.draw(false);

                        }
                        else {
                            alert("編輯失敗!!" + data);
                        }
                    },
                    error: function (data) {
                        alert("編輯失敗!!" + data);
                    }
                });
            }
        }, {
            label: '取消',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });

}

function createProgram(groupName) {
    var groupTable;
    var arrGroupName = groupName.split(',');
    checkGroup = [];
    
    groupTable = $('#group-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
            "url": "api/GroupApi",
            "type": 'GET'
        },
        "columns": [
           { "data": "" },
           { "data": "groupID" },
           { "data": "groupName" }
        ],
        "columnDefs": [
            {

                "render": function (data, type, row) {
                    var thHTML;
                    var flag = false;
                    var flagActive = false;

                    for (var i = 0; i < arrGroupName.length; i++) {
                        if (arrGroupName[i] == row.groupName) {
                            flagActive = true;
                            thHTML = '<th class="text-center"><div class="btn-group" data-toggle="buttons"><label id="' + row.groupID + '" class="btn btn-primary active" onclick="changeShowTips(this)">' +
				                        '<input type="checkbox" autocomplete="off"   >' +
				                        '<span class="glyphicon glyphicon-ok"></span>' +
			                            '</label></div></th>';
                            break;
                            
                            
                        }
                    }
                    if (flagActive == false) {
                        thHTML = '<th class="text-center"><div class="btn-group" data-toggle="buttons"><label id="' + row.groupID + '" class="btn btn-primary " onclick="changeShowTips(this)">' +
				                        '<input type="checkbox" autocomplete="off"   >' +
				                        '<span class="glyphicon glyphicon-ok"></span>' +
			                            '</label></div></th>';
                    }
                    else
                    {
                        for (var i = 0; i < checkGroup.length; i++) {
                            if (checkGroup[i] == row.groupID) {
                                flag = true;
                                break;
                            }
                        }
                        if (flag == false)
                            checkGroup.push(row.groupID);
                    }

                    return thHTML;

                },
                "targets": 0
            }
        ]
    });

    $("#group-table_filter").css("display", "none");
}

function changeShowTips(showTips) {
    var classlength = showTips.classList.length;
    if (classlength == 2) {
        for (var i = 0; i < checkGroup.length; i++) {
            if (checkGroup[i] == showTips.id)
                return;
        }
        checkGroup.push(showTips.id);
    }
    else {
        for (var i = 0; i < checkGroup.length; i++) {
            if (checkGroup[i] == showTips.id) {
                checkGroup.splice(i, 1);
                break;
            }
        }
    }
}

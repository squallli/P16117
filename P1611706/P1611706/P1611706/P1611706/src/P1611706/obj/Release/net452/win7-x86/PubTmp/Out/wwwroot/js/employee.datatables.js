var ready;
var time = setInterval(removeStyle, 100);
var employeeTable;

$(document).ready(function () {

    employeeTable = $('#employee-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
            "url": "api/EmployeeApi",
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
                    return '<button class="btn btn-primary" type="button" onclick="btnEdit(\'' + row.employeeNo + '\', \'' + row.employeeName + '\');"><i class="glyphicon glyphicon-pencil"></i>Edit</button>' +
                        '<button class="btn btn-danger" type="button" name="remove_levels" value="delete" onclick="btnDelete(\'' + row.employeeNo + '\', \'' + row.employeeName + '\');"><span class="fa fa-times"></span>Delete</button>' +
                        '<button class="btn btn-warning" type="button" name="remove_levels" value="reset" onclick="btnPassword(\'' + row.employeeNo + '\', \'' + row.employeeName + '\');"><span class="fa fa-key"></span>密碼重設</button>';
                    
                },
                "targets": 3
            }
        ]
    });
    
    $("#employee-table_filter").css("display", "none");

});

//移除table style屬性
function removeStyle() {
	var style = $('#employee-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#employee-table').removeAttr("style");
}

function searchEmployee() {
    var data = "{ 'EmployeeNo': '" + $('#EmployeeNo').val() + "', 'EmployeeName': '" + $('#EmployeeName').val() + "' }";
    employeeTable.search(data).draw();
    
}

function addEmployee() {

	BootstrapDialog.show({
		title: '新增群組',
		message: '<div class="input-group">' +
		'<span class="input-group-addon">員工編號</span>' +
		'<input type="text" id="txtEmpID" class="form-control"  >' +
		'</div>' +
		'</br>' +
		'<div class="input-group">' +
		'<span class="input-group-addon">員工姓名</span>' +
		'<input type="text" id="txtEmpName" class="form-control" >' +
		'</div>' +
		'</br>' +
		'<div class="form-group">' +
		'<label>密碼</label>' +
		'<div class="input-group">' +
		'<div class="input-group-addon">' +
		'<i class="glyphicon glyphicon-wrench"></></i>' +
		'</div>' +
		'<input class="form-control" data-val="true" data-val-length="password must be between 6 and 100 characters long." data-val-length-max="100" data-val-length-min="6" data-val-required="password is required" id="Password" name="Password" placeholder="請輸入密碼，長度只少為6" rows="12" type="password" />' +
		'</div>' +
		'</br>' +
		'<div class="form-group">' +
		'<label>確認密碼</label>' +
		'<div class="input-group">' +
		'<div class="input-group-addon">' +
		'<i class="glyphicon glyphicon-wrench"></></i>' +
		'</div>' +
		'<input class="form-control" data-val="true" data-val-length="password must be between 6 and 100 characters long." data-val-length-max="100" data-val-length-min="6" data-val-required="password is required" id="ComfirmPassword" name="ComfirmPassword" placeholder="請輸入密碼，長度只少為6" rows="12" type="password" />' +
		'</div>' +
		'</br>',
		onshown: function (dialog) {
			$('#ComfirmPassword').change(function () {
				if ($('#ComfirmPassword').val() != $('#Password').val())
					alert('密碼不相符!!');
			});

			$('#Password').change(function () {
				if ($('#Password').val().length < 6)
					alert('長度至少為6!!');
			});

			$('#btnSave').click(function () {
				if ($('#Password').val() == "")
					alert('請輸入密碼');
				else if ($('#ComfirmPassword').val() == "")
					alert('請確認新密碼');
			});
		},
		type: BootstrapDialog.TYPE_INFO,
		buttons: [{
			label: '確認',
			cssClass: 'btn-info',
			action: function (dialog) {
				if ($('#Password').val() == "") {
					alert('請輸入密碼');
					return;
				}
				else if ($('#ComfirmPassword').val() == "") {
					alert('請確認新密碼');
					return;
				}
				var employee = { employeeNo: $('#txtEmpID').val(), employeeName: $('#txtEmpName').val(), passWord: $('#Password').val() }; 
				//url: '/api/EmployeeApi/Add',
				$.ajax({
					type: 'Post',
					url: 'api/EmployeeApi/Add',
					data: JSON.stringify(employee),
					contentType: 'application/json; charset=utf-8',
					dataType: 'text',
					success: function (data) {
						if (data == "success") {
							//alert("新增成功!!");
							dialog.close();
							employeeTable.draw(false);

						}
						else {
							alert("新增失敗!!" + data);
						}
					},
					error: function (data) {
						alert("新增失敗!!" + data);
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

function btnEdit(employeeNo, employeeName)
{
    var sexVal;
    if (employeeNo == "null")
        employeeNo = "";
    if (employeeName == "null")
        employeeName = "";
    BootstrapDialog.show({
        title: '編輯-' + employeeNo,
        message:'<div class="input-group">' +
                    '<span class="input-group-addon">員工編號</span>' +
                    '<input type="text" class="form-control" value="' + employeeNo + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">姓名</span>' +
                    '<input type="text" id="txtEmployeeName" class="form-control" value="' + employeeName + '"  >' +
                '</div>' +
                '</br>',
        onshown: function (dialog) {
			
        },
        buttons: [{
            label: '確認',
            cssClass: 'btn-primary',
            action: function (dialog) {
                var sexValue = $('#selectSex').val();

                var employee = { employeeNo: employeeNo, employeeName: $('#txtEmployeeName').val() };
                $.ajax({
                    type: 'Put',
                    url: 'api/EmployeeApi',
                    data: JSON.stringify(employee),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'text',
                    success: function (data) {
                        if (data == "success") {
                            //alert("編輯成功!!");
                            dialog.close();
                            employeeTable.draw(false);

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

function btnPassword(employeeNo, employeeName) {
    var sexVal;
    if (employeeNo == "null")
        employeeNo = "";
    if (employeeName == "null")
        employeeName = "";
   
    BootstrapDialog.show({
        title: '重設密碼-' + employeeNo,
        message: '<div class="input-group">' +
                            '<span class="input-group-addon">員工編號</span>' +
                            '<div class="form-group">' +
                                '<input type="text" class="form-control" id="EmployeeNo" name="EmployeeNo" value="' + employeeNo + '" disabled>' +
                            '</div>' +
                        '</div>' +
                        '<br/>' +
                        '<div class="input-group">' +
                            '<span class="input-group-addon">員工姓名</span>' +
                            '<div class="form-group">' +
                                '<input type="text" class="form-control" id="EmployeeName" name="EmployeeName" value="' + employeeName + '" disabled>' +
                            '</div>' +
                        '</div>' +
                       '<br/>' +
                        '<div class="form-group">' +
                            '<label>新密碼</label>' +
                            '<div class="input-group">' +
                                '<div class="input-group-addon">' +
                                    '<i class="glyphicon glyphicon-wrench"></></i>' +
                                '</div>' +
                                '<input class="form-control" data-val="true" data-val-length="password must be between 6 and 100 characters long." data-val-length-max="100" data-val-length-min="6" data-val-required="password is required" id="NewPassword" name="NewPassword" placeholder="請輸入新密碼，長度只少為6" rows="12" type="password" />' +
                            '</div>' +
                        '</div>' +
                        '<div class="form-group">' +
                            '<label>新密碼確認</label>' +
                            '<div class="input-group">' +
                                '<div class="input-group-addon">' +
                                    '<i class="glyphicon glyphicon-wrench"></></i>' +
                                '</div>' +
                                '<input class="form-control" data-val="true" data-val-length="password must be between 6 and 100 characters long." data-val-length-max="100" data-val-length-min="6" data-val-required="password is required" id="ComfirmPassword" name="ComfirmPassword" placeholder="請輸入新密碼，長度只少為6" rows="12" type="password" />' +
                            '</div>' +
                        '</div>',
        onshown: function (dialog) {
            $('#ComfirmPassword').change(function () {
                if ($('#ComfirmPassword').val() != $('#NewPassword').val())
                    alert('新密碼不相符!!');
            });

            $('#NewPassword').change(function () {
                if ($('#NewPassword').val().length < 6)
                    alert('長度至少為6!!');
            });

            $('#btnSave').click(function () {
                if ($('#NewPassword').val() == "")
                    alert('請輸入新密碼');
                else if ($('#ComfirmPassword').val() == "")
                    alert('請確認新密碼');
            });
        },
        type: BootstrapDialog.TYPE_WARNING,
        buttons: [{
            label: '確認',
            cssClass: 'btn-warning',
            action: function (dialog) {

                var employee = { employeeNo: employeeNo, newPassword: $('#NewPassword').val() };
                $.ajax({
                    type: 'Post',
                    url: 'api/EmployeeApi',
                    data: JSON.stringify(employee),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'text',
                    success: function (data) {
                        if (data == "success") {
                            //alert("重設成功!!");
                            dialog.close();
                            employeeTable.draw(false);

                        }
                        else {
                            alert("重設失敗!!" + data);
                        }
                    },
                    error: function (data) {
                        alert("重設失敗!!" + data);
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

function addGroup() {

	BootstrapDialog.show({
		title: '新增群組',
		message: '<div class="input-group">' +
		'<span class="input-group-addon">群組代碼</span>' +
		'<input type="text" id="txtGroupID" class="form-control"  >' +
		'</div>' +
		'</br>' +
		'<div class="input-group">' +
		'<span class="input-group-addon">群組名稱</span>' +
		'<input type="text" id="txtGroupName" class="form-control" >' +
		'</div>' +
		'</br>' +
		'<table id="program-table" class="table  table-bordered table-hover">' +
		'<thead>' +
		'<tr>' +
		'<th width="50">勾選</th>' +
		'<th>作業名稱</th>' +
		'</tr>' +
		'</thead>' +
		'<tbody>' +
		'</tbody>' +
		'</table>',
		onshown: function (dialog) {
			createProgram(0);
			$('#program-table_paginate').css("display", "none");

		},
		type: BootstrapDialog.TYPE_INFO,
		buttons: [{
			label: '確認',
			cssClass: 'btn-info',
			action: function (dialog) {
				var power;
				for (var i = 0; i < checkProgram.length; i++) {
					power = power | checkProgram[i];
				}
				var group = { groupID: $('#txtGroupID').val(), groupName: $('#txtGroupName').val(), power: power };
				$.ajax({
					type: 'Post',
					url: 'api/GroupApi',
					data: JSON.stringify(group),
					contentType: 'application/json; charset=utf-8',
					dataType: 'text',
					success: function (data) {
						if (data == "success") {
							//alert("新增成功!!");
							dialog.close();
							groupTable.draw(false);

						}
						else {
							alert("新增失敗!!" + data);
						}
					},
					error: function (data) {
						alert("新增失敗!!" + data);
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

function btnDelete(employeeNo, employeeName) {
    BootstrapDialog.show({
        title: '刪除-' + employeeNo,
        message: '<div class="input-group">' +
        '<span class="input-group-addon">員工編號</span>' +
        '<input type="text" class="form-control" value="' + employeeNo + '" disabled >' +
        '</div>' +
        '</br>' +
        '<div class="input-group">' +
        '<span class="input-group-addon">員工姓名</span>' +
        '<input type="text" class="form-control" value="' + employeeName + '" disabled >' +
        '</div>' +
        '</br>' +
        '<h3>確認刪除此員工?</h3>',
        type: BootstrapDialog.TYPE_DANGER,
        buttons: [{
            label: '確認',
            cssClass: 'btn-danger',
            action: function (dialog) {
                var dataJSON = employeeNo;
                $.ajax({
                    type: 'DELETE',
                    url: 'api/EmployeeApi',
                    data: JSON.stringify(dataJSON),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'text',
                    success: function (data) {
                        if (data == "success") {
                            //alert("刪除成功!!");
                            dialog.close();
                            employeeTable.draw(false);

                        }
                        else {
                            alert("刪除失敗!!" + data);
                        }
                    },
                    error: function (data) {
                        //alert("刪除失敗!!" + data);
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


function ValidateNumber(e, pnumber) {
    if (pnumber == "")
        return false;
    if (!/^\d+$/.test(pnumber)) {
        alert("只能輸入數字!!");
        $(e).val(/^\d+/.exec($(e).val()));
    }
    return false;
}




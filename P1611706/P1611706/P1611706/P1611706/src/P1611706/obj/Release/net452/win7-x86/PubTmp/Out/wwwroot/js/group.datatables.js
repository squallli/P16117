var ready;
var time = setInterval(removeStyle, 100);
var groupTable;
var checkProgram = [];
$(document).ready(function () {
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
           { "data": "groupID" },
           { "data": "groupName" }
        ],
        "columnDefs": [
            {

                "render": function (data, type, row) {
                    return '<button class="btn btn-primary" type="button" onclick="btnEdit(\'' + row.groupID + '\', \'' + row.groupName + '\', \'' + row.power + '\');"><i class="glyphicon glyphicon-pencil"></i>Edit</button>' +
                    '<button class="btn btn-danger" type="button" name="remove_levels" value="delete" onclick="btnDelete(\'' + row.groupID + '\', \'' + row.groupName + '\');"><span class="fa fa-times"></span> delete</button>';

                },
                "targets": 2
            }
        ]
    });
    
    $("#group-table_filter").css("display", "none");

    
});

//移除table style屬性
function removeStyle() {
	var style = $('#group-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#group-table').removeAttr("style");
}


function searchGroup() {

    //var data = JSON.stringify(objectifyForm($(searchForm).serializeArray()));
    var data = "{ 'GroupID': '" + $('#GroupID').val() + "', 'GroupName': '" + $('#GroupName').val() + "' }";
    groupTable.search(data).draw();
    
}

function padLeft(str, lenght) {
	if (str.length >= lenght)
		return str;
	else
		return padLeft("0" + str, lenght);
}

function addGroup() {
	var groupId;

	$.ajax({
		type: "post",
		url: "Group/GetID",
		success: function (data) {
			groupId = data;
			groupId = data[0];
			groupId = data[0].groupID;

			groupId++;
			groupId = padLeft(groupId, 4);
			BootstrapDialog.show({
				title: '新增群組',
				message: '<div class="input-group">' +
				'<span class="input-group-addon">群組代碼</span>' +
				'<input type="text" id="txtGroupID" class="form-control" value="' + groupId + '"  >' +
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
									groupTable.draw(false);
								}
							},
							error: function (data) {
								alert("新增失敗!!" + data);
								groupTable.draw(false);
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
	})


    

}

function btnEdit(groupID, groupName, power)
{
    if (groupID == "null")
        groupID = "";
    if (groupName == "null")
        groupName = "";
    BootstrapDialog.show({
        title: '編輯-' + groupID,
        message:'<div class="input-group">' +
                    '<span class="input-group-addon">群組代碼</span>' +
                    '<input type="text" id="txtGroupID" class="form-control" value="' + groupID + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">群組名稱</span>' +
                    '<input type="text" id="txtGroupName" class="form-control" value="' + groupName + '"  >' +
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
        onshown: function(dialog){
            createProgram(power);
            $('#program-table_paginate').css("display", "none");

        },
        buttons: [{
            label: '確認',
            cssClass: 'btn-primary',
            action: function (dialog) {
                var power;
                for (var i = 0; i < checkProgram.length; i++) {
                    power = power | checkProgram[i];
                }
                var group = { groupID: $('#txtGroupID').val(), groupName: $('#txtGroupName').val(), power: power };
                $.ajax({
                    type: 'Put',
                    //url: '/api/GroupApi/' + groupID,
                    url: 'api/GroupApi/',
                    data: JSON.stringify(group),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'text',
                    success: function (data) {
                        if (data == "success") {
                            //alert("編輯成功!!");
                            dialog.close();
                            groupTable.draw(false);

                        }
                        else
                        {
                            alert("編輯失敗!!" + data);
                            groupTable.draw(false);
                        }
                    },
                    error: function (data) {
                        alert("編輯失敗!!" + data);
                        groupTable.draw(false);
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

function btnDelete(groupID, groupName) {
    BootstrapDialog.show({
        title: '刪除-' + groupID,
        message: '<div class="input-group">' +
                    '<span class="input-group-addon">群組代碼</span>' +
                    '<input type="text" class="form-control" value="' + groupID + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">群組名稱</span>' +
                    '<input type="text" class="form-control" value="' + groupName + '" disabled >' +
                '</div>' +
                '</br>' +
                '<h3>確認刪除此項目?</h3>',
        type: BootstrapDialog.TYPE_DANGER,
        buttons: [{
            label: '確認',
            cssClass: 'btn-danger',
            action: function (dialog) {
                var dataJSON = groupID;
                $.ajax({
                    type: 'DELETE',
                    url: 'api/GroupApi',
                    data: JSON.stringify(dataJSON),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'text',
                    success: function (data) {
                        if (data == "success") {
                            //alert("刪除成功!!");
                            dialog.close();
                            groupTable.draw(false);

                        }
                        else {
                            alert("刪除失敗!!" + data);
                        }
                    },
                    error: function (data) {
                        alert("刪除失敗!!" + data);
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

function createProgram(power) {
    var programTable;
    checkProgram = [];

    programTable = $('#program-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
            "url": "api/ProgramApi",
            "type": 'GET'
        },
        "columns": [
           { "data": "" },
           { "data": "progName" }
        ],
        "columnDefs": [
            {

                "render": function (data, type, row) {
                    var thHTML;
                    var flag = false;

					if ((power & row.power) == 0)
						{
						thHTML = '<th class="text-center"><div class="btn-group" data-toggle="buttons"><label id="' + row.power + '" class="btn btn-primary " onclick="changeShowTips(this)">' +
							'<input type="checkbox" autocomplete="off"   >' +
							'<span class="glyphicon glyphicon-ok"></span>' +
							'</label></div></th>';
						//thHTML = '<th class="text-center"><input type="checkbox" id="' + row.power + '" onclick="changeShowTips(this)" ></th>';
					}
                    else {
                        for (var i = 0; i < checkProgram.length; i++)
                        {
                            if (checkProgram[i] == row.power) {
                                flag = true;
                                break;
                            }
                        }
                        if (flag == false)
							checkProgram.push(row.power);
						
						thHTML = '<th class="text-center"><div class="btn-group" data-toggle="buttons"><label id="' + row.power + '" class="btn btn-primary active" onclick="changeShowTips(this)">' +
							'<input type="checkbox" autocomplete="off"   >' +
							'<span class="glyphicon glyphicon-ok"></span>' +
							'</label></div></th>';
                        //thHTML = '<th class="text-center"><input type="checkbox" id="' + row.power + '" onclick="changeShowTips(this)" checked=true ></th>';
                    }
                    return thHTML;

                },
                "targets": 0
            }
        ]
    });

    $("#program-table_filter").css("display", "none");
}

function changeShowTips(showTips) {
	var classlength = showTips.classList.length;
	if (classlength == 2) {
		for (var i = 0; i < checkProgram.length; i++) {
			if (checkProgram[i] == showTips.id)
				return;
		}
		checkProgram.push(showTips.id);
	}
	else {
		for (var i = 0; i < checkProgram.length; i++) {
			if (checkProgram[i] == showTips.id) {
				checkProgram.splice(i, 1);
				break;
			}
		}
	}
}


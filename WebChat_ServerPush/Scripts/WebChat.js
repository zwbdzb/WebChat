var Global = {
    Action: null,
    MyInfo: current_user,
    FriendInfo: null
};
// Ajax请求参数信息提交类，对象.ToJson就会变成JSon对象
function PHPArray(Action) {
    this.params = new Object();
    this.length = 3;
    Global.Action = Action;
    this.params["Action"] = Global.Action;
    this.params["UserId"] = Global.MyInfo.UserId;
    this.params["PassWord"] = Global.MyInfo.PassWord;
}
PHPArray.prototype.Add = function (key, value) {
    this.params[key] = value;
    this.length++;
}
PHPArray.prototype.ToJson = function () {
    var tempstr = "{";
    for (var key in this.params) {
        tempstr += key + ":\"" + this.params[key] + "\",";
    }
    tempstr = tempstr.substring(0, tempstr.length - 1);
    tempstr += "}";
    return (new Function("return " + tempstr))();           // 显示产生函数对象
}

$(document).ready(function () {
    //初始化事件
    InitEvent();
    //显示我的资料
    ShowUserInfo("my", Global.MyInfo);
    //获取好友
    GetFriends();

    //维持长连接,这是通信的关键  Comet推送
    Keepline();
});
//初始化事件
function InitEvent() {              //  注册鼠标覆盖、鼠标移出、鼠标双击事件 
    $('#Friends .list .user').live('mouseover mouseout dblclick', function (e) {
        if (e.type == 'mouseover') {
            // do something on mouseover
            //Selected==0表示未选中,1表示选中
            if ($(this).attr("Selected") == "false") {
                $(this).css("background-color", "#B0DAED");
            }
                var friend = {
                UserId: $(this).attr("UserId"),
                UserName: $(this).attr("UserName"),
                Sex: $(this).attr("Sex"),
                Age: $(this).attr("Age"),
                Email: $(this).attr("Email")
            };
            ShowUserInfo("temp_friend", friend);
            $("#tabFriend").css({
                "top": $(e.toElement).offset().top + this.offsetHeight + "px",
                "left": $(e.toElement).offset().left + this.offsetWidth + "px"
            }).show("fast");

        } else if (e.type == "mouseout") {
            // do something on mouseout
            if ($(this).attr("Selected") == "false") {
                $(this).css("background-color", "transparent");
            }
            $("#tabFriend").remove();

        } else if (e.type == "dblclick") {
            if ($(this).attr("Selected") == "true") return;
            if ($(this).attr("Status") == "离线") return;
            $("#Friends .list .user[Selected='true']")
            .css("background-color", "transparent")
            .attr("Selected", "false");
            $(this).attr("Selected", "true");
            Global.FriendInfo = {
                UserId: $(this).attr("UserId"),
                UserName: $(this).attr("UserName"),
                Sex: $(this).attr("Sex"),
                Age: $(this).attr("Age"),
                Email: $(this).attr("Email")
            };
            // 双击之后显示对方的信息
            ShowUserInfo("friend", Global.FriendInfo);
            $(this).css("background-color", "#ffe6b0");
        }
    });
    //发送消息
    $("#btnSendMsg").click(function () { SendMsg(); });
}
function ShowUserInfo(type, user) {
    var tabId, tabTitle, selector;
    selector = "#Messages .user_info";
    if (type == "my") {
        tabId = "my_info";
        tabTitle = "我的资料(" + user.UserId + ")";
        $("#Messages .user_info #" + tabId).remove();
    } else {
        tabTitle = "好友资料(" + user.UserId + ")";
        if (type == "friend") {
            tabId = "friend_info";
            $("#Messages .user_info #" + tabId).remove();
        } else {
            tabId = "tabFriend";
            selector = "body";
        }
    }
    var tempstr = "<table id='" + tabId + "' cellpadding='0' cellspacing='0'>";
    tempstr += "<tr>";
    tempstr += "<td colspan='4' class='title'>" + tabTitle + "</td>";
    tempstr += "</tr>";
    tempstr += "<tr>";
    tempstr += "<td class='label'>姓名：</td>";
    tempstr += "<td colspan='3'>" + user.UserName + "</td>";
    tempstr += "</tr>";
    tempstr += "<tr>";
    tempstr += "<td class='label'>性别：</td>";
    tempstr += "<td>" + user.Sex + "</td>";
    tempstr += "<td class='label'>年龄：</td>";
    tempstr += "<td>" + user.Age + "</td>";
    tempstr += "</tr>";
    tempstr += "<tr>";
    tempstr += "<td class='label'>邮箱：</td>";
    tempstr += "<td colspan='3'>" + user.Email + "</td>";
    tempstr += "</tr>";
    tempstr += "</table>";
    $(selector).append(tempstr);
}

function PostSubmit(params, success) {
    $.post("comet_broadcast.asyn", params,
    success, "json");
}

// 基于Ajax的长轮询long-polling 方式实现的Comet技术
function Keepline() {
    var array = new PHPArray("Keepline");
    // 本地JS函数
    var success = function (data, status) {
        if (data.ResponseCode == 1) {
            ShowMessage(data.ResponseData, "recive");
        }
        Keepline();
    }
    PostSubmit(array.ToJson(), success);
}
// Ajax获取所有朋友，最后在区分上下线
function GetFriends() {
    var array = new PHPArray("GetFriends");
    var success = function (data, status) {
        if (data.ResponseCode != 1) return;
        ShowFriends(data.ResponseData.OnlineFriends, "Online");
        ShowFriends(data.ResponseData.OfflineFriends, "Offline");
    }
    PostSubmit(array.ToJson(), success);
}

// 根据类型显示不同的朋友
function ShowFriends(data, type) {
    if (!data) return;
    var tempstr = "";
    // 为每个朋友构造HTML字符串tempstr
    $(data).each(function (index, item) {
        tempstr += "<div class='user' selected='false' ";
        tempstr += "UserId='" + item.UserId + "' ";
        tempstr += "UserName='" + item.UserName + "' ";
        tempstr += "PassWord='" + item.PassWord + "' ";
        tempstr += "Sex='" + item.Sex + "' ";
        tempstr += "Age='" + item.Age + "' ";
        tempstr += "Email='" + item.Email + "' ";
        tempstr += "Status='" + item.Status + "' ";
        tempstr += ">";
        tempstr += item.UserId;
        tempstr += "</div>";
    });
    var selector = "#Friends ";
    if (type == "Online") {
        selector += ".list_online";
    } else {
        selector += ".list_offline";
    }
    selector += " .list";
    $(selector).append(tempstr);
}
function SendMsg() {
    if (!Global.FriendInfo) {
        alert("请双击选择一位在线好友...");
        return;
    }
    var Content = $("#txtSendMsg").val();
    var array = new PHPArray("SendMsg");
    array.Add("ReciveUserId", Global.FriendInfo.UserId);
    array.Add("Content", Content);
    var success = function (data, status) {
        ShowMessage(data.ResponseData, "send");
    }
    $("#txtSendMsg").val("");
    PostSubmit(array.ToJson(), success);
}
function ShowMessage(message, type) {
    var tabClass;
    if (type == "send") {                           // 决定本行使用Send-Class类，还是Receive-Class类
        tabClass = "sendmsg";
    } else if (type == "recive") {
        tabClass = "recivemsg";
    }
    var tempstr = "<table class='" + tabClass + "' cellpadding='0' cellspacing='0'>";
    tempstr += "<tr>";
    tempstr += "<td class='user'>" + message.SendUserId + " " + message.SendTime + "</td>";
    tempstr += "</tr>";
    tempstr += "<tr>";
    tempstr += "<td class='msg'>" + message.Content + "</td>";
    tempstr += "</tr>";
    tempstr += "</table>";
    $("#Messages .chat .message").append(tempstr);
}
function Offline() {
    var array = new PHPArray("Offline");
    var success = function (data, status) {
        alert("下线成功！");
    }
    PostSubmit(array, success);
    return false;
}
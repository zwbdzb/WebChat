$(function () {
    //登录
    $("#btnSubmit").click(function () {
        var UserId = $("#txtUserId").val();
        if (!UserId) { $("#msg").text("请输入账  号！"); return; }
        var PassWord = $("#txtPassWord").val();
        if (!PassWord) { $("#msg").text("请输入密  码！"); return; }
        $.post("comet_broadcast.asyn",
        {                                                           // 提交的数据时JSon格式
            Action: "Online",
            UserId: UserId,
            PassWord: PassWord
        },                                                          // 以下是本地JS处理函数
        function (data, status) {
            if (data.ResponseCode != 1) {                           // 登录失败，返回详细错误信息
                $("#msg").text(data.ResponseDetails);
                return;
            }
            var url = "WebChat.aspx";                               // 登录成功，开始拼接Chat-URL
            url += "?UserId=" + data.ResponseData.UserId;
            url += "&UserName=" + data.ResponseData.UserName;
            url += "&PassWord=" + data.ResponseData.PassWord;
            url += "&Sex=" + data.ResponseData.Sex;
            url += "&Age=" + data.ResponseData.Age;
            url += "&Email=" + data.ResponseData.Email;
            window.location.href = url;
        }, "json");
    });
    //注册
    $("#btnRegister").click(function () {
        window.location.href = "Register.aspx";
    });
});

/* login.aspx页面的js代码，使用Ajax-action提交验证*/


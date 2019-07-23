<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebApplication1.About" %>

<html>
    <head>
        <title>test</title>
    </head>
    <body>
        <div>判断浏览器</div>
    </body>
</html>
<script>

    var ua = navigator.userAgent;
    if (ua.match(/Windows\sPhone/i) != null) return true;
    if (ua.match(/iPhone|iPod/i) != null) return true;
    if (ua.match(/Android/i) != null) return true;
    if (ua.match(/Edge\D?\d+/i) != null) return true;

    var verTrident = ua.match(/Trident\D?\d+/i);
    var verIE = ua.match(/MSIE\D?\d+/i);
    var verOPR = ua.match(/OPR\D?\d+/i);
    var verFF = ua.match(/Firefox\D?\d+/i);
    var x64 = ua.match(/x64/i);


    alert(verTrident);
    alert(verIE);
    alert(verOPR);
    alert(verFF);
    alert(x64);
</script>
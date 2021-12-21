$(document).ready(function () {

    let clientsIdArray = [];

    $("a.page-link").removeAttr("href");

    $(document).on("click", "a.page-link", function () {
        //debugger;
        //por defecto el evento del a (acnhor o action link) nos lleva al index, entonces se lo deshabilitamos (prevenimos)
        event.preventDefault();

        let _pageIndex = $(this).html();

        var url_ = "/Groups/GetClientsList?pageIndex=" + _pageIndex;

        //con jquery llamamos a un método get del controller
        //$.get(url_, function (data) {
        //    debugger;
        //    $("div#clientsList").html(data);
        //});

        $.ajax({
            url: url_,
            data: {},//formData,
            cache: false,
            contentType: false,
            processData: false,
            type: 'GET',
            async: false,
            success: function (data) {
                //debugger;
                $("div#clientsList").html(data);
                $("a.page-link").removeAttr("href");

                //COLOCAMOS CHECK A LOS INPUT CUYO ID CLIENTE COINCIDA CON EL QUE TENEMOS EN EL ARRAY
                checkIfWasChecked();
                console.log(clientsIdArray);
            },
            error: function (e) {
                //debugger;
                //console.log(e);
            }
        });

    });

    //ESTA FUNCION LE COLOCA CHECK SI YA LO HABIAMOS SELECCIONADO
    function checkIfWasChecked() {
        //ME QUEDO CON TODOS LOS CHECKBOX
        var checkBoxs = document.getElementsByClassName("clientGroup");

        for (var i = 0; i < checkBoxs.length; i++) {
            console.log(checkBoxs[i]);

            let existe = findValueInArray(checkBoxs[i].dataset.clientid, clientsIdArray);

            console.log("existe? " + existe);

            if (existe) {
                console.log(checkBoxs[i]);
                checkBoxs[i].checked = true;
            }//checkBoxs[i].attr("checked", true);
        }
    }


    function findValueInArray(value, arr) {
        let result = false;

        for (var i = 0; i < arr.length; i++) {
            var name = arr[i];
            if (name === value) {
                result = true;
                break;
            }
        }

        return result;
    }

    $(document).on("click", "input.clientGroup", function () {

        /*TENGO QUE PREGUNTAR SI ESTÁ CHEQUEADO O NO
         
        - SI ESTÁ CHEQUEADO, AGREGO EL ID DEL CLIENTE AL ARRAT
        - SI NO ESTÁ CHEQUEADO, LO REMUEVO

         */

        let clientId = $(this)[0].dataset.clientid;
        let isChecked = $(this).is(":checked");

        console.log("chequeado? " + $(this).is(":checked"));
        console.log("valor del dataset " + $(this)[0].dataset.clientid);

        console.log("-----------------------------------------------");

        //1 - PREGINTO SI ESTA CHEQUEADO
        if (isChecked)
            clientsIdArray.push(clientId)
        else {
            clientsIdArray = jQuery.grep(clientsIdArray, function (value) {
                return value != clientId;
            });
        }

        console.log(clientsIdArray);
        //arr.splice($.inArray(clientId, clientsIdArray), 1);
    });

    //CREAMOS EL GRUPO USANDO AJAX
    $("input#createGroup").click(function () {

        let _groupName = $("input#Name").val();
        let _groupDescription = $("input#Description").val();

        var formData = new FormData();

        formData.append('groupName', _groupName);
        formData.append('groupDescription', _groupDescription);
        formData.append('clientsId', JSON.stringify(clientsIdArray));

        $.ajax({
            url: "/Groups/CreateGroup",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            type: 'POST',
            async: true,
            success: function () {

            },
            error: function (e) {
                alert(e);
            }
        });

    });

});
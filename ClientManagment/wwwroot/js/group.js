$(document).ready(function () {

    let clientsIdArray = [];

    fillArrayOfId();

    $("a.page-link").removeAttr("href");

    $(document).on("click", "a.page-link", function () {
        //por defecto el evento del a (acnhor o action link) nos lleva al index, entonces se lo deshabilitamos (prevenimos)
        event.preventDefault();
        //nos quedamos con la página actual
        let _pageIndex = $(this).html();
        //llamamos al método que nos actualiza la tabla clientes
        fillClientsTable(_pageIndex);
    });

    $(document).on("click", "button#searchButton", function () {
        fillClientsTableWhenFilter();
    });

    $(document).bind("enterKey", "input#filter", function (e) {
        fillClientsTableWhenFilter();
    });

    $(document).on("keyup", "input#filter", function (e) {
        if (e.keyCode == 13) {
            $(this).trigger("enterKey");
        }
    });

    function fillClientsTableWhenFilter() {
        let _pageIndex = $("li.active")[0] != undefined ? $("li.active")[0].children[0].text : "1";
        fillClientsTable(_pageIndex);
    }

    function fillClientsTable(_pageIndex) {

        let _filter = $("input#filter").val();
        let url_ = "/Groups/GetClientsList?filter=" + _filter + "&pageIndex=" + _pageIndex;

        //con jquery llamamos a un método get del controller
        $.ajax({
            url: url_,
            data: {},//formData,
            cache: false,
            contentType: false,
            processData: false,
            type: 'GET',
            async: false,
            success: function (data) {
                debugger;
                $("div#clientsList").html(data);
                $("a.page-link").removeAttr("href");
                //COLOCAMOS CHECK A LOS INPUT CUYO ID CLIENTE COINCIDA CON EL QUE TENEMOS EN EL ARRAY
                checkIfWasChecked();
                //Colocamos el texto que ingresamos, nuevamente como valor del input
                $("input#filter").val(_filter);
            },
            error: function (e) {
                console.log(e);
            }
        });
    }

    //ESTA FUNCION LE COLOCA CHECK SI YA LO HABIAMOS SELECCIONADO
    function checkIfWasChecked() {
        //ME QUEDO CON TODOS LOS CHECKBOX
        var checkBoxs = document.getElementsByClassName("clientGroup");

        for (var i = 0; i < checkBoxs.length; i++) {

            let existe = findValueInArray(checkBoxs[i].dataset.clientid, clientsIdArray);

            if (existe) {
                checkBoxs[i].checked = true;
            }
        }
    }

    //CON ESTA FUNCION VAMOS A LLENAR EL ARRAY DE ID'S SI ES QUE CARGAMOS LA TABLA CON CLIENTES SELECCIONADOS
    function fillArrayOfId() {
        //ME QUEDO CON TODOS LOS CHECKBOX
        var checkBoxs = document.getElementsByClassName("clientGroup");

        for (var i = 0; i < checkBoxs.length; i++) {

            let existe = findValueInArray(checkBoxs[i].dataset.clientid, clientsIdArray);

            if (checkBoxs[i].checked == true && existe == false) {
                clientsIdArray.push(checkBoxs[i].dataset.clientid);
            }
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
            - SI ESTÁ CHEQUEADO, AGREGO EL ID DEL CLIENTE AL ARRAY
            - SI NO ESTÁ CHEQUEADO, LO REMUEVO
         */

        let clientId = $(this)[0].dataset.clientid;
        let isChecked = $(this).is(":checked");

        //1 - PREGINTO SI ESTA CHEQUEADO
        if (isChecked)
            clientsIdArray.push(clientId)
        else {
            clientsIdArray = jQuery.grep(clientsIdArray, function (value) {
                return value != clientId;
            });
        }

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

    //EDITAMOS EL GRUPO USANDO AJAX
    $("input#editGroup").click(function () {
        debugger;
        let _groupId = $("input#groupId").val();
        let _groupName = $("input#Name").val();
        let _groupDescription = $("input#Description").val();

        var formData = new FormData();

        formData.append('groupId', _groupId);
        formData.append('groupName', _groupName);
        formData.append('groupDescription', _groupDescription);
        formData.append('clientsId', JSON.stringify(clientsIdArray));

        $.ajax({
            url: "/Groups/Edit",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            type: 'POST',
            async: true,
            success: function () {
                window.location.href = "/Groups/Index";
            },
            error: function (e) {
                alert(e);
            }
        });

    });

});
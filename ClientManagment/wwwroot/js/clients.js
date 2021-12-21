$(document).ready(function () {
    //$("input#DocumentNumber").on('input keydown keyup', function () {

    //});
    const expresiones = {
        nombre: /^[a-zA-ZÀ-ÿ\s]{1,40}$/, // Letras y espacios, pueden llevar acentos.
        dni: /^\d{8}$/ // 8numeros.
    }


    $("input#DocumentNumber").on('input', function () {
        //debugger;

        //NOS QUEDAMOS CON EL VALOR QUE INGRESAMOS
        let character = $(this).val();

        //ACA PONER EL CODIGO QUE ME BORRA LA LETRA SI ES QUE INGRESE UNA LETRA
        /*
         1 - nos fijamos si el valor ingresado es número
            a - si no es número, borra el caracter
            a - si es número no pasa nada
         */

        if (!Number(character)) { //! --> negado
            //aca reemplazamos lo que es letra por ''
            let newValue = $(this).val().replace(/[^0-9]/g, '');

            //acá reasignamos al input su valor sin la letra
            $(this).val(newValue);
        };
        //PRIMERO PREGUNTAS SI EL TIPO NO ES PASAPORTE, SI NO ES ENTONCES VALIDAR, SINO NO
        //FIJARSE SEGUN EL TIPO DE DOCUMENTO, CUANTOS CARACTERES DEJARLE AGREGAR
    });

    $("input#Name").on('input', function () {
        //TOMA EL VALOR
        let name = $(this).val();
        if (Number(name)) { //! --> negado
            //aca reemplazamos lo que es letra por ''
            let newValue = $(this).val().replace(/[0-9]/g, '');

            //acá reasignamos al input su valor sin la letra
            $(this).val(newValue);
        } else {
            //aca reemplazamos lo que es letra por ''
            let newValue = $(this).val().replace(/[0-9]/g, '');
            //acá reasignamos al input su valor sin la letra
            $(this).val(newValue);
        };
    });

    $("input#Surname").on('input', function () {
        let name = $(this).val();
        if (Number(name)) {
            //Si ingresa primero un numero lo reemplaza por ''
            let newValue = $(this).val().replace(/[0-9]/g, '');
            //acá reasignamos al input su valor sin la letra
            $(this).val(newValue);
        } else {
            //caso contrario, ingreso primero una letra y luegro un numero
            //aca verifica si ingreso una letra antes de un numero y luego
            //reemplaza el numero por ''
            let newValue = $(this).val().replace(/[0-9]/g, '');
            //acá reasignamos al input su valor sin la letra
            $(this).val(newValue);
        };
    });
    
    //$("input#isNotWished").on('change',function () {
      
    //})

});




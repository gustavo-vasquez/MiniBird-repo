$.validator.setDefaults({
    ignore: ""
});

$.validator.unobtrusive.adapters.add('filemaxsize', ['maxsize'], function (options) {
    var params = {
        maxSize: options.params.maxsize
    };
    options.rules['filemaxsize'] = params;
    options.messages['filemaxsize'] = options.message;
});

$.validator.addMethod('filemaxsize', function (value, element, params) {
    if (element.files.length > 0)
        return element.files[0].size <= params.maxSize;

    return true;
});

$.validator.unobtrusive.adapters.add('multiplefilesmaxsize', ['maxsize'], function (options) {
    var params = {
        maxSize: options.params.maxsize
    };
    options.rules['multiplefilesmaxsize'] = params;
    options.messages['multiplefilesmaxsize'] = options.message;
});

$.validator.addMethod('multiplefilesmaxsize', function (value, element, params) {
    var totalSize = 0;

    if (element.files.length > 0) {
        for (var i = 0; i < element.files.length; i++) {
            if (element.files[i].size > params.maxSize)
                return false;

            totalSize = totalSize + element.files[i].size;
        }

        if (totalSize > params.maxSize)
            return false;
    }

    return true;
});

$.validator.unobtrusive.adapters.add('collectionmaxlength', ['maxlength'], function (options) {
    var params = {
        maxLength: options.params.maxlength
    };
    options.rules['collectionmaxlength'] = params;
    options.messages['collectionmaxlength'] = options.message;
});

$.validator.addMethod('collectionmaxlength', function (value, element, params) {
    return element.files.length <= params.maxLength;
});

$.validator.unobtrusive.adapters.add('filevalidextension', ['extensions'], function (options) {
    var params = {
        extensions: JSON.parse(options.params.extensions)
    };
    options.rules['filevalidextension'] = params;
    options.messages['filevalidextension'] = options.message;
});

$.validator.addMethod('filevalidextension', function (value, element, params) {
    if (element.files.length > 0) {
        var extension = value.substring(value.lastIndexOf(".") + 1).toLowerCase();
        return $.inArray(extension, params.extensions) != -1;
    }

    return true;
});

$.validator.unobtrusive.adapters.add('multiplefilesvalidextension', ['extensions'], function (options) {
    var params = {
        extensions: JSON.parse(options.params.extensions)
    };
    options.rules['multiplefilesvalidextension'] = params;
    options.messages['multiplefilesvalidextension'] = options.message;
});

$.validator.addMethod('multiplefilesvalidextension', function (value, element, params) {
    if (element.files.length > 0) {
        for (var i = 0; i < element.files.length; i++) {
            var extension = element.files[i].name.substring(element.files[i].name.lastIndexOf(".") + 1).toLowerCase();
            if ($.inArray(extension, params.extensions) === -1)
                return false;
        }
    }

    return true;
});
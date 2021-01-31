function ShowModal(id) {
    var myModal = new bootstrap.Modal(document.getElementById(id));
    myModal.show();
}

function ShowModalStatic(id) {
    var myModal = new bootstrap.Modal(document.getElementById(id), {
        backdrop: 'static'
    });
    myModal.show();
}

function HideModal(id) {
    var myModalEl = document.getElementById(id);
    var modal = bootstrap.Modal.getInstance(myModalEl);
    modal.hide();
}

async function GetFormDataJson(formName)
{
    let form = document.forms[formName];

    let fd = new FormData(form);

    let data = {};

    for (let [key, prop] of fd) {

        console.log(key);
        console.log(prop);

        if (prop instanceof File) {
            console.log('We got here dude');
            data[key] = Array.from(new Uint8Array(await readFileAsData(prop)));

        } else {
            data[key] = prop;
        }
        
    }

    data = JSON.stringify(data, null, 2);

    console.log(data);
    return data;
}

async function readFileAsData(file) {
    let result_base64 = await new Promise((resolve) => {
        let fileReader = new FileReader();
        fileReader.onload = (e) => resolve(fileReader.result);
        fileReader.readAsArrayBuffer(file);
    });

    console.log(result_base64);

    return result_base64;
}
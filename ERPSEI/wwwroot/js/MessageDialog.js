const MSG_TYPE_ALERT = 0;
const MSG_TYPE_ERROR = 1;
const MSG_TYPE_INFO = 2;
const MSG_TYPE_QUESTION = 3;
const MSG_TYPE_OK = 4;

var messageDialog = null;

const alertIcon = `<i class="d-block bi bi-exclamation-circle-fill me-3" style="font-size: 55px"></i>`;
const errorIcon = `<i class="d-block bi bi-x-circle-fill me-3" style="font-size: 55px"></i>`;
const questionIcon = `<i class="d-block bi bi-question-circle-fill me-3" style="font-size: 55px"></i>`;
const infoIcon = `<i class="d-block bi bi-info-circle-fill me-3" style="font-size: 55px"></i>`;
const okIcon = `<i class="d-block bi bi-check-circle-fill me-3" style="font-size: 55px"></i>`;

const messageDialogHTML = `<div id="messageDialog" class="modal" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1">
                             <div class="modal-dialog modal-dialog-centered">
                               <div class="modal-content shadow-lg">
                                 <div class="modal-header">
                                   <h5 id="titleContainer" class="modal-title"></h5>
                                   <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                 </div>
                                 <div id="modalBody" class="modal-body d-flex align-items-center"></div>
                                 <div id="buttonsContainer" class="modal-footer">
                                 </div>
                               </div>
                             </div>
                         </div>`;

document.addEventListener("DOMContentLoaded", async function (event) {
    let messageDialogDOM = document.getElementById('messageDialog');
    if (messageDialogDOM == null) {
        document.body.innerHTML += messageDialogHTML;
        messageDialogDOM = document.getElementById('messageDialog');
    }
    messageDialog = new bootstrap.Modal(messageDialogDOM);
});

function showMessage(title, message, type, funcOK, funcCancel) {
    let titleContainer = document.getElementById('titleContainer');
    let modalBody = document.getElementById('modalBody');
    let icon = "";
    let buttonsContainer = document.getElementById('buttonsContainer');

    titleContainer.innerHTML = title;

    let cancelbutton = null;
    let color = "";

    switch (type) {
        case MSG_TYPE_ALERT:
            color = "warning";
            icon = alertIcon;
            break;
        case MSG_TYPE_ERROR:
            color = "danger";
            icon = errorIcon;
            break;
        case MSG_TYPE_OK:
            color = "success";
            icon = okIcon;
            break;
        case MSG_TYPE_QUESTION:
            color = "primary";
            icon = questionIcon;
            break;
        default:
            color = "info";
            icon = infoIcon;
            break;
    };

    modalBody.innerHTML = `<div class="w-100 alert alert-${color} align-items-center"><center>${icon}<div class="d-block">${message}</div></center></div>`;

    if (type == MSG_TYPE_QUESTION) {
        buttonsContainer.innerHTML = `<button id="cancelbutton" type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                                      <button id="okbutton" type="button" class="btn btn-primary" data-bs-dismiss="modal">Confirmar</button>`;
    }
    else {
        buttonsContainer.innerHTML = `<button id="okbutton" type="button" class="btn btn-primary" data-bs-dismiss="modal">Ok</button>`;
    }

    let okbutton = document.getElementById('okbutton');

    if (typeof funcOK === 'function' && okbutton != null) {
        okbutton.addEventListener('click', funcOK);
    }
    if (typeof funcCancel === 'function' && cancelbutton != null) {
        cancelbutton.addEventListener('click', funcCancel);
    }
    

    messageDialog.show();
}

function showAlert(title, message, funcOK) {
    showMessage(title, message, MSG_TYPE_ALERT, funcOK, null);
}
function showError(title, message, funcOK) {
    showMessage(title, message, MSG_TYPE_ERROR, funcOK, null);
}
function showInfo(title, message, funcOK) {
    showMessage(title, message, MSG_TYPE_INFO, funcOK, null);
}
function showSuccess(title, message, funcOK) {
    showMessage(title, message, MSG_TYPE_OK, funcOK, null);
}
function askConfirmation(title, message, funcOK, funcCancel) {
    showMessage(title, message, MSG_TYPE_QUESTION, funcOK, funcCancel);
}
function hideMessage() {
    let titleContainer = document.getElementById('titleContainer');
    let modalBody = document.getElementById('modalBody');
    let buttonsContainer = document.getElementById('buttonsContainer');

    messageDialog.hide();

    titleContainer.innerHTML = '';
    modalBody.innerHTML = '';
    buttonsContainer.innerHTML = '';

    setTimeout(function () {
        let modal = document.getElementById('messageDialog');
        if (modal.classList.contains('show')) {
            hideMessage();
        }
    }, 200);
}
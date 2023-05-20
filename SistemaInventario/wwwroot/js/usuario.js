var datatable;
$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros Por Pagina",
            "zeroRecords": "Ningun Registro",
            "info": "Mostrar pagina _PAGE_ de _PAGES_",
            "infoEmpty": "no hay registros",
            "infoFiltered": "(filtered from _MAX_ total registros)",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "ajax": {
            "url":"/Admin/Usuario/ObtenerTodos"
        },
        "columns": [
            { "data": "userName", "width": "10%" },
            { "data": "nombres", "width": "20%" },
            { "data": "apellidos", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "role", "width": "10%" },
            {
                "data": {
                    id: "id",
                    lockoutEnd:"lockoutEnd"
                },
                "render": function (data) {

                    const hoy = new Date().getTime();
                    const bloqueo = new Date(data.lockoutEnd).getTime();
                    if (bloqueo > hoy) {
                        //usuario esta bloqueado
                        return `
                                <div class="text-center">
                                
                                    <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-danger text-white" style="cursor:pointer">
                                        <i class="fas fa-lock-open"></i> Desbloquear
                                    </a>
                                </div>
                                `;
                    } else {
                        return `
                                <div class="text-center">
                                
                                    <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-success text-white" style="cursor:pointer">
                                        <i class="fas fa-lock"></i> Bloquear
                                    </a>
                                </div>
                                `;
                    }
                },
                "width":"20%"
            }

        ]
    });
}

function BloquearDesbloquear( id) {

    $.ajax({
        type: "POST",
        url: '/Admin/Usuario/BloquearDesbloquear',
        data: JSON.stringify(id),
        contentType:"application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                datatable.ajax.reload();
            } else {
                toastr.error(data.message);
            }
        }
    });

}
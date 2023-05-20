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
            "url":"/Inventario/Inventario/ObtenerHistorial"
        },
        "columns": [
            { "data": "fechaInicial", "width": "15%", "render": (data) => { return new Date(data).toLocaleString() } },
            { "data": "fechaFinal", "width": "15%", "render": (data) => { return new Date(data).toLocaleString() } },
            { "data": "bodega.nombre", "width": "15%" },
            { "data": (data) => data.usuarioAplicacion.nombres + " " + data.usuarioAplicacion.apellidos, "width": "30%" },
            
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Inventario/Inventario/DetalleHistorial/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                                    Detalle
                                </a>
                                
                            </div>
                            `;
                },
                "width":"10%"
            }

        ]
    });
}

function Delete( url) {
    swal({
        title: "¿Esta seguro de eliminar la Categoria?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true

    })
        .then(borrar => {
            if (borrar) {
                $.ajax({
                    type: "DELETE",
                    url,
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
        });
}
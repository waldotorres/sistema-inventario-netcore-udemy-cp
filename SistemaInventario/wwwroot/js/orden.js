var datatable;
$(document).ready(function () {

    const urlParams = new URLSearchParams(window.location.search);
    const estado = urlParams.get("estado");

    if (estado != "") {
        loadDataTable(`ObtenerOrdenLista?estado=${estado}`);
    } else {
        loadDataTable(`ObtenerOrdenLista`);
    }
});


function loadDataTable(url) {
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
            "url": `/Admin/Orden/${url}`
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "nombresCliente", "width": "20%" },
            { "data": "telefono", "width": "10%" },
            { "data": "usuarioAplicacion.email", "width": "15%" },
            { "data": "estadoOrden", "width": "10%" },
            { "data": "totalOrden", "width": "10%" },

            
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/Orden/Detalle/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-list-ul"></i>
                                </a>                                
                            </div>
                            `;
                },
                "width": "10%"
            }

        ]
    });
}
 
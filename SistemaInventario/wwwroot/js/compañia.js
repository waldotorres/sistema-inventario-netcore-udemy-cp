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
            "url":"/Admin/Compañia/ObtenerTodos"
        },
        "columns": [
            { "data": "nombre", "width": "15%" },
            { "data": "descripcion", "width": "15%" },
            { "data": "pais", "width": "15%" },
            { "data": "ciudad", "width": "15%" },
            { "data": "telefono", "width": "15%" },
          
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/Compañia/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i>
                                </a>                                
                            </div>
                            `;
                },
                "width":"20%"
            }

        ]
    });
}
 
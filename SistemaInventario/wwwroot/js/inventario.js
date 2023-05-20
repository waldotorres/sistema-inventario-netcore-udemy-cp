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
            "url":"/Inventario/Inventario/ObtenerTodos"
        },
        "columns": [
            { "data": "bodega.nombre", "width": "20%" },
            { "data": "producto.descripcion", "width": "30%" },
            { "data": "producto.costo", "width": "30%", "className": "text-right" },
            { "data": "cantidad", "width": "10%", "className":"text-right" }
        ]
    });
}
 
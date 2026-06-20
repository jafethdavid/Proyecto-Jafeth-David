let total = 0;

function agregarProducto() {

    let select = document.getElementById("productoSelect");

    let id = select.value;

    let nombre = select.options[select.selectedIndex].text;

    let precio = parseFloat(select.options[select.selectedIndex].dataset.precio);

    let stock = parseInt(select.options[select.selectedIndex].dataset.stock);

    let cantidad = parseInt(document.getElementById("cantidad").value);


    // VALIDAR PRODUCTO REPETIDO
    let filas = document.querySelectorAll("#tablaVenta tbody tr");

    for (let fila of filas) {
        let productoExistente = fila.dataset.id;

        if (productoExistente == id) {
            document.getElementById("alertaStock").innerHTML =
                `<div class="alert alert-warning alert-dismissible fade show">

Este producto ya está en la lista.

<button type="button" class="btn-close" data-bs-dismiss="alert"></button>

</div>`;

            return;
        }
    }


    // VALIDAR STOCK
    if (cantidad > stock) {
        document.getElementById("alertaStock").innerHTML =
            `<div class="alert alert-danger alert-dismissible fade show">

Stock insuficiente. Disponible: ${stock}

<button type="button" class="btn-close" data-bs-dismiss="alert"></button>

</div>`;

        return;
    }

    let subtotal = precio * cantidad;

    let index = document.querySelectorAll("#tablaVenta tbody tr").length;

    let fila = `
<tr data-id="${id}">

<td>${nombre}</td>

<td>${cantidad}</td>

<td>${precio}</td>

<td>${subtotal}</td>

<td>

<button type="button"
class="btn btn-danger btn-sm"
onclick="eliminarFila(this, ${subtotal})">

Eliminar

</button>

</td>

<input type="hidden"
name="Detalles[${index}].ProductoId"
value="${id}">

<input type="hidden"
name="Detalles[${index}].Cantidad"
value="${cantidad}">

<input type="hidden"
name="Detalles[${index}].PrecioUnitario"
value="${precio}">

</tr>
`;

    document.querySelector("#tablaVenta tbody").innerHTML += fila;

    total += subtotal;

    document.getElementById("total").innerText = total;

    document.getElementById("cantidad").value = 1;

    document.getElementById("alertaStock").innerHTML = "";

}


function eliminarFila(btn, subtotal) {

    btn.closest("tr").remove();

    total -= subtotal;

    document.getElementById("total").innerText = total;

}


window.onload = function () {

    let alerta = document.getElementById("alertaVenta");

    if (alerta) {
        setTimeout(function () {

            alerta.classList.remove("show");
            alerta.classList.add("fade");

            setTimeout(() => alerta.remove(), 500);

        }, 5000);
    }

};
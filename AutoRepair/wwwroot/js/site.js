async function GetCars(ownerID) {
    const button = document.getElementById('btCars');

    const response = await fetch(`/profile/Car/GetCars?Status=${button.value == '1' ? true : false}&ID=${ownerID}`);

    if (response.ok) {
        const list = await response.json();

        console.log(list);

        const tbody = document.getElementById('carTable');

        tbody.innerHTML = '';

        for (const car of list) {
            // Name
            const tdName = document.createElement('td');
            tdName.innerText = car.Name;

            // Color
            const tdColor = document.createElement('td');
            tdColor.innerText = car.Color;

            // LicensePlate
            const tdLicensePlate = document.createElement('td');
            tdLicensePlate.innerText = car.LicensePlate;

            const tdLinks = document.createElement('td');

            if (button.value == '1') {
                // Edit link
                const editLink = document.createElement('a');
                editLink.innerText = 'Edit';
                editLink.className = 'btn btn-light btn-outline-secondary';
                editLink.href = '/profile/Car/Edit/' + car.ID;

                // Delete link
                const deleteLink = document.createElement('a');
                deleteLink.innerText = 'Delete';
                deleteLink.className = 'btn btn-outline-danger';
                deleteLink.href = '/profile/Car/Delete/' + car.ID;

                tdLinks.appendChild(editLink);
                tdLinks.appendChild(deleteLink);
            }

            const parent = document.createElement('tr');
            const childs = [tdName, tdColor, tdLicensePlate, tdLinks];

            for (const child of childs) {
                console.log(child);
                parent.appendChild(child);
            }

            tbody.appendChild(parent);
        }

        if (button.value == '1') {
            button.innerText = 'Archieved Cars';
            button.value = '0';
        }
        else {
            button.innerText = 'Active Cars';
            button.value = '1';
        }
    }
    else
        console.log("Error HTTP: " + response.status);

}

async function ChangeOrderStatus(type, orderID) {
    const response = type == 'start' ?
        await fetch('/profile/Order/Start/' + orderID, { method: 'POST' }) :
            type == 'finish' ?
                await fetch('/profile/Order/Finish/' + orderID, { method: 'POST' }) :
                await fetch('/profile/Order/Pay/' + orderID, { method: 'POST' });

    if (response.ok)
        location.reload();
    else
        console.log("Error HTTP: " + response.status);
}

async function GetServices(masterID) {
    const response = await fetch('/profile/Service/GetServices?masterID=' + masterID);

    if (response.ok) {
        const list = await response.json();

        const selectList = document.getElementById("servicesList");

        selectList.innerHTML = "";

        for (const service of list) {
            const option = document.createElement("option");
            option.value = service.SpecialtyID;
            option.text = service.Name;

            selectList.appendChild(option);
        }
    }
    else
        console.log("Error HTTP: " + response.status);
}